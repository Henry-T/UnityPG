using UnityEngine;
using System.Collections;

public class SnakeGame : MonoBehaviour {

	private static SnakeGame instance = null;
	private GUIText displayLives;
	private GUIText displayScore;

	public int gameScore = 0;
	public int gameLives = 3;
	public int scoreMultiplier = 100;

	public static SnakeGame Instance{
		get{
			if(instance == null)
				instance = new GameObject("SnakeGame").AddComponent<SnakeGame>();
			return instance;
		}
	}

	public void OnApplicationQuit(){
		DestroyInstance ();
	}

	public void DestroyInstance(){
		print ("Snake Game Instance Destroyed");
		instance = null;
	}

	public void UpdateScore(int additive){
		gameScore += additive * scoreMultiplier;
		displayScore.text = "Score: " + gameScore.ToString();
	}

	public void UpdateLives(int additive){
		gameLives += additive;
		gameLives = Mathf.Clamp (gameLives, 0, 3);
		displayLives.text = "Lives: " + gameLives.ToString ();
	}

	public void Initialize(){
		print ("Snake Game Initialized");
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;

		gameScore = 0;
		gameLives = 3;
		scoreMultiplier = 100;

		UpdateScore (0);
		UpdateLives (0);

	}
}
