﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeleeBehaviour : EnemyBehaviour {
	public bool meleeAttacking = false, usesProjectile = false;
	public float attackTime = 4.0f, throwingTime = 0.0f, sideSpeed = 0.5f, chargeSpeed = 4.0f;
	public GameObject player = null;
	public GameObject pow = null;

	protected bool _projectileFired = false;
	protected float _targetX = 0.0f, _startY = 4.0f;

	public List<AudioClip> sounds = new List<AudioClip> ();

	protected override void Awake()
	{
		base.Awake ();
		collider2D.enabled = false;
	}

	protected override void Initialize ()
	{
		collider2D.enabled = true;
		_startY = transform.position.y;
		anim = GetComponent<Animator> ();
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
		if (usesProjectile)
		{
			StartCoroutine (StartAttack (throwingTime));
		} else
		{
			StartCoroutine (StartAttack (attackTime));
		}

		if (audio)
		{
			audio.volume = Statics.soundVolume;
			
			/*audio.clip = sounds [2];
			audio.pitch = Random.Range (0.9f, 1.2f);
			audio.Play ();*/
		}
	}
	IEnumerator StartAttack(float t)
	{
		yield return new WaitForSeconds (t);
		if (usesProjectile && !_projectileFired)
		{
			GameObject tempo = (GameObject)Instantiate (projectilePrefab, transform.position, transform.rotation);
			levelManager.GetComponent<LevelBehaviour> ().AddToDeflectable (tempo);
			_projectileFired = true;
			StartCoroutine (StartAttack (attackTime));
		} else
		{
			meleeAttacking = true;
			anim.SetBool("Attack",true);
			if (sounds.Count > 0 && audio)
			{
				audio.clip = sounds [2];
				audio.pitch = Random.Range (0.9f, 1.2f);
				audio.Play ();
			}
		}
	}

	// Update is called once per frame
	protected override void Update () {
		base.Update ();
		if (!spawning && !isPaused)
		{
			if (usesProjectile && !_projectileFired)
			{
				float xDifference = _targetX - transform.position.x, yDifference = _startY - transform.position.y;
				while (Mathf.Abs(xDifference) < 0.1f)
				{
					_targetX = Random.Range (-2.7f, 2.7f);
					xDifference = _targetX - transform.position.x;
				}
				xDifference = Mathf.Clamp (xDifference, -sideSpeed, sideSpeed);
				yDifference = Mathf.Clamp (yDifference, -chargeSpeed, chargeSpeed);
				rigidbody2D.velocity = new Vector2 (xDifference, yDifference);
			} else if (!meleeAttacking)
			{
				float xDifference = player.transform.position.x - transform.position.x, yDifference = _startY - transform.position.y;
				xDifference = Mathf.Clamp (xDifference, -sideSpeed, sideSpeed);
				yDifference = Mathf.Clamp (yDifference, -chargeSpeed, chargeSpeed);
				rigidbody2D.velocity = new Vector2 (xDifference, yDifference);
				anim.SetBool("Attack",false);
			}
			else
			{
				rigidbody2D.velocity = new Vector2 (0.0f, -(chargeSpeed));
			}
		}
	}

	protected override void OnCollisionEnter2D (Collision2D collision)
	{
		base.OnCollisionEnter2D (collision);

		if (collision.gameObject.CompareTag ("Player"))
		{
			meleeAttacking = false;
			if (usesProjectile)
			{
				StartCoroutine (StartAttack (throwingTime));
			}
			else
			{
				StartCoroutine (StartAttack (attackTime));
			}
		}
	}

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		base.OnTriggerEnter2D (other);

		if (other.CompareTag ("Removal"))
		{
			DamageHealth ();
			Instantiate (pow, transform.position, transform.rotation);
			meleeAttacking = false;
			collider2D.enabled = false;
			StartCoroutine (EnableColliderAgain ());
			if (sounds.Count > 0 && audio)
			{
				audio.clip = sounds [1];
				audio.pitch = Random.Range (0.9f, 1.2f);
				audio.Play ();
			}
		}
	}
	IEnumerator EnableColliderAgain()
	{
		yield return new WaitForSeconds (1.0f);
		collider2D.enabled = true;
		StartCoroutine (StartAttack (attackTime));
	}
}
