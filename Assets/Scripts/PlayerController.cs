using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Rigidbody2D rigidBody;
	public float walkSpeed = 5f;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() 
    {
		rigidBody.velocity = new Vector2(Input.GetAxis("Horizontal") * walkSpeed, Input.GetAxis("Vertical") * walkSpeed);
	}
}
