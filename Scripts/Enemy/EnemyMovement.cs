using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	[HideInInspector]
	public bool reachedCastle;
	[HideInInspector]
	public GameObject playerCastle;

	public float speed = 5;

	Animator anim;
	EnemyHealth enemyHealth;
	Vector3 targetDirection;


	void Awake()
	{
		anim = GetComponent<Animator>();
		enemyHealth = GetComponent<EnemyHealth>();
		//find shortest distance to one of the targets behind player base
		GameObject[] enemyTargets = GameObject.FindGameObjectsWithTag("EnemyTarget");
		float shortest = Mathf.Infinity;
		GameObject closestEnemyTarget = null;
		foreach(GameObject enemyTarget in enemyTargets)
		{
			Vector3 offset = transform.position - enemyTarget.transform.position;
			if (offset.sqrMagnitude < shortest)
			{
				shortest = offset.sqrMagnitude;
				closestEnemyTarget = enemyTarget;
				playerCastle = enemyTarget.transform.parent.gameObject;
			}
		}
		//find normalized vector to target
		targetDirection = closestEnemyTarget.transform.position - transform.position;
		Vector3.Normalize(targetDirection);
	}

	void Start()
	{
		StartCoroutine(OnSpawn());
	}

	void Update()
	{
		if(enemyHealth.currentHealth > 0 && !reachedCastle && ButtonManager.gameActive && !PlayerHealth.gameEnd)
		{
			transform.position += targetDirection / 300000 * speed;//300000 bc it works well
		}

		if(PlayerHealth.gameEnd)
		{
			anim.SetTrigger("GameEnd");
		}
	}

	IEnumerator OnSpawn()
	{
		anim.Play("Spawn");
		yield return new WaitForSeconds(0.333f);
		anim.SetFloat("CycleOffset", Random.Range(0f, 1f));
		anim.SetFloat("WalkSpeed", speed / 5);//5 was used as reference for the standard animation 
		anim.Play("Walk");
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject == playerCastle)
		{
			reachedCastle = true;
		}
	}
}
