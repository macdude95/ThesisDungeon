using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public RoomSetUp wallsController;
    public GameObject player;

    private void Awake()
    {
        RoomConnection[] connections = { RoomConnection.North, RoomConnection.South };
        wallsController.SetupRoom(connections, 0.15f, 20, 20);
        Vector3Int entrancePosition = wallsController.PositionOfRoomConnection(RoomConnection.North);
        player.transform.position = entrancePosition;
    }
}
