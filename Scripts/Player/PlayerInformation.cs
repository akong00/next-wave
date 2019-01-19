using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour {

	[HideInInspector]
	public bool won;

	//f/coin system for future?
	//f/public int coins;
	//f/public int coinsCollected;
	public int enemiesKilled;
	public int wavesStarted;

	void Awake()
	{
		won = true;
	}
}
