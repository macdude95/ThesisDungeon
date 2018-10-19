using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 5f;
    public float projectileSpeed = 7f;
    public float fireRate = 4f;
    public GameObject ProjectilePrefab;
    private Rigidbody2D rigidBody;
    private HashSet<ProjectileController> projectileSet;
    private bool allowFire = true;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        projectileSet = new HashSet<ProjectileController>();
    }

    private void FixedUpdate() 
    {
		rigidBody.velocity = new Vector2(Input.GetAxis("MoveHorizontal") * walkSpeed, Input.GetAxis("MoveVertical") * walkSpeed);
	}

    private void Update()
    {
        Vector2 fireVelocity = new Vector2(Input.GetAxis("FireHorizontal"), Input.GetAxis("FireVertical")).normalized;
        fireVelocity.Scale(new Vector2(projectileSpeed, projectileSpeed));
        if (fireVelocity.magnitude > 0 && allowFire) 
        {
            // TODO: Limit Fire Rate using the "fireRate" variable
            StartCoroutine(FireProjectile(fireVelocity));
        }

    }

    private IEnumerator FireProjectile(Vector2 fireVelocity)
    {
        allowFire = false;
        ProjectileController projectile = getUnusedProjectile();
        projectile.fireWithVelocity(fireVelocity, transform.position);
        yield return new WaitForSeconds(1f/fireRate);
        allowFire = true;
    }

    private ProjectileController getUnusedProjectile()
    {
        foreach (ProjectileController projectileController in projectileSet) {
            if (!projectileController.gameObject.activeSelf) {
                return projectileController;
            }
        }

        // if we get through the entire set without finding an unused projectile, just make a new one
        GameObject projectile = Instantiate(ProjectilePrefab);
        projectile.SetActive(false);
        ProjectileController newProjectileController = projectile.GetComponent<ProjectileController>();
        projectileSet.Add(newProjectileController);
        return newProjectileController;
    }


}
