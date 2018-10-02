using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public WallsController wallsController;
	
	// Update is called once per frame
	void Update () {
        Vector3 positionOfCenterOfRoom = wallsController.PositionOfCenter();
        Debug.Log(positionOfCenterOfRoom.x);
        transform.position = new Vector3(positionOfCenterOfRoom.x, positionOfCenterOfRoom.y, transform.position.z);
	}
}
