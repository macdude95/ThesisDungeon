using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public WallsController wallsController;
    public GameObject player;

    private void Awake()
    {
        wallsController.SetupRoom(RoomConnectionType.North);
        Vector3Int entrancePosition = wallsController.PositionOfEntrance();
        player.transform.position = entrancePosition;
    }
}
