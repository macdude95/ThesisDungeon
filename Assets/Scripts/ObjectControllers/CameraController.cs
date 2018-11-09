using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameController gameController;

    // Update is called once per frame
    void Update() {
        Vector3 center = gameController.centerOfScreen;
        transform.position = new Vector3(center.x, center.y, transform.position.z);
    }
}
