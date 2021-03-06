using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    //f/ implement coinValue in the future?
    public int startingHealth;
    public GameObject bloodSquirt;
    public GameObject smallBloodStain;
    public GameObject largeBloodStain;

    ///[HideInInspector]
    public int currentHealth;

    Animator anim;
    AudioSource hitAudio;
    SpriteRenderer spriteRenderer;
    bool isDead;
    bool isFading;
    float fadeTime = 15f;
    float alpha = 1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        hitAudio = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = startingHealth;
    }

    void Update()
    {
        if (isFading)
        {
            alpha -= Time.deltaTime / fadeTime;
            spriteRenderer.color = new Color(1, 1, 1, alpha);
        }
    }

    public void TakeDamage(int amount, Vector2 hitPoint, GameObject player)
    {
        if (isDead) return;

        currentHealth -= amount;

        hitAudio.Play();

        //small blood stain effect
        Vector3 tempVector = Random.rotation.eulerAngles;
        Quaternion tempRotation = Quaternion.Euler(0, 0, tempVector.z);
        GameObject bloodClone = Instantiate(smallBloodStain, hitPoint, tempRotation);
        //prevents lag
        Destroy(bloodClone, 60);

        //blood squirt effect
        bloodClone = Instantiate(bloodSquirt, hitPoint, tempRotation);
        Destroy(bloodClone, 0.5f);
        if (currentHealth <= 0)
        {
            Death(player);
        }
    }

    void Death(GameObject player)
    {
        isDead = true;
        EnemyManager.enemiesLeft -= 1;

        //add kill count to person who killed enemy
        PlayerInformation playerInfo = player.GetComponent<PlayerInformation>();
        //f/coins in future?
        //f/playerInfo.coins += coinValue;
        playerInfo.enemiesKilled += 1;

        //large blood stain effect
        Vector3 tempVector = Random.rotation.eulerAngles;
        Quaternion tempRotation = Quaternion.Euler(0, 0, tempVector.z);
        GameObject bloodClone = Instantiate(largeBloodStain, transform.position, tempRotation);
        //prevents lag
        Destroy(bloodClone, 60);

        //random rotation on corpse
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-180f, 180f));
        anim.SetBool("Dead", true);
        isFading = true;
        Destroy(gameObject, fadeTime + 1);
    }
}