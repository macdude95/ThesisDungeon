using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public WallsController wallsController;
    public GameObject player;

    private void Awake()
    {
        RoomConnectionType[] connections = { RoomConnectionType.North, RoomConnectionType.South };
        wallsController.SetupRoom(connections, 0.15f, 20, 20);
        Vector3Int entrancePosition = wallsController.PositionOfRoomConnection(RoomConnectionType.North);
        player.transform.position = entrancePosition;
    }
}
