using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public int maxHealth = 100;
    private int health;

    private void Awake()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (health <= 0) {
            die();
        }
    }

    private void die() {
        gameObject.SetActive(false);
        FindObjectOfType<StatsController>().PlayerKillsEnemy();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            health -= collision.gameObject.GetComponent<ProjectileController>().damage;
        }
    }
}
