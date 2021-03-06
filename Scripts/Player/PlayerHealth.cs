using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

	public static bool gameEnd;

	[HideInInspector]
	public int currentHealth;

	public Slider healthBar;
	public int startingHealth = 1000;

	Animator anim;
	AudioSource playerAudio;
	GunMechanics gunMechanics;

	void Awake()
	{
		gameEnd = false;
		anim = GetComponent<Animator>();
		playerAudio = GetComponent<AudioSource>();
		gunMechanics = transform.parent.transform.GetComponentInChildren<GunMechanics>();
		currentHealth = startingHealth;
	}

	void Update()
	{
		if(gameEnd)
		{
			StartCoroutine(EndGame());
		}
	}
	public void TakeDamage(int amount)
	{
		currentHealth -= amount;

		healthBar.value = currentHealth;
		playerAudio.Play();
		anim.Play("CastleDamageAnimation");

		if(currentHealth <= 0 && !gameEnd)
		{
			PlayerInformation playerInfo = transform.parent.GetComponent<PlayerInformation>();
			playerInfo.won = false;
			gameEnd = true;
		}
	}

	IEnumerator EndGame()
	{
		gunMechanics.DisableEffects();
		yield return new WaitForSeconds(gunMechanics.timeBetweenBullets);
		gunMechanics.enabled = false;
	}
}
