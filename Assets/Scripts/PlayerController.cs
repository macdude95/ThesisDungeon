using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 5f;
    public float projectileSpeed = 7f;
    public float fireRate = 4f;
    public GameObject ProjectilePrefab;
    public GameObject HeartsPrefab;
    public int maxHealth;
    private int health
    {
        get {
            return hearts.numberOfHearts;
        }
    }
    private HeartsController hearts;
    private Rigidbody2D rigidBody;
    private HashSet<ProjectileController> projectileSet;
    private bool allowFire = true;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        projectileSet = new HashSet<ProjectileController>();

        GameObject heartsGameObject = Instantiate(HeartsPrefab, gameObject.transform.parent);
        hearts = heartsGameObject.GetComponent<HeartsController>();
        hearts.SetupHearts(this);
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
            StartCoroutine(FireProjectile(fireVelocity));
        }

    }

    public void MoveToPositionInNewRoom(Vector3Int position)
    {
        transform.position = position;
        foreach (ProjectileController projectile in projectileSet)
        {
            projectile.gameObject.SetActive(false);
        }
    }

    public void die()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameController>().loadNewLevel();
        print("You died!");
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // TODO: Possibly add force so the player gets knocked back?
            hearts.SubtractHeart();
        }
    }

}
