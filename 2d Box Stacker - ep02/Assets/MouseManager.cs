using UnityEngine;
using System.Collections;

public class MouseManager : MonoBehaviour {

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
				}
			}
		}

		if( Input.GetMouseButtonUp(0) ) {
			grabbedObject = null;
		}
	}

	void FixedUpdate () {
		if(grabbedObject != null) {
			// Move the object with the mouse
			Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2D = new Vector2(mouseWorldPos3D.x, mouseWorldPos3D.y);
			grabbedObject.position = mousePos2D;
		}
	}

}
