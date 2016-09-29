using UnityEngine;
using System.Collections;

public class BrickScript : MonoBehaviour {
	
	static int numBricks = 0;
	public int pointValue = 1;
	public int hitPoints = 1;
	public int powerUpChance = 3;
	
	public GameObject[] powerUpPrefabs;

	// Use this for initialization
	void Start () {
		numBricks++;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter( Collision col ) {
		hitPoints--;
		
		if ( hitPoints <= 0 ) {
			Die();
		}
	}
	
	void Die() {
		Destroy( gameObject );
		PaddleScript paddleScript = GameObject.Find ("paddle").GetComponent<PaddleScript>();
		paddleScript.AddPoint(pointValue);
		numBricks--;
		
		if ( Random.Range(0, powerUpChance) == 0 ) {
			Instantiate( powerUpPrefabs[ Random.Range(0, powerUpPrefabs.Length) ] , transform.position, Quaternion.identity );
		}

		if ( numBricks <= 0 ) {
			// Load a new level??
			Application.LoadLevel("level2");
		}
	}
}
