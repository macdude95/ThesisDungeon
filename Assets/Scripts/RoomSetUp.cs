using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

// Note: Idea for spread of the walls... doing a Poisson disc sampling 
// https://en.wikipedia.org/wiki/Supersampling#Poisson_disc 
// https://gamedev.stackexchange.com/questions/129642/how-to-randomly-place-entities-that-dont-overlap
// https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
// https://bost.ocks.org/mike/algorithms/
// Other note: after reading more about it probalby isn't super useful to me... we will see

public enum RoomConnection { North, South, East, West }//, Up, Down, NextLevel }
public enum TileType { Ground, Wall };

public class RoomSetUp : MonoBehaviour
{

    public GameObject WallPrefab;
    private List<GameObject> wallObjects;
    private int currentWall = 0;
    private int numberOfInteriorWalls;
    private int numberOfExteriorWalls;
    private TileType[,] walls;

    private Room room;


    void Awake()
    {
        wallObjects = new List<GameObject>();
        CreateWallObjects();
    }

    public void SetupRoom(Room room)
    {
        this.room = room;

        // Set game objects to active accordingly
        SetWallObejctsToActive();

        // Create rooms until a valid one is complete
        bool allPathsArePossible = true;
        int count = 1;
        do
        {
            Debug.Log("attempt number " + count++);
            walls = new TileType[room.width, room.height];
            foreach (GameObject go in wallObjects)
            {
                go.SetActive(false);
            }
            PlaceExteriorWalls();
            PlaceInteriorWalls();
            PositionTiles();
            AstarPath.active.Scan();

            allPathsArePossible = true;
            // Check to make sure all paths are possible
            foreach (RoomConnection connection in room.Connections)
            {
                GraphNode node1 = AstarPath.active.GetNearest(PositionOfRoomConnection(connection), NNConstraint.Default).node;
                foreach (RoomConnection otherConnection in room.Connections)
                {
                    if (connection == otherConnection) { continue; }
                    GraphNode node2 = AstarPath.active.GetNearest(PositionOfRoomConnection(otherConnection), NNConstraint.Default).node;

                    bool pathIsPossible = PathUtilities.IsPathPossible(node1, node2);
                    Debug.Log("Is there a path between " + connection + " and " + otherConnection + "?: " + pathIsPossible);
                    if (!pathIsPossible) { allPathsArePossible = false; }
                }
            }
        } while (!allPathsArePossible);
    }

    public void SetupRoom(RoomConnection[] roomConnections, float density = 0.3f, int width = 11, int height = 11)
    {
        SetupRoom(new Room(width, height, density, roomConnections));
    }

    public Vector3 PositionOfCenter()
    {
        Vector3Int eastPosition = PositionOfRoomConnection(RoomConnection.East);
        Vector3Int westPosition = PositionOfRoomConnection(RoomConnection.West);

        return Vector3.Lerp(eastPosition, westPosition, 0.5f);
    }

    public Vector3Int PositionOfRoomConnection(RoomConnection roomConnection)
    {
        int x = 0, y = 0;
        switch (roomConnection)
        {
            case RoomConnection.North:
                x = room.width / 2;
                y = room.height - 1;
                break;
            case RoomConnection.South:
                x = room.width / 2;
                y = 0;
                break;
            case RoomConnection.East:
                x = room.width - 1;
                y = room.height / 2;
                break;
            case RoomConnection.West:
                x = 0;
                y = room.height / 2;
                break;

        }
        return new Vector3Int(x, y, 0);
    }

    //private void ChooseRoomConnections() 
    //{
    //    // Connections to other rooms
    //    roomConnections = {};
    //    roomConnections.Add(entrance);
    //    System.Array allPossibleRoomConnections = System.Enum.GetValues(typeof(RoomConnectionType));
    //    int numberOfOtherConnections = Random.Range(1, allPossibleRoomConnections.Length);
    //    while (numberOfOtherConnections > 0) 
    //    {
    //        RoomConnectionType roomConnection = (RoomConnectionType)Random.Range(0, allPossibleRoomConnections.Length);
    //        if (!roomConnections.Contains(roomConnection))
    //        {
    //            roomConnections.Add(roomConnection);
    //            numberOfOtherConnections--;
    //        }
    //    }
    //}

    private void SetWallObejctsToActive()
    {
        numberOfExteriorWalls = room.width * 2 + room.height * 2 - 4; // subtract 4 because corners are counted twice
        numberOfInteriorWalls = (int)((room.width - 2) * (room.height - 2) * room.density);
        int totalNumberToSetActive = numberOfExteriorWalls + numberOfInteriorWalls;
        foreach (GameObject wall in wallObjects)
        {
            if (totalNumberToSetActive > 0)
            {
                wall.SetActive(true);
            }
            wall.SetActive(false);
        }
    }

    private void CreateWallObjects() 
    {
        for (int i = 0; i < Room.MaximumHeight * Room.MaximumWidth; i++)
        {
            GameObject w = Instantiate(WallPrefab, gameObject.transform);
            w.SetActive(false);
            wallObjects.Add(w);
        }
    }

    private void PlaceExteriorWalls()
    {
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                bool isOnEdge = x == room.width - 1 || x == 0 || y == room.height - 1 || y == 0;
                if (isOnEdge)
                {
                    walls[x, y] = TileType.Wall;
                }
            }
        }

        foreach (RoomConnection connection in room.Connections)
        {
            Vector3Int connectionPosition = PositionOfRoomConnection(connection);
            walls[connectionPosition.x, connectionPosition.y] = TileType.Ground;
        }
    }

    private void PlaceInteriorWalls()
    {
        int occupiedSpots = 0;
        while (occupiedSpots < numberOfInteriorWalls)
        {
            int x = Random.Range(1, room.width - 1);
            int y = Random.Range(1, room.height - 1);
            if (walls[x,y] != TileType.Wall)
            {
                walls[x, y] = TileType.Wall;
                occupiedSpots++;
            }
        }
    }

    private void PositionTiles() 
    {
        currentWall = 0;
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                PositionTile(walls[x, y], new Vector2Int(x, y));
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