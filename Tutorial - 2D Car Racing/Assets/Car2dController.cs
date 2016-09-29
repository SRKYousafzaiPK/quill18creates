using UnityEngine;
using System.Collections;

public class Car2dController : MonoBehaviour {

	float speedForce = 15f;
	float torqueForce = -200f;
	float driftFactorSticky = 0.9f;
	float driftFactorSlippy = 1;
	float maxStickyVelocity = 2.5f;
	float minSlippyVelocity = 1.5f;	// <--- Exercise for the viewer

	// Use this for initialization
	void Start () {
	
	}

	void Update() {
		// check for button up/down here, then set a bool that you will use in FixedUpdate
	}
	
	// Update is called once per frame
	void FixedUpdate () {


		Rigidbody2D rb = GetComponent<Rigidbody2D>();

		Debug.Log(RightVelocity().magnitude);

		float driftFactor = driftFactorSticky;
		if(RightVelocity().magnitude > maxStickyVelocity) {
			driftFactor = driftFactorSlippy;
		}

		rb.velocity = ForwardVelocity() + RightVelocity()*driftFactor;

		if(Input.GetButton("Accelerate")) {
			rb.AddForce( transform.up * speedForce );

			// Consider using rb.AddForceAtPosition to apply force twice, at the position
			// of the rear tires/tyres
		}
		if(Input.GetButton("Brakes")) {
			rb.AddForce( transform.up * -speedForce/2f );

			// Consider using rb.AddForceAtPosition to apply force twice, at the position
			// of the rear tires/tyres
		}

		// If you are using positional wheels in your physics, then you probably
		// instead of adding angular momentum or torque, you'll instead want
		// to add left/right Force at the position of the two front tire/types
		// proportional to your current forward speed (you are converting some
		// forward speed into sideway force)
		float tf = Mathf.Lerp(0, torqueForce, rb.velocity.magnitude / 2);
		rb.angularVelocity = Input.GetAxis("Horizontal") * tf;



	}

	Vector2 ForwardVelocity() {
		return transform.up * Vector2.Dot( GetComponent<Rigidbody2D>().velocity, transform.up );
	}

	Vector2 RightVelocity() {
		return transform.right * Vector2.Dot( GetComponent<Rigidbody2D>().velocity, transform.right );
	}


}
