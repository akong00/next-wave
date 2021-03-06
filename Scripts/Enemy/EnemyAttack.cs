using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

	public float attackTime = 1f;
	public int attackDamage = 10;

	Animator anim;
	GameObject playerCastle;
	PlayerHealth playerHealth;
	EnemyHealth enemyHealth;
	EnemyMovement enemyMovement;
	bool playerInRange;
	float timer;

	void Awake()
	{
		enemyHealth = GetComponent<EnemyHealth>();
		enemyMovement = GetComponent<EnemyMovement>();
		anim = GetComponent<Animator>();
	}

	void Start()
	{
		playerCastle = enemyMovement.playerCastle;
		playerHealth = playerCastle.gameObject.GetComponent<PlayerHealth>();
	}

	void Update()
	{
		timer += Time.deltaTime;

		if(timer >= attackTime && enemyMovement.reachedCastle && enemyHealth.currentHealth > 0 && !PlayerHealth.gameEnd)
		{
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{
		timer = 0;

		anim.Play("Attack");
		yield return new WaitForSeconds(0.5f);
		if(playerHealth.currentHealth > 0)
		{
			playerHealth.TakeDamage(attackDamage);
		}
	}
}
