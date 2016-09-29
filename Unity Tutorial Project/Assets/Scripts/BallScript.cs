using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
	
	public AudioClip[] blipAudio;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Die() {
		Destroy( gameObject );
		GameObject paddleObject = GameObject.Find("paddle");
		PaddleScript paddleScript = paddleObject.GetComponent<PaddleScript>();
		paddleScript.LoseLife();
	}
	
	void OnCollisionEnter( Collision collision ) {
		// Play a blip???
		AudioSource.PlayClipAtPoint(blipAudio[ Random.Range(0, blipAudio.Length) ], transform.position, .25f);
	}
}
