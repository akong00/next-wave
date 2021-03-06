using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	public PlayerHealth playerHealth1;
	public PlayerHealth playerHealth2;
	public GameObject enemy;
	public float wavesPerIncrement = 1;
	public int firstAppearanceWave = 1;
	public int firstAppearanceAmount = 1;
	public float staggerTime = 0.2f;

	float xSpawnRange;
	float ySpawnRange;

	void Awake()
	{
		xSpawnRange = (4.865f - enemy.GetComponent<SpriteRenderer>().bounds.size.x / 2) / 2;//4.865 is width for 18:37 aspect ratio(slimmest)
		ySpawnRange = (GetComponent<SpriteRenderer>().bounds.size.y - enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2) /2;
	}

	void SpawnEnemies()
	{
		StartCoroutine(StaggerSpawn());
	}

	IEnumerator StaggerSpawn()
	{
		if(EnemyManager.wave < firstAppearanceWave) yield break;
		//single player mode
		if(PlayerPrefs.GetInt("mode", 1) == 1)
		{
			for(float i = firstAppearanceAmount + EnemyManager.wave - firstAppearanceWave; i > 0; i-= wavesPerIncrement)
			{
				//spawn for player 1
				Vector3 randSpawnpoint = new Vector3(
					Random.Range(-xSpawnRange, xSpawnRange) + transform.position.x,
					Random.Range(-ySpawnRange, ySpawnRange) + transform.position.y,
					0);
				Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);
				Spawn(randSpawnpoint, spawnRotation);

				EnemyManager.enemiesLeft += 1;

				yield return new WaitForSeconds(staggerTime);
			}
		}
		//two player mode
		else if(PlayerPrefs.GetInt("mode", 1) == 2)
		{
			for(float i = firstAppearanceAmount + EnemyManager.wave - firstAppearanceWave; i > 0; i-= wavesPerIncrement)
			{
				//spawn for player 1
				Vector3 randSpawnpoint = new Vector3(
					Random.Range(-xSpawnRange, xSpawnRange) + transform.position.x,
					Random.Range(-ySpawnRange, -0.0001f) + transform.position.y,
					0);
				Quaternion spawnRotation = Quaternion.Euler(0, 0, 0);
				Spawn(randSpawnpoint, spawnRotation);

				//spawn for player 2
				randSpawnpoint = new Vector3(
					Random.Range(-xSpawnRange, xSpawnRange) + transform.position.x,
					-randSpawnpoint.y,
					0);
				spawnRotation = Quaternion.Euler(0, 0, 180);
				Spawn(randSpawnpoint, spawnRotation);

				EnemyManager.enemiesLeft += 2;

				yield return new WaitForSeconds(staggerTime);
			}
		}
	}

	void Spawn(Vector3 spawnpoint, Quaternion spawnRotation)
	{
		Instantiate(enemy, spawnpoint, spawnRotation);
	}
}
