using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Rigidbody2D rigidBody;
	public float walkSpeed = 5f;

	void FixedUpdate() {
		rigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
	}
}
