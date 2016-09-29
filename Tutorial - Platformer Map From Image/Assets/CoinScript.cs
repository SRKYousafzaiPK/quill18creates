using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D col) {
		Destroy(gameObject);
	}

}
