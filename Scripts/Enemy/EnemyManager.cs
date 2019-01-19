using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour {

	public static int enemiesLeft;
	public static int wave;

	public Text waveText1;
	public Text waveText2;

	AudioSource[] zombieAudio;
	float letterPause = 0.05f;
	float displayTime = 1f;
	float sfxtimer;
	bool isFirstWave = true;//queue pregame visuals
	bool processingWave;

	void Awake()
	{
		zombieAudio = gameObject.GetComponents<AudioSource>();
		enemiesLeft = 0;
		wave = 0;
	}

	void Update()
	{
		//auto start wave
		if(enemiesLeft <= 0)
		{
			StartWave();
		}

		//zombie sound effects
		sfxtimer += Time.deltaTime;
		if(sfxtimer > 1 && wave > 0)
		{
			sfxtimer = 0;
			if(Random.Range(0f, 1000 / wave) < 20)
			{
				zombieAudio[Random.Range(0, zombieAudio.Length - 1)].Play();
			}
		}
	}

	public void StartWave(GameObject player = null)
	{
		if(processingWave) return;
		StartCoroutine(StartWaveCoroutine(player));
	}

	IEnumerator StartWaveCoroutine(GameObject player = null)
	{
		processingWave = true;

		if(isFirstWave) 
		{
			yield return new WaitForSeconds(0.5f);
			yield return DisplayText("Ready...");
			yield return DisplayText("Set...");
			yield return DisplayText("Go!");
			isFirstWave = false;
		}

		wave += 1;

		//begin wave with audio
		zombieAudio[Random.Range(0, zombieAudio.Length - 1)].Play();

		if(player != null)
		{
			PlayerInformation playerInfo = player.GetComponent<PlayerInformation>();
			playerInfo.wavesStarted += 1;
			string tempString = "NEXT WAVE!";
			waveText1.color = waveText2.color = Color.red;
			yield return DisplayText(tempString);
			waveText1.color = waveText2.color = Color.black;
		}
		gameObject.SendMessage("SpawnEnemies", SendMessageOptions.DontRequireReceiver);

		string tempString1 = "WAVE " + wave.ToString();
		yield return DisplayText(tempString1);

		processingWave = false;
	}

	IEnumerator DisplayText(string text)
	{
		waveText1.enabled = waveText2.enabled = true;
		waveText1.GetComponent<AudioSource>().Play();
		foreach(var letter in text)
		{
			waveText1.text += letter;
			if(PlayerPrefs.GetInt("mode", 2) == 2)
				waveText2.text += letter;
			yield return new WaitForSeconds(letterPause);
		}
		waveText1.GetComponent<AudioSource>().Stop();

		yield return new WaitForSeconds(displayTime);

		waveText1.GetComponent<AudioSource>().Play();
		foreach(var letter in waveText1.text)
		{
			waveText1.text = waveText1.text.Substring(1, waveText1.text.Length - 1);
			if(PlayerPrefs.GetInt("mode", 2) == 2)
				waveText2.text = waveText1.text;
			yield return new WaitForSeconds(letterPause);
		}
		waveText1.GetComponent<AudioSource>().Stop();
	}

}
