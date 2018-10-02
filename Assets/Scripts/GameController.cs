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
        Room r = new Room(20, 13, 0.35f, connections);
        wallsController.SetupRoom(r);
        Vector3Int entrancePosition = r.PositionOfRoomConnection(RoomConnection.North);
        player.transform.position = entrancePosition;
    }
}
