using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// This component is only enabled for "my player" (i.e. the character belonging to the local client machine).
	// This script is responsible for reading input commands from the player
	// and then passing that info to NetworkCharacter, which is responsible for
	// actually moving things.

	NetworkCharacter netChar;
	Animator anim;

	// Use this for initialization
	void Start () {
		netChar = GetComponent<NetworkCharacter>();
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {

		// WASD forward/back & left/right movement is stored in "direction"
		netChar.direction = transform.rotation * new Vector3( Input.GetAxis("Horizontal") , 0, Input.GetAxis("Vertical") );

		// This ensures that we don't move faster going diagonally
		if(netChar.direction.magnitude > 1f) {
			netChar.direction = netChar.direction.normalized;
		}

		// Set our animation "Speed" parameter. This will move us from "idle" to "run" animations,
		// but we could also use this to blend between "walk" and "run" as well.
		anim.SetFloat("Speed", netChar.direction.magnitude);

		// If we're on the ground and the player wants to jump, set
		// verticalVelocity to a positive number.
		// If you want double-jumping, you'll want some extra code
		// here instead of just checking "cc.isGrounded".
		if(Input.GetButton("Jump")) {
			netChar.isJumping = true;
		}
		else {
			netChar.isJumping = false;
		}

		AdjustAimAngle();
	}

	void AdjustAimAngle() {
		Camera myCamera = this.GetComponentInChildren<Camera>();

		if(myCamera==null) {
			Debug.LogError("Why doesn't my character have a camera?  This is an FPS!");
			return;
		}

		float AimAngle = 0;

		if(myCamera.transform.rotation.eulerAngles.x <= 90f) {
			// We are looking DOWN
			AimAngle = -myCamera.transform.rotation.eulerAngles.x;
		}
		else {
			AimAngle = 360 - myCamera.transform.rotation.eulerAngles.x;
		}

		anim.SetFloat("AimAngle", AimAngle);
	}

}
