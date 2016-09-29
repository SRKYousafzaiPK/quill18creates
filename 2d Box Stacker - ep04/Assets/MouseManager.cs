using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {

	public LineRenderer dragLine;

	float dragSpeed = 4f;

	Rigidbody2D grabbedObject = null;
	SpringJoint2D springJoint = null;

	void Update() {
		if( Input.GetMouseButtonDown(0) ) {
			// We clicked, but on what?
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

			Vector2 dir = Vector2.zero;

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, dir);
			if(hit.collider!=null) {
				// We clicked on SOMETHING that has a collider
				if(hit.collider.rigidbody2D != null) {
					grabbedObject = hit.collider.rigidbody2D;

					springJoint = grabbedObject.gameObject.AddComponent<SpringJoint2D>();
					// Set the anchor to the spot on the object that we clicked.
					Vector3 localHitPoint = grabbedObject.transform.InverseTransformPoint(hit.point);
					springJoint.anchor = localHitPoint;
					springJoint.connectedAnchor = mouseWorldPos3D;
					springJoint.distance = 0.5f;
					springJoint.dampingRatio = 1;
					springJoint.frequency = 1;

					// Enable this if you want to collide with objects still (and you probably do)
					// This will also WAKE UP the spring.
					springJoint.collideConnected = true;

					// This will also WAKE UP the spring, even if it's a totally
					// redundant line because the connectedBody should already be null
					springJoint.connectedBody = null;

					dragLine.enabled = true;
				}
			}
		}

		if( Input.GetMouseButtonUp(0) && grabbedObject!=null ) {
			Destroy(springJoint);
			springJoint = null;
			grabbedObject = null;
			dragLine.enabled = false;
		}

	}


	void FixedUpdate () {
		if(springJoint != null) {
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			springJoint.connectedAnchor = mouseWorldPos3D;
		}
	}

	void LateUpdate() {
		if(springJoint != null) {

			Vector3 worldAnchor = grabbedObject.transform.TransformPoint(springJoint.anchor);

			dragLine.SetPosition(0, new Vector3(worldAnchor.x, worldAnchor.y, -1));
			dragLine.SetPosition(1, new Vector3(springJoint.connectedAnchor.x, springJoint.connectedAnchor.y, -1));
		}
	}

}
