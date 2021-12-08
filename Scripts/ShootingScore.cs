using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingScore: MonoBehaviour {
	public static int scoreValue = 0; // creats a score value starting from 0
	public Text shootingScore; // conects the text to the shooting score value that will be set when a game object is destroyed

	// Start is called before the first frame update
	void Start() {
		shootingScore = GetComponent<Text>();
	}

	private void Update() {
		//updates the score accordingly to the event
		shootingScore.text = "Score:" + scoreValue;
		}
}
