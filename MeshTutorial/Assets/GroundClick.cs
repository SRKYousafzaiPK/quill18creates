using UnityEngine;
using System.Collections;

public class GroundClick : MonoBehaviour {
	
	public GameObject prefabNode;
	public GameObject prefabRoad;
	
	GameObject nodeStart;
	
	void Update() {
		if(Input.GetMouseButtonDown(0)) {
			Vector3 roadStart;
			if( ClickLocation(out roadStart) ) {
				nodeStart = (GameObject)Instantiate(prefabNode, roadStart, Quaternion.identity);
				nodeStart.GetComponent<NodeClick>().ground = this;
			}
		}
		
		if(Input.GetMouseButtonUp(0)) {
			Vector3 roadEnd;
			
			if( nodeStart!=null && ClickLocation(out roadEnd) ) {
				GameObject nodeEnd = (GameObject)Instantiate(prefabNode, roadEnd, Quaternion.identity);
				nodeEnd.GetComponent<NodeClick>().ground = this;
				
				CreateRoad(nodeStart.transform.position, nodeEnd.transform.position);
			}
			else if ( nodeStart != null && ClickLocationNode(out roadEnd) ) {
				CreateRoad(nodeStart.transform.position, roadEnd);
			}
			
			nodeStart = null;
		}
	}
	
	public void SetNodeStart(GameObject n) {
		Debug.Log ("NodeStart: " + n.transform.position);
		nodeStart = n;
	}
	
	public void SetNodeEnd(GameObject n) {
		Debug.Log ("NodeStart: " + nodeStart.transform.position);
		Debug.Log ("NodeEnd: " + n.transform.position);
		CreateRoad(nodeStart.transform.position, n.transform.position);
		nodeStart = null;
	}
	
	bool ClickLocation(out Vector3 point) {
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		
		RaycastHit hitInfo = new RaycastHit();
		if( Physics.Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			if( hitInfo.collider == collider ) {
				point = hitInfo.point;
				return true;
			}
		}
		point = Vector3.zero;
		return false;
	}
		
	bool ClickLocationNode(out Vector3 point) {
		Debug.Log ("ClickLocationNode");
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		
		RaycastHit hitInfo = new RaycastHit();
		if( Physics.Raycast( ray, out hitInfo, Mathf.Infinity ) ) {
			Debug.Log (hitInfo.collider.gameObject.name);
			if( hitInfo.collider.transform.parent!=null && hitInfo.collider.transform.parent.tag == "Node" ) {
				point = hitInfo.collider.transform.position;
				return true;
			}
		}
		point = Vector3.zero;
		Debug.Log ("false");
		return false;
	}
		
	void CreateRoad( Vector3 roadStart, Vector3 roadEnd ) {
		float width = 1;
		float length = Vector3.Distance(roadStart, roadEnd);
		
		if(length < 1) {
			return;
		}
		
		GameObject road = (GameObject)Instantiate(prefabRoad);
		road.transform.position = roadStart + new Vector3(0, 0.01f, 0);
		
		road.transform.rotation = Quaternion.FromToRotation( Vector3.right, roadEnd - roadStart );
		Debug.Log (road.transform.rotation.eulerAngles);
	
		Vector3[] vertices = {
				new Vector3(0, 		0, -width/2),
				new Vector3(length, 0, -width/2),
				new Vector3(length, 0,  width/2),
				new Vector3(0, 		0,  width/2)
			};

		int[] triangles = {
				1, 0, 2,	// triangle 1
				2, 0, 3		// triangle 2
			};
		

		Vector2[] uv = {
				new Vector2(0, 0),
				new Vector2(length, 0),
				new Vector2(length, 1),
				new Vector2(0, 1)
			};
		
		Vector3[] normals = {
				Vector3.up,
				Vector3.up,
				Vector3.up,
				Vector3.up
			};
		
		Mesh mesh = new Mesh();

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.normals = normals;
		
		MeshFilter mesh_filter = road.GetComponent<MeshFilter>();
		mesh_filter.mesh = mesh;
		
		
	}
	
}
