using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RoomController : MonoBehaviour
{

    public GameObject WallPrefab;
    public GameObject RoomConnectionPrefab;
    public Room room;
    private List<GameObject> wallObjects;
    private int currentWall = 0;
    private List<GameObject> roomConnections;

    void Awake()
    {
        wallObjects = new List<GameObject>();
        CreateWallObjects();
        roomConnections = new List<GameObject>();
    }

    public void SetupRoom(Room room)
    {
        this.room = room;
        if (room.notAccessible)
        {
            return;
        }

        // Put the wall gameobjects in the right positions
        PositionTiles();
        CreateRoomConnectionBoxColliders();
    }

    private void CreateRoomConnectionBoxColliders() {
        foreach(RoomConnection rc in room.roomConnections)
        {
            Vector3Int offset = room.PositionOfRoomConnection(rc);
            GameObject roomConnection = Instantiate(RoomConnectionPrefab, transform);
            roomConnection.transform.position = offset;
            roomConnection.GetComponent<RoomConnectionController>().SetRoomConnection(rc, room.stairsFacingLeft);
            roomConnections.Add(roomConnection);
        }
    }

    private void CreateWallObjects() 
    {
        for (int i = 0; i < Room.MaximumLength * Room.MaximumWidth; i++)
        {
            GameObject w = Instantiate(WallPrefab, gameObject.transform);
            w.SetActive(false);
            wallObjects.Add(w);
        }
    }

    private void PositionTiles() 
    {
        if (room.Grid == null) {
            print("room grid is null");
            if (room != null) {
                print("room itself it not null");
            }
        }
        currentWall = 0;
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.length; y++)
            {
                PositionTile(room.Grid[x, y].type, new Vector2Int(x, y));
            }
        }
    }

    private void PositionTile(TileType type, Vector2Int position2D) {
        Vector3Int position = new Vector3Int(position2D.x, position2D.y, 0);
        switch (type)
        {
            case TileType.Ground:
                break;
            case TileType.Wall:
                GameObject wall = wallObjects[currentWall];
                wall.transform.position = position;
                wall.SetActive(true);
                currentWall++;
                break;
        }
    }

}