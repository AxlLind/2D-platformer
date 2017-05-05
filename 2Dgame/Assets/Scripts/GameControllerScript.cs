﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class GameControllerScript : MonoBehaviour {

	public static bool isPaused;
	public GameObject nameInput;
	public Button sendHighscore;
	public Text scoreText;
	public Rigidbody2D playerRB;
	public int score;

	void Start() {
		isPaused = false;
		sendHighscore.interactable = true;
	}

	void Update () {
		if (isPaused) {
			return;
		}
		UpdateScore ();
	}

	/**
	 * Updates the UI-text object showing the score
	 * Takes the score simply from the camera's y-position.
	 */
	void UpdateScore() {
		score = (int) Camera.main.transform.position.y;
		scoreText.text = "Score: " + score;
	}

	/**
	 * Pauses and unpauses the game (depending on the current state).
	 * Does this by the field 'isPaused' and by turning off 'simulated'
	 * on the player.
	 */
	public void PauseUnPauseGame() {
		isPaused = !isPaused;
		playerRB.simulated = isPaused ? false : true;

	}

	/**
	 * Generates a random long number
	 * (how does C# NOT have a built in function for this?!)
	 * to be used as an ID for high score submissions.
	 */
	private long RandomLong() {
		int a = Random.Range (int.MinValue, int.MaxValue);
		int b = Random.Range (int.MinValue, int.MaxValue);
		return ((long) a) << 32 + b;
	}



	// Button press-methods

	public void OnClickRestart() {
		SceneManager.LoadScene ("Main_Game");
	}

	public void OnClickStartScreen() {
		SceneManager.LoadScene ("Start_Screen");
	}

	public void OnClickSendHighScore() {
		if (nameInput.activeSelf) {
			OnEnterName ();
		} else {
			nameInput.SetActive (true);
		}
	}

	/**
	 * Sends the input from the 'input-field' along with the current score
	 * to the firebase database. Also makes the send highscore button inactive
	 * and changes the text to "High score sent!"
	 */
	public void OnEnterName() {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ("https://jumperunitygame.firebaseio.com/");
		DatabaseReference highscoreRef = FirebaseDatabase.DefaultInstance.GetReference ("Highscores");
		highscoreRef.Child(RandomLong().ToString()).Child(nameInput.GetComponent<InputField>().text).SetValueAsync(score);

		nameInput.SetActive (false);
		sendHighscore.interactable = false;
		sendHighscore.GetComponentInChildren<Text>().text = "High score sent!";
	}

	// Button press-methods
}
