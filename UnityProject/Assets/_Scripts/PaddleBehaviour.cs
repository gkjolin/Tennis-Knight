﻿using UnityEngine;
using System.Collections;

public class PaddleBehaviour : MonoBehaviour {
	public GameObject player;
	public bool isLeft;

	private Vector3 _startPosition;
	private Quaternion _startRotation;
	private float _hitTime = 0.0f;
	private int _deflectedLayer;
	private bool _hitting = false;

	// Use this for initialization
	void Start () {
		_startPosition = transform.localPosition;
		_startRotation = transform.localRotation;
		_deflectedLayer = LayerMask.NameToLayer ("OwnProjectiles");
		Hitting (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (_hitting)
		{
			_hitTime += Time.deltaTime * 5;
			if (isLeft)
			{
				transform.localPosition = Vector3.Lerp (_startPosition, _startPosition + new Vector3 (0.6f, 0.8f), _hitTime);
				transform.localRotation = Quaternion.Lerp (_startRotation, Quaternion.Euler (0.0f, 0.0f, -35.0f), _hitTime);
			} else
			{
				transform.localPosition = Vector3.Lerp (_startPosition, _startPosition + new Vector3 (-0.6f, 0.8f), _hitTime);
				transform.localRotation = Quaternion.Lerp (_startRotation, Quaternion.Euler (0.0f, 0.0f, 35.0f), _hitTime);
			}
		}
	}

	public void PaddleHit()
	{
		Hitting (true);
		StartCoroutine (PaddleDisable ());
	}

	IEnumerator PaddleDisable ()
	{
		yield return new WaitForSeconds (0.2f);
		transform.localPosition = _startPosition;
		transform.localRotation = _startRotation;
		_hitTime = 0.0f;
		player.GetComponent<PlayerBehaviour>().paddleActive = false;
		Hitting(false);
	}

	void Hitting(bool hit)
	{
		_hitting = hit;
		renderer.enabled = hit;
		collider2D.enabled = hit;
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag ("Deflectable"))
		{
			collision.gameObject.tag = "Deflected";
			collision.gameObject.layer = (LayerMask)_deflectedLayer;
		}
	}
}
