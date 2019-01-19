using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupMechanics : MonoBehaviour {

    public string powerup;

    public void SelfDestruct()
    {
        GetComponent<AudioSource>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1);
    }
}
