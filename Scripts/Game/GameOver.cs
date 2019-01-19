using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour {

	public Text winText1;
	public Text winText2;
	public Text statsText1;
	public Text statsText2;
	public GameObject pauseMask;

	PlayerInformation playerInfo1;
	PlayerInformation playerInfo2;
	float letterPause = 0.05f;
	bool endingGame;

	void Awake()
	{
		playerInfo1 = GameObject.Find("Player1").GetComponent<PlayerInformation>();
		if(PlayerPrefs.GetInt("mode", 1) == 2)
		{
			playerInfo2 = GameObject.Find("Player2").GetComponent<PlayerInformation>();
		}
	}

	void Update()
	{
		if(PlayerHealth.gameEnd && !endingGame) EndGame();
	}

	void EndGame()
	{
		endingGame = true;
		pauseMask.SetActive(true);
		string text1;
		string stat1;

		winText1.color = Color.yellow;
		text1 = "Wave " + EnemyManager.wave.ToString();
		stat1 = "Enemies Killed: " + playerInfo1.enemiesKilled.ToString() + "\n" +
			"Waves Started: " + playerInfo1.wavesStarted.ToString();

		//2 player mode
		if(PlayerPrefs.GetInt("mode", 1) == 2)
		{
			string text2;
			string stat2;

			if(playerInfo1.won)
			{
				text1 = "Victory";
				winText1.color = Color.yellow;
				text2 = "Defeat";
				winText2.color = Color.red;
			}
			else
			{
				text1 = "Defeat";
				winText1.color = Color.red;
				text2 = "Victory";
				winText2.color = Color.yellow;
			}
			stat2 = "Enemies Killed: " + playerInfo2.enemiesKilled.ToString() + "\n" +
				"Waves Started: " + playerInfo2.wavesStarted.ToString();
			StartCoroutine(DisplayText(winText2, text2, statsText2, stat2));
		}

		StartCoroutine(DisplayText(winText1, text1, statsText1, stat1));
	}

	IEnumerator DisplayText(Text winText, string text, Text statsText, string stat)
	{
		winText.enabled = statsText.enabled = true;
		winText.text = "";
		statsText.text = "";
		winText.GetComponent<AudioSource>().Play();
		foreach(var letter in text)
		{
			winText.text += letter;
			yield return new WaitForSeconds(letterPause);
		}
		winText.GetComponent<AudioSource>().Stop();

		yield return new WaitForSeconds(0.5f);

		winText.GetComponent<AudioSource>().Play();
		foreach(var letter in stat)
		{
			statsText.text += letter;
			yield return new WaitForSeconds(letterPause);
		}
		winText.GetComponent<AudioSource>().Stop();
	}
}
