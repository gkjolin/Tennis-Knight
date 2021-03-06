﻿using UnityEngine;
using System.Collections;

public class FlyingMoneyBehaviour : MonoBehaviour {
	public float lifetime = 2.0f, rotateSpeed = 5.0f, randomRange = 1.0f, flyingSpeed = 5.0f;

	void Start ()
	{
		audio.volume = Statics.soundVolume;
		Destroy (gameObject, lifetime);
		rigidbody2D.velocity = new Vector2 (Random.Range (-randomRange, randomRange), Random.Range (-randomRange, randomRange) + flyingSpeed);
	}
	
	void Update ()
	{
		transform.Rotate(new Vector3(0.0f, 0.0f, rotateSpeed));
	}

	public void PlaySound()
	{
		audio.Play ();
	}
}
