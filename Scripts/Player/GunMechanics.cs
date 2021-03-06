using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMechanics : MonoBehaviour {

	public static float sensitivity = 3f;
    
	public int bulletDamage = 10;
	public float timeBetweenBullets = 0.12f;
	public float bloom = 0.15f;
    public float powerupTime = 15;
	public GameObject hitDirt;

	AudioSource fireAudio;
	GameObject flash;
	GameObject crosshair;
    SpriteRenderer gunBase;
    RectTransform barrel;
    string bulletType;
    float explosionRadius = 0.75f;
    float effectsDisplayTime = 0.4f;//percentage of timeBetweenBullets
	float timer;
	float touchRangey;
	float shootRangex;
	float shootRangey;
	float minShootRangey;
	bool alreadyShooting;//prevents two fingers on one side shooting
	bool hitEnemy;
    bool explosivex2;//if 2 explosive powerups at the same time
    Color playerColor;//for increase damage powerup

	void Awake()
	{
		fireAudio = GetComponent<AudioSource>();
		flash = transform.Find("GunBarrel").gameObject.transform.Find("Flash").gameObject;
		crosshair = transform.Find("Crosshair").gameObject;
        gunBase = transform.Find("GunBase").gameObject.transform.GetComponent<SpriteRenderer>();
		barrel = transform.Find("GunBarrel").gameObject.GetComponent<RectTransform>();
        playerColor = gunBase.color;
		touchRangey = 5f;
		shootRangex = transform.parent.transform.Find("Castle").GetComponent<SpriteRenderer>().bounds.size.x / 2;
		shootRangey = 9.9f;
		minShootRangey = transform.parent.transform.Find("Castle").GetComponent<SpriteRenderer>().bounds.size.y;
	}

	void Update()
	{
		timer += Time.deltaTime;

		if(Input.touchCount > 0)
		{
			alreadyShooting = false;

			foreach(Touch touch in Input.touches)
			{
				if(Mathf.Abs(Camera.main.ScreenToWorldPoint(touch.position).y - transform.position.y) < touchRangey &&
					touch.phase != TouchPhase.Ended &&
					touch.phase != TouchPhase.Canceled &&
					!alreadyShooting)
				{
					alreadyShooting = true;
					Shoot(touch);
				}
			}
		}
	}

	void Shoot(Touch touch)
    {
        //always update crosshair and barrel, later see if it's time to shoot

		//convert screen to world point
		Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

		//rotating barrel(x and y flipped and negative x so angle is correct)
		float angle = Mathf.Atan2(-(touchPos.x - transform.position.x), (touchPos.y - transform.position.y)) * Mathf.Rad2Deg;
		Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
		barrel.rotation = targetRotation;

		//find crosshair position relative to gun and bloom
		Vector2 targetPosition;
		Vector2 hitPosition = Random.insideUnitCircle * bloom;
		if(transform.parent.name == "Player1")
		{
			//crosshair
			targetPosition = new Vector2((touchPos.x - transform.position.x) * sensitivity, (touchPos.y - transform.position.y) * sensitivity);
			//limit to within play area
			targetPosition = RestrictTargetPosition(targetPosition);
			crosshair.transform.localPosition = targetPosition;

			//bloom
			hitPosition = new Vector2(hitPosition.x + targetPosition.x + transform.position.x, hitPosition.y + targetPosition.y + transform.position.y);
		}
		else
		{
			//crosshair
			targetPosition = new Vector2(-(touchPos.x - transform.position.x) * sensitivity, -(touchPos.y - transform.position.y) * sensitivity);
			//limit to within play area
			targetPosition = RestrictTargetPosition(targetPosition);
			crosshair.transform.localPosition = targetPosition;

			//bloom
			hitPosition = new Vector2(hitPosition.x - targetPosition.x + transform.position.x, hitPosition.y - targetPosition.y + transform.position.y);
		}

        //actual shooting
        if (timer >= timeBetweenBullets)
        {
            timer = 0;

            //gun flash
            flash.SetActive(true);
            StartCoroutine(DisableEffects());

            //gun sound
            fireAudio.Play();

            //determine type of bullet
            if (bulletType == "explosive")
            {
                Collider2D[] explosionHits = Physics2D.OverlapCircleAll(hitPosition, explosionRadius);

                //take health from enemy or get powerup
                foreach (Collider2D explosionHit in explosionHits)
                {
                    try
                    {
                        EnemyHealth enemyHealth = explosionHit.transform.parent.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(bulletDamage, hitPosition, transform.parent.gameObject);
                        }
                    }
                    catch(System.NullReferenceException e)
                    {
                    }

                    PowerupMechanics powerupMechanics = explosionHit.transform.GetComponent<PowerupMechanics>();
                    if (powerupMechanics != null)
                    {
                        StartCoroutine(GivePowerup(powerupMechanics.powerup));
                        powerupMechanics.SelfDestruct();
                    }
                }

                //hit dirt effect
                if (transform.parent.name == "Player1")
                {
                    GameObject hitDirtClone = Instantiate(hitDirt, hitPosition, Quaternion.identity);
                    hitDirtClone.GetComponent<Animator>().SetBool("isExplosion", true);
                    Destroy(hitDirtClone, 1);
                }
                //player 2
                else
                {
                    GameObject hitDirtClone = Instantiate(hitDirt, hitPosition, Quaternion.Euler(0, 0, 180));
                    hitDirtClone.GetComponent<Animator>().SetBool("isExplosion", true);
                    Destroy(hitDirtClone, 1);
                }
            }
            //normal bullet
            else
            {
                RaycastHit2D shootHit = Physics2D.Raycast(hitPosition, Vector2.zero);

                //take health from enemy or get powerup
                if (shootHit && shootHit.collider != null)
                {
                    try
                    {
                        EnemyHealth enemyHealth = shootHit.collider.transform.parent.GetComponent<EnemyHealth>();
                        if (enemyHealth != null)
                        {
                            enemyHealth.TakeDamage(bulletDamage, hitPosition, transform.parent.gameObject);
                            //prevent dirt effect because enemy has blood effect
                            hitEnemy = true;
                        }
                    }
                    catch(System.NullReferenceException e)
                    {
                    }

                    PowerupMechanics powerupMechanics = shootHit.collider.transform.GetComponent<PowerupMechanics>();
                    if (powerupMechanics != null)
                    {
                        StartCoroutine(GivePowerup(powerupMechanics.powerup));
                        powerupMechanics.SelfDestruct();
                    }
                }

                //hit dirt effect
                if (transform.parent.name == "Player1" && !hitEnemy)//enemy will produce blood
                {
                    GameObject hitDirtClone = Instantiate(hitDirt, hitPosition, Quaternion.identity);
                    Destroy(hitDirtClone, 1);
                }
                else if (!hitEnemy)//player 2
                {
                    GameObject hitDirtClone = Instantiate(hitDirt, hitPosition, Quaternion.Euler(0, 0, 180));
                    Destroy(hitDirtClone, 1);
                }
                hitEnemy = false;
            }
        }
	}

    public IEnumerator GivePowerup(string powerup)
    {
        switch (powerup)
        {
            case "larger crosshair":
                if(crosshair.transform.localScale.x >= 2) break;

                //give buffs
                crosshair.transform.localScale *= 1.5f;
                bloom *= 1.5f;
                timeBetweenBullets /= 3;

                yield return new WaitForSeconds(powerupTime);

                //reset buffs
                crosshair.transform.localScale /= 1.5f;
                bloom /= 1.5f;
                timeBetweenBullets *= 3;
                break;
            case "increase damage":
                if(gunBase.color == Color.magenta) break;

                //give buffs
                if (gunBase.color == Color.yellow)
                {
                    //second buff
                    gunBase.color = Color.magenta;
                    bulletDamage += 10;
                }
                else
                {
                    //first buff
                    gunBase.color = Color.yellow;
                    bulletDamage += 10;
                }

                yield return new WaitForSeconds(powerupTime);

                //reset buffs
                if (gunBase.color == Color.magenta)
                {
                    //reset second buff
                    gunBase.color = Color.yellow;
                    bulletDamage -= 10;
                }
                else
                {
                    //reset first buff
                    bulletDamage -= 10;
                    gunBase.color = playerColor;
                }
                break;
            case "explosive bullets":
                if(explosivex2) break;

                //give buffs
                if(bulletType == "explosive")
                {
                    //second buff
                    explosivex2 = true;
                    timeBetweenBullets /= 2;
                }
                else
                {
                    //first buff
                    bulletType = "explosive";
                    bulletDamage += 50;
                    timeBetweenBullets *= 10;
                }

                yield return new WaitForSeconds(powerupTime);

                //reset buffs
                if (explosivex2)
                {
                    //reset second buff
                    explosivex2 = false;
                    timeBetweenBullets *= 2;
                }
                else
                {
                    //reset first buff
                    bulletDamage -= 50;
                    timeBetweenBullets /= 10;
                    bulletType = "normal";
                }
                break;
        }
        yield break;
    }

	Vector2 RestrictTargetPosition(Vector2 targetPosition)
	{
		//target position is relative already
		if(Mathf.Abs(targetPosition.x) > shootRangex)
		{
			float a = targetPosition.x - Mathf.Sign(targetPosition.x) * shootRangex;
			float b = targetPosition.x;
			float c = targetPosition.y;
			targetPosition = new Vector2(Mathf.Sign(targetPosition.x) * shootRangex, c - a * c / b);
		}
		if(Mathf.Abs(targetPosition.y) > shootRangey)
		{
			float a = targetPosition.y - Mathf.Sign(targetPosition.y) * shootRangey;
			float b = targetPosition.y;
			float c = targetPosition.x;
			targetPosition = new Vector2(c - a * c / b, Mathf.Sign(targetPosition.y) * shootRangey);
		}
		else if(Mathf.Abs(targetPosition.y) < minShootRangey)
		{
			targetPosition = new Vector2(targetPosition.x, Mathf.Sign(targetPosition.y) * minShootRangey);
		}
		return targetPosition;
	}

	public IEnumerator DisableEffects()
	{
		yield return new WaitForSeconds(timeBetweenBullets * effectsDisplayTime);
		flash.SetActive(false);
	}
}
