﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[System.Serializable]
//public class Sounds
//{
//	public AudioClip deathSound;
//	public AudioClip attackSound;
//	public AudioClip jokuSound;
//}

public class BigOssiBehaviour : EnemyBehaviour {

	public GameObject shieldBallPrefab;
	public float ballCount = 5f;
	public float shootingSpeed = 3f;
	public bool isSpawningBalls = true;
	
	protected BigOssiBallBehaviour shieldBall;
	protected List<GameObject> shieldBalls = new List<GameObject>();
	protected List<GameObject> ownProjectiles = new List<GameObject> ();
	protected float spawnInterval = 0f;
	protected float spawnTime = 0f;
	protected GameObject bigOssiReference;
	protected bool isOnLimitDistance = false;
	protected float _shootTimer = 0f;
	protected float _shakeTimer = 0f;
	protected bool isBossDying = false;
	protected float _dieTimer = 0f;
	protected Vector3 tempPosition;

	public ParticleSystem darkMatter;
	public GameObject deathExplosionPrefab;
	public GameObject explosionPrefab;
	//public Sounds soundsstruct;
	public List<AudioClip> sounds = new List<AudioClip> ();

	// Use this for initialization
	void Start ()
	{   //Get the animator
		anim = GetComponent<Animator> ();
		darkMatter = GetComponentInChildren<ParticleSystem> ();

		shieldBall = shieldBallPrefab.GetComponent<BigOssiBallBehaviour> ();
		//calculation for spawning for first time
		bossPhaseSetter ();
		//calculateCircumference ();

		if (audio)
		{
			audio.volume = Statics.soundVolume;
			
			audio.clip = sounds [2];
			audio.pitch = Random.Range (0.9f, 1.2f);
			audio.Play ();
		}
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		//Check, if boss is spawning shield. No shooting until they are ready
		if (isSpawningBalls == true)
		{
			SpawnBalls();
		}
		//Move left when touched the right corner
		else
		{
			ShootBalls();
		}

		if (isBossDying == true)
		{
			bossDeathAnim ();
		}
		else
		{
			bossMoving();
		}

		//spaceGtowin();


	}
	protected override void Flicker ()
	{
		_flickerTimer += Time.deltaTime;
		if (_flickerTimer % 0.4f < 0.2f)
		{
			GetComponent<SpriteRenderer>().color = new Color(1f,0.7f,0.7f);
		} else
		{
			GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f);
		}
		if (_flickerTimer > flickerTimerLimit)
		{
			_flickerActive = false;
			GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f);
			_flickerTimer = 0.0f;
		}
	}

	//Spawn shield balls in calculated time.
	public void SpawnBalls()
	{
		spawnTime += Time.deltaTime;
		//spawn ball when it is in desired distance away from previous ball
		if (spawnTime >= spawnInterval)
		{
			Spawn();
			spawnTime = 0f;
		}
		//check if there is desired amount of shieldballs in the field
		if (shieldBalls.Count >= ballCount)
			isSpawningBalls = false;
	}
	//Spawn shield balls to position related to boss function
	public void Spawn()
	{
		//spawn ball
		shieldBalls.Add((GameObject)Instantiate(shieldBallPrefab,
		                        new Vector3(gameObject.transform.position.x,
		                                    gameObject.transform.position.y + gameObject.renderer.bounds.size.y/2),
		                                    gameObject.transform.rotation));
		//set that ball to list
		shieldBalls [shieldBalls.Count - 1].GetComponent<BigOssiBallBehaviour> ().bigOssi = this;
		shieldBalls [shieldBalls.Count - 1].GetComponent<BigOssiBallBehaviour> ().SetListing(ListDeflectable, levelManager);

	}
	//Shoot projectile balls like every other ranged enemy.
	public void ShootBalls()
	{
		_shootTimer += Time.deltaTime;
		if (_shootTimer >= shootingSpeed)
		{
			for (int i = 0; i < ownProjectiles.Count; ++i)
			{
				if (!ownProjectiles [i])
				{
					ownProjectiles.RemoveAt (i);
					--i;
				}
			}
			if (ownProjectiles.Count < projectileLimit || projectileLimit == 0)
			{
				//set timer to 0
				_shootTimer = 0.0f;
				//Shoot projectile
				GameObject tempo = (GameObject)Instantiate (projectilePrefab, transform.position, transform.rotation);
				//Set velocity to it
				tempo.GetComponent<BallBehaviour> ().SetStartVelocity (new Vector2 (Random.Range (-0.2f, 0.2f), -0.4f));
				//animation Attack
				anim.SetTrigger ("BOAttack");

				if (sounds.Count > 0 && audio)
				{
					audio.clip = sounds [0];
					audio.pitch = Random.Range (0.9f, 1.2f);
					audio.Play ();
				}

				//list projectile ball
				ownProjectiles.Add (tempo);
				ListDeflectable (tempo);
			}
		}
	}
	//Boss took damage, do function with stuff in it
	protected override void DamageHealth ()
	{
		_shootTimer = 0.0f;

		if (!_flickerActive)
		{   //take damage to health
			--health;
			//start flickering -> invureable for a while
			_flickerActive = true; 
			//clear all shield balls
			DeleteAll();
			//animation Take Damage
			anim.SetTrigger("BODamage");
			bossPhaseSetter();

			if (sounds.Count > 0 && audio)
			{
				audio.clip = sounds [3];
				audio.pitch = Random.Range (0.9f, 1.2f);
				audio.Play ();
			}


			if (health <= 0)
			{
				isBossDying = true;
			}
			else
			{
				//Start spawning new balls
				isSpawningBalls = true;
			}
		}
	}
	//Destroy desired game object. (This time it is shield ball)
	public void Delete(GameObject me)
	{
		Destroy (me);
	}

	//Destroys and clears all balls from the field
	public void DeleteAll()
	{   //Destroy all shield balls from a list
		for(int i = 0; i < shieldBalls.Count; ++i)
		{
			if (shieldBalls[i])
			shieldBalls[i].GetComponent<BigOssiBallBehaviour>().DeleteObject();
		}
		//clear all shieldBalls from a list
		shieldBalls.Clear();
	}
	//Calculates circumference for spawning the balls correclty in circle
	protected void calculateCircumference()
	{		
		//Get speed from ball object
		var speed = shieldBall.speed;
		//Get radius from shield ball and add it to boss sprite width
		var radius = renderer.bounds.size.x/2 + shieldBall.spinningRadius;
		//Calculate circumference
		var circumference = 2 * radius * Mathf.PI;
		//Calculate time with cirumference and speed
		var ballTime = circumference / speed;
		//Calculate spawn interval
		spawnInterval = ballTime / ballCount;
		//Lastly divide spawn interval with 2 and you have right spawning rate for balls!
		spawnInterval /= 2;
	}

	protected void bossPhaseSetter()
	{
		switch (health)
		{
		case 5:
			shieldBall.speed = 1.5f;
			ballCount = 5;
			calculateCircumference ();
			darkMatter.emissionRate = 3f;
			break;
		case 4:
			shieldBall.speed = 1.8f;
			ballCount = 6;
			calculateCircumference ();
			darkMatter.emissionRate = 5f;
			break;
		case 3:
			shieldBall.speed = 2f;
			ballCount = 7;
			calculateCircumference ();
			darkMatter.emissionRate = 7f;
			break;
		case 2:
			shieldBall.speed = 2.2f;
			ballCount = 8;
			calculateCircumference ();
			darkMatter.emissionRate = 9f;
			break;
		case 1:
			shieldBall.speed = 2.5f;
			ballCount = 10;
			calculateCircumference ();
			darkMatter.emissionRate = 11f;
			break;
		default:
			tempPosition = gameObject.transform.position;
			break;
		}
	}
	
	protected void bossMoving()
	{	
		if (isOnLimitDistance == true)
		{
			gameObject.transform.Translate (new Vector3 (-Time.deltaTime, 0, 0));
			if (gameObject.transform.position.x <= -2.5f)
			{
				isOnLimitDistance = false;
				anim.SetBool ("BOMovingRight", true);
				anim.SetBool ("BOMovingLeft", false);
			}
		} else
		{
			gameObject.transform.Translate (new Vector3 (Time.deltaTime, 0, 0));
			if (gameObject.transform.position.x >= 2.5f)
			{
				isOnLimitDistance = true;
				anim.SetBool ("BOMovingLeft", true);
				anim.SetBool ("BOMovingRight", false);
			}
		}
	}

	protected void bossDeathAnim()
	{
		anim.SetTrigger("BODamage");
		_dieTimer += Time.deltaTime;
		_shakeTimer += Time.deltaTime;
		var randX = Random.Range(-0.1f,0.1f);
		var randY = Random.Range(-0.1f,0.1f);

		gameObject.transform.position = new Vector3(tempPosition.x + randX , tempPosition.y + randY);
		
		darkMatter.emissionRate = 11f + _dieTimer*20f;
		darkMatter.startSize = 1f + _dieTimer;

		if (_shakeTimer >= 0.15f)
		{
			var explosion = Instantiate(explosionPrefab,
			                            new Vector2(transform.position.x + Random.Range (-1, 1), transform.position.y + Random.Range (-1, 1)),
			                            transform.rotation);
			_shakeTimer = 0;
		}

		if (_dieTimer >= 3f)
		{
			Instantiate(deathExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
			Destroy (gameObject);
		}

	}

//	protected void spaceGtowin()
//	{
//		if (Input.GetKey (KeyCode.G))
//		{
//			DamageHealth();
//		}
//	}

	//Test ienumerator
	IEnumerator joku()
	{
		yield return new WaitForSeconds (4.0f);
	}
}
