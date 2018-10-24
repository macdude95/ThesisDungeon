using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject player;
    public Level currentLevel;
    public RoomController currentRoomController
    {
        get
        {
            return roomControllers[currentRoomLocation.x, currentRoomLocation.y, currentRoomLocation.z];
        }
    }
    public int numberOfHeartsAtStart = 3;
    private RoomController[,,] roomControllers;
    private Vector3Int currentRoomLocation;

    private void Awake()
    {
        // initialization
        currentLevel = new Level();
        createRooms();
    }

    public void enterRoomConnection(RoomConnection roomConnection)
    {
        if (roomConnection == RoomConnection.NextLevel)
        {
            exitLevel();
            return;
        }
        currentRoomController.gameObject.SetActive(false);
        currentRoomLocation = Level.getNextRoomLocation(currentRoomController.room, roomConnection);
        RoomController nextRoom = currentRoomController;
        RoomConnection entranceConnection = Room.oppositeOfRoomConnection(roomConnection);
        Vector3Int playerStartingPositionInNextRoom = nextRoom.room.EntrancePositionOfRoomConnection(entranceConnection);
        PutPlayerInRoomAtPosition(nextRoom, playerStartingPositionInNextRoom);
    }

    private void exitLevel()
    {
        print("Level Complete!");
        reloadLevel();
    }

    public void reloadLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void Start()
    {
        // start player in entrance room
        currentRoomLocation = currentLevel.entranceRoomLocation;
        Vector3Int entrancePosition = currentRoomController.room.PositionOfRoomConnection(RoomConnection.North) + Vector3Int.down;
        PutPlayerInRoomAtPosition(currentRoomController, entrancePosition);
    }

    private void PutPlayerInRoomAtPosition(RoomController roomController, Vector3Int position) 
    {
        roomController.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().MoveToPositionInNewRoom(position);
        resetAStarPathingSystem();
    }

    private void resetAStarPathingSystem()
    {
        int width = (int)(currentRoomController.room.width / AstarPath.active.data.gridGraph.nodeSize);
        int length = (int)(currentRoomController.room.length / AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.center = currentRoomController.room.PositionOfCenter();
        //AstarPath.active.data.gridGraph.SetDimensions(width, length, AstarPath.active.data.gridGraph.nodeSize);
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
