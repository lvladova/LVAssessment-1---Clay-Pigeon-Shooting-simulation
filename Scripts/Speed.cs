using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speed : MonoBehaviour {
	public static float speed = 0; // creats speed starting from 0
	public Text pigeonSpeed; // conects the text to the speed value that will be set when a game object is destroyed
	
	// Start is called before the first frame update
	void Start() {
		pigeonSpeed = GetComponent<Text>();
	}

	private void Update() {
		//updates the speed accordingly
		pigeonSpeed.text = "Speed:" + speed + "m/s";
	}
}
