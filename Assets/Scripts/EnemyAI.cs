using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour {

    public Transform target;

    // How many times per second we want to update our path
    public float updateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;

    // The calculated path
    public Path path;

    // The AI's Speed
    public float speed = 300f;
    public ForceMode2D forceMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    // The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 0.5f;

    // The waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null) {
            //Debug.LogError("No target found.");
            return;
        }

        // Start a new path to the target position and return the result to the OnPathCOmplete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath() {
        if (target == null) {
            // TODO: Insert player search here
            yield return false;
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path path) {
        //Debug.Log("We got a path, did it have an error?" + path.error);

        if (!path.error) {
            this.path = path;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            // TODO: Insert player search here
            return;
        }

        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            if (pathIsEnded) {
                return;
            }
            //Debug.Log("End of path reached.");
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        // Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 force = dir * speed * Time.fixedDeltaTime;

        // Move the AI
        rb.AddForce(force, forceMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }
}
