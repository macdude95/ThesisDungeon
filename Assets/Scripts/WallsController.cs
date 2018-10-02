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

public enum RoomConnectionType { North, South, East, West }//, Up, Down, NextLevel }

public class WallsController : MonoBehaviour
{
    public const int maximumWidth = 20;
    public const int maximumHeight = 13;

    public GameObject wallPrefab;
    private List<GameObject> wallObjects;
    private int currentWall = 0;
    private int numberOfInteriorWalls;
    private int numberOfExteriorWalls;
    private TileType[,] walls;
    private enum TileType { Ground, Wall };

    // Variables
    private RoomConnectionType[] roomConnections;
    private float density;
    private int width;
    private int height;


    void Awake()
    {
        wallObjects = new List<GameObject>();
        CreateWallObjects();
    }

    public void SetupRoom(RoomConnectionType[] roomConnections, float density = 0.3f, int width = 11, int height = 11)
    {
        // Set instance variables
        this.width = Mathf.Min(width, maximumWidth);
        this.height = Mathf.Min(height, maximumHeight); ;
        this.density = density;
        this.roomConnections = roomConnections;

        // Set game objects to active accordingly
        SetWallObejctsToActive();

        // Create rooms until a valid one is complete
        bool allPathsArePossible = true;
        int count = 1;
        do
        {
            Debug.Log("attempt number " + count++);
            walls = new TileType[width, height];
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
            foreach (RoomConnectionType connection in roomConnections)
            {
                GraphNode node1 = AstarPath.active.GetNearest(PositionOfRoomConnection(connection), NNConstraint.Default).node;
                foreach (RoomConnectionType otherConnection in roomConnections) 
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

    private void SetWallObejctsToActive() 
    {
        numberOfExteriorWalls = width * 2 + height * 2 - 4; // subtract 4 because corners are counted twice
        numberOfInteriorWalls = (int)((width - 2) * (height - 2) * density);
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

    public Vector3 PositionOfCenter()
    {
        Vector3Int eastPosition = PositionOfRoomConnection(RoomConnectionType.East);
        Vector3Int westPosition = PositionOfRoomConnection(RoomConnectionType.West);

        return Vector3.Lerp(eastPosition, westPosition, 0.5f);
    }

    public Vector3Int PositionOfRoomConnection(RoomConnectionType roomConnection)
    {
        int x = 0, y = 0;
        switch (roomConnection)
        {
            case RoomConnectionType.North:
                x = width / 2;
                y = height - 1;
                break;
            case RoomConnectionType.South:
                x = width / 2;
                y = 0;
                break;
            case RoomConnectionType.East:
                x = width - 1;
                y = height / 2;
                break;
            case RoomConnectionType.West:
                x = 0;
                y = height / 2;
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

    private void CreateWallObjects() 
    {
        for (int i = 0; i < maximumWidth * maximumHeight; i++)
        {
            GameObject w = Instantiate(wallPrefab, gameObject.transform);
            w.SetActive(false);
            wallObjects.Add(w);
        }
    }

    private void PlaceExteriorWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isOnEdge = x == width - 1 || x == 0 || y == height - 1 || y == 0;
                if (isOnEdge)
                {
                    walls[x, y] = TileType.Wall;
                }
            }
        }

        foreach (RoomConnectionType connection in roomConnections)
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
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
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