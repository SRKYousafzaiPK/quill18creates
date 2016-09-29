using UnityEngine;
using System.Collections;

public class BotMovement : MonoBehaviour {

	// This script is only for "my bot" -- in other words, only the "local" client will have this
	// enabled.  In practice, this means the MASTER client -- which is probably responsible for
	// spawning bots.
	// REMOTE bots will have this script disabled.

	NetworkCharacter netChar;

	// Use this for initialization
	void Start () {
		netChar = GetComponent<NetworkCharacter>();
	}
	
	// Update is called once per frame
	void Update () {
		netChar.isJumping = true;
	}
}
