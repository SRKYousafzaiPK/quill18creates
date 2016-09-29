using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

	public Vector2 velocity;

	void FixedUpdate () {
		rigidbody2D.velocity = velocity;
	}
}
