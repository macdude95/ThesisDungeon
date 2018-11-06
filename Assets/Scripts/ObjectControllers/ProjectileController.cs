using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    private Rigidbody2D rb;
    public int damage = 30;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void fireWithVelocity(Vector2 fireVelocity, Vector2 startingPosition)
    {
        gameObject.transform.position = startingPosition;
        gameObject.SetActive(true);
        rb.velocity = fireVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gameObject.SetActive(false);
    }

}
