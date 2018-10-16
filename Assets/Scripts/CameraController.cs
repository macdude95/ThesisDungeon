using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameController gameController;
	
	// Update is called once per frame
	void Update () {
        Vector3 positionOfCenterOfRoom = gameController.currentRoomController.room.PositionOfCenter();
        transform.position = new Vector3(positionOfCenterOfRoom.x, positionOfCenterOfRoom.y, transform.position.z);
	}
}
