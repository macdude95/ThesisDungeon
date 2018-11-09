using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelController : MonoBehaviour {
    public GameObject roomPrefab;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Level level;
    public RoomController currentRoomController {
        get {
            return roomControllers[currentRoomLocation.x, currentRoomLocation.y, currentRoomLocation.z];
        }
    }
    private RoomController[,,] roomControllers;
    private Vector3Int currentRoomLocation;

    private void Awake() {
        GenerateLevel();
    }

    private void GenerateLevel() {
        // initialization
        level = new Level();
        createRooms();
    }

    public void StartLevel() {
        // start player in entrance room
        currentRoomLocation = level.entranceRoomLocation;
        Vector3Int entrancePosition = currentRoomController.room.PositionOfRoomConnection(RoomConnection.North) + Vector3Int.down;
        PutPlayerInRoomAtPosition(currentRoomController, entrancePosition);
        FindObjectOfType<StatsController>().StartNewLevel(level);
    }

    public void enterRoomConnection(RoomConnection roomConnection) {
        if (roomConnection == RoomConnection.NextLevel) {
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

    private void exitLevel() {
        print("Level Complete!");
        FindObjectOfType<GameController>().loadInBetweenLevel();
        FindObjectOfType<StatsController>().FinishLevel();
    }

    private void PutPlayerInRoomAtPosition(RoomController roomController, Vector3Int position) {
        roomController.gameObject.SetActive(true);
        player.GetComponent<PlayerController>().MoveToPositionInNewRoom(position);
        resetAStarPathingSystem();
    }

    private void resetAStarPathingSystem() {
        int width = (int)(currentRoomController.room.width / AstarPath.active.data.gridGraph.nodeSize);
        int length = (int)(currentRoomController.room.length / AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.data.gridGraph.center = currentRoomController.room.PositionOfCenter();
        //AstarPath.active.data.gridGraph.SetDimensions(width, length, AstarPath.active.data.gridGraph.nodeSize);
        AstarPath.active.Scan();
    }

    private void createRooms() {
        roomControllers = new RoomController[level.width, level.length, level.height];
        for (int x = 0; x < level.width; x++) {
            for (int y = 0; y < level.length; y++) {
                for (int z = 0; z < level.height; z++) {
                    GameObject roomObject = Instantiate(roomPrefab);
                    roomObject.SetActive(false);
                    roomControllers[x, y, z] = roomObject.GetComponent<RoomController>();
                    roomControllers[x, y, z].SetupRoom(level.rooms[x, y, z]);
                }
            }
        }
    }
}
