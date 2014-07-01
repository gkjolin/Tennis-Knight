﻿using UnityEngine;
using System.Collections;

public class BombBehaviour : BallBehaviour {
	public float fuseLength = 6.0f, explosiveForce = 1.0f;
	public AudioClip explosion;

	private bool _exploding = false;

	private Animator anim;

	void Awake()
	{
		StartCoroutine (LitFuse());
		anim = GetComponent<Animator> ();
	}

	IEnumerator LitFuse()
	{
		yield return new WaitForSeconds (fuseLength);
		Explode ();
	}
	IEnumerator StopExploding()
	{
		yield return new WaitForSeconds (0.3f);
		collider2D.enabled = false;
		gameObject.transform.localScale = new Vector3(4f,4f,0);
		anim.SetTrigger("explode");
	}

	void Explode()
	{
		if (!_exploding)
		{
			GetComponent<CircleCollider2D> ().isTrigger = true;
			GetComponent<CircleCollider2D> ().radius = explosiveForce;
			rigidbody2D.velocity = Vector2.zero;
			if (!audio.isPlaying)
			{
				audio.clip = explosion;
				audio.volume = 0.6f;
				audio.Play ();

			}
			StartCoroutine (StopExploding ());

			Destroy (gameObject, 1.2f);

			//anim.GetComponent<Renderer>().transform

			//GetComponent<CircleCollider2D>().transform.localScale=new Vector3(0.25f,0.25f,0);
			//animation.Play();
		}

	}

	protected override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.CompareTag ("Removal"))
		{
			Explode ();

		}
	}

	public override void BallDestroy ()
	{
		Explode ();
		//anim.SetTrigger("explode");
	}
}
