using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScorePanelUpdater : MonoBehaviour {

	void Start() {
		
	}

	void Update () {
/*		// This a (relatively slow/bad) way to grab the GameStatus
		GameObject go = GameObject.Find("GameStatus");
		if(go == null) {
			Debug.LogError("Failed to find an object named 'GameStatus'");
			this.enabled = false;
			return;
		}

		// It's the GameStatus script that we actually care about.
		GameStatus gs = go.GetComponent<GameStatus>();

		GetComponent<Text>().text = "Score: " + gs.GetScore() + " Lives: " + gs.GetLives();
*/

		GetComponent<Text>().text = "Score: " + GameStatus.GetInstance().GetScore() + " Lives: " + GameStatus.GetInstance().GetLives();

	}
}
