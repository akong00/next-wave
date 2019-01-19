using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour {

    public static float powerupMultiplier;

    public float spawnCooldown = 15;
    public GameObject[] powerups;

    float timer;

    private void Update()
    {
        if (powerupMultiplier == 0 || PlayerHealth.gameEnd) return;

        timer += Time.deltaTime;
        if(timer >= spawnCooldown / powerupMultiplier)
        {
            timer = 0;
            SpawnPowerups();
        }
    }

    void SpawnPowerups()
    {
        GameObject powerup = Instantiate(powerups[Random.Range(0, powerups.Length)]);
        Destroy(powerup, spawnCooldown * Random.Range(0.7f, 1.0f));
    }
}
