using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : MonoBehaviour {

	Rigidbody2D rb;
	float dirX;
	float moveSpeed = 20f;
	CharacterController characterController;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		dirX = Input.acceleration.x * moveSpeed;
		transform.position = new Vector2 (Mathf.Clamp (transform.position.x, -7.5f, 7.0), transform.position.y);
	}

	void FixedUpdate()
	{
		rb.velocity = new Vector2 (dirX, 0f);
	}

}
