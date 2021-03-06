using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour {

	public GameObject player2;
	public GameObject player2UI;
	public GameObject resumeButton2;
	public GameObject enemySpawner;
	public GameObject waveText1;

	void Awake()
	{
		Time.timeScale = 1;
		if(PlayerPrefs.GetInt("mode", 1) == 1)
		{
			SetupSinglePlayer();
		}
	}

	void SetupSinglePlayer()
	{
		Destroy(player2);
		Destroy(player2UI);
		Destroy(resumeButton2);
		waveText1.transform.localPosition = Vector3.zero;
		enemySpawner.transform.position = new Vector3(0, 4, 0);
	}
}
