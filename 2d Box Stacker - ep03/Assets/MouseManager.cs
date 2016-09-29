using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {

	public LineRenderer dragLine;

	float dragSpeed = 4f;

	Rigidbody2D grabbedObject = null;

	void Update() {
		if( Input.GetMouseButtonDown(0) ) {
			// We clicked, but on what?
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

			Vector2 dir = Vector2.zero;

			RaycastHit2D hit = Physics2D.Raycast(mousePos2D, dir);
			if(hit != null && hit.collider!=null) {
				// We clicked on SOMETHING that has a collider
				if(hit.collider.rigidbody2D != null) {
					grabbedObject = hit.collider.rigidbody2D;
					grabbedObject.gravityScale = 0;
					dragLine.enabled = true;
				}
			}
		}

		if( Input.GetMouseButtonUp(0) && grabbedObject!=null ) {
			grabbedObject.gravityScale = 1;
			grabbedObject = null;
			dragLine.enabled = false;
		}
	}

	void FixedUpdate () {
		if(grabbedObject != null) {
			// Move the object with the mouse
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);

			Vector2 dir = mousePos2D - grabbedObject.position;

			dir *= dragSpeed;

			grabbedObject.velocity = dir;

		}
	}

	void LateUpdate() {
		if(grabbedObject != null) {
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
			dragLine.SetPosition(0, new Vector3(grabbedObject.position.x, grabbedObject.position.y, -1));
			dragLine.SetPosition(1, new Vector3(mousePos2D.x, mousePos2D.y, -1));
		}
	}

}
