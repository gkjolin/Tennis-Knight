﻿using UnityEngine;
using System.Collections;

public class RangedBehaviour : EnemyBehaviour {
	public float shootTimerLimit = 1.0f;
	public bool usesSinModifier = false;

	private float _shootTimer = 0.0f, _levelStartTime = 0.0f, _sinModifier = 1.0f;
	private bool _sinDirection = true;

	// Use this for initialization
	void Start () {
		_levelStartTime = Time.time;
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (usesSinModifier)
		{
			if (_sinDirection)
			{
				_sinModifier -= Time.deltaTime;
				if (_sinModifier < 0.0f)
				{
					_sinModifier = 0.0f;
					_sinDirection = false;
				}
			} else
			{
				_sinModifier += Time.deltaTime;
				if (_sinModifier > 1.0f)
				{
					_sinModifier = 1.0f;
					_sinDirection = true;
				}
			}
		}

		transform.position = new Vector3(Mathf.Sin(Time.time - _levelStartTime) * 2.5f * _sinModifier, transform.position.y);
		
		_shootTimer += Time.deltaTime;
		if (_shootTimer > shootTimerLimit)
		{
			_shootTimer = 0.0f;
			GameObject tempo = (GameObject)Instantiate (projectilePrefab, transform.position, transform.rotation);
			tempo.GetComponent<BallBehaviour>().SetStartVelocity(new Vector2(Random.Range(-0.2f, 0.2f), -0.4f));
			ListDeflectable(tempo);
		}
	}
}