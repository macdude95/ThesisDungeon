using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public GameObject roomPrefab;
    private RoomController[,,] roomControllers;
    public GameObject player;
    public Level currentLevel;
    private Vector3Int currentRoomLocation;
    public RoomController currentRoomController {
        get {
            return roomControllers[currentRoomLocation.x, currentRoomLocation.y, currentRoomLocation.z];
        }
    }

    private void Awake()
    {
        // initialization
        currentLevel = new Level();
        createRooms();
    }

    private void Start()
    {
        // start player in entrance room
        currentRoomLocation = currentLevel.entranceRoomLocation;
        Vector3Int entrancePosition = currentRoomController.room.PositionOfRoomConnection(RoomConnection.North) + Vector3Int.down;
        PutPlayerInRoomAtPosition(currentRoomController, entrancePosition);
    }

    public void enterRoomConnection(RoomConnection roomConnection)
    {
        currentRoomController.gameObject.SetActive(false);
        currentRoomLocation = Level.getNextRoomLocation(currentRoomController.room, roomConnection);
        RoomController nextRoom = currentRoomController;
        RoomConnection entranceConnection = Room.oppositeOfRoomConnection(roomConnection);
        Vector3Int playerStartingPositionInNextRoom = nextRoom.room.EntrancePositionOfRoomConnection(entranceConnection);
        PutPlayerInRoomAtPosition(nextRoom, playerStartingPositionInNextRoom);
    }

    private void PutPlayerInRoomAtPosition(RoomController roomController, Vector3Int position) 
    {
        roomController.gameObject.SetActive(true);
        player.transform.position = position;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        AstarPath.active.Scan();
    }

    private void createRooms()
    {
        roomControllers = new RoomController[currentLevel.width, currentLevel.length, currentLevel.height];
        for (int x = 0; x < currentLevel.width; x++)
        {
            for (int y = 0; y < currentLevel.length; y++)
            {
                for (int z = 0; z < currentLevel.height; z++)
                {
                    GameObject roomObject = Instantiate(roomPrefab);
                    roomObject.SetActive(false);
                    roomControllers[x, y, z] = roomObject.GetComponent<RoomController>();
                    roomControllers[x, y, z].SetupRoom(currentLevel.rooms[x, y, z]);
                }
            }
        }
    }
}
