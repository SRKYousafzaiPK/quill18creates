using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

	// This script is responsible for actually moving a character.
	// For local character, we read things like "direction" and "isJumping"
	// and then affect the character controller.
	// For remote characters, we skip that and simply update the raw transform
	// position based on info we received over the network.


	// NOTE! Only our local character will effectively use this.
	// Remove character will just give us absolute positions.
	public float speed = 10f;		// The speed at which I run
	public float jumpSpeed = 6f;	// How much power we put into our jump. Change this to jump higher.

	// Bookeeping variables
	[System.NonSerialized]
	public Vector3 direction = Vector3.zero;	// forward/back & left/right
	[System.NonSerialized]
	public bool isJumping = false;
	float   verticalVelocity = 0;		// up/down

	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;
	float realAimAngle = 0;

	Animator anim;

	bool gotFirstUpdate = false;

	CharacterController cc;


	// Use this for initialization
	void Start () {
		CacheComponents();
	}

	void CacheComponents() {
		if(anim == null) {
			anim = GetComponent<Animator>();
			if(anim == null) {
				Debug.LogError ("ZOMG, you forgot to put an Animator component on this character prefab!");
			}

			cc = GetComponent<CharacterController>();
			if(cc == null) {
				Debug.LogError("No character controller!");
			}
		}

		// Cache more components here if required!
	}
	
	// FixedUpdate is called once per physics loop
	// Do all MOVEMENT and other physics stuff here.
	void FixedUpdate () {
		if( photonView.isMine ) {
			// Do nothing -- the character motor/input/etc... is moving us
			DoLocalMovement();
		}
		else {
			transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
			anim.SetFloat("AimAngle", Mathf.Lerp(anim.GetFloat("AimAngle"), realAimAngle, 0.1f ) );
		}
	}

	void DoLocalMovement () {
		
		// "direction" is the desired movement direction, based on our player's input
		Vector3 dist = direction * speed * Time.deltaTime;

		if(isJumping) {
			isJumping = false;
			if(cc.isGrounded) {
				verticalVelocity = jumpSpeed;
			}
		}

		if(cc.isGrounded && verticalVelocity < 0) {
			// We are currently on the ground and vertical velocity is
			// not positive (i.e. we are not starting a jump).
			
			// Ensure that we aren't playing the jumping animation
			anim.SetBool("Jumping", false);
			
			// Set our vertical velocity to *almost* zero. This ensures that:
			//   a) We don't start falling at warp speed if we fall off a cliff (by being close to zero)
			//   b) cc.isGrounded returns true every frame (by still being slightly negative, as opposed to zero)
			verticalVelocity = Physics.gravity.y * Time.deltaTime;
		}
		else {
			// We are either not grounded, or we have a positive verticalVelocity (i.e. we ARE starting a jump)
			
			// To make sure we don't go into the jump animation while walking down a slope, make sure that
			// verticalVelocity is above some arbitrary threshold before triggering the animation.
			// 75% of "jumpSpeed" seems like a good safe number, but could be a standalone public variable too.
			//
			// Another option would be to do a raycast down and start the jump/fall animation whenever we were
			// more than ___ distance above the ground.
			if(Mathf.Abs(verticalVelocity) > jumpSpeed*0.75f) {
				anim.SetBool("Jumping", true);
			}
			
			// Apply gravity.
			verticalVelocity += Physics.gravity.y * Time.deltaTime;
		}
		
		// Add our verticalVelocity to our actual movement for this frame
		dist.y = verticalVelocity * Time.deltaTime;

		// Set our animation "Speed" parameter. This will move us from "idle" to "run" animations,
		// but we could also use this to blend between "walk" and "run" as well.
		anim.SetFloat("Speed", direction.magnitude);

		// Apply the movement to our character controller (which handles collisions for us)
		cc.Move( dist );
	}





	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		CacheComponents();

		if(stream.isWriting) {
			// This is OUR player. We need to send our actual position to the network.

			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation);
			stream.SendNext(anim.GetFloat("Speed"));
			stream.SendNext(anim.GetBool("Jumping"));
			stream.SendNext(anim.GetFloat("AimAngle"));
		}
		else {
			// This is someone else's player. We need to receive their position (as of a few
			// millisecond ago, and update our version of that player.

			// Right now, "realPosition" holds the other person's position at the LAST frame.
			// Instead of simply updating "realPosition" and continuing to lerp,
			// we MAY want to set our transform.position to immediately to this old "realPosition"
			// and then update realPosition


			realPosition = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext();
			anim.SetFloat("Speed", (float)stream.ReceiveNext());
			anim.SetBool("Jumping", (bool)stream.ReceiveNext());
			realAimAngle = (float)stream.ReceiveNext();

			if(gotFirstUpdate == false) {
				transform.position = realPosition;
				transform.rotation = realRotation;
				anim.SetFloat("AimAngle", realAimAngle );
				gotFirstUpdate = true;
			}

		}

	}
}
