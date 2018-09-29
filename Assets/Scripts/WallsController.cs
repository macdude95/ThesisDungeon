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

    public GameObject wallPrefab;
    public int maxXDimension = 11;
    public int maxYDimension = 11;
    [Range(0,1)]
    public float density = 0.3f;
    private List<GameObject> wallObjects;
    private int numberOfInteriorWalls;
    private RoomConnectionType entrance;
    private int currentWall = 0;
    private TileType[,] walls;
    private enum TileType { Ground, Wall };

    // Random Variables
    private List<RoomConnectionType> roomConnections;


    void Awake()
    {
        wallObjects = new List<GameObject>();
        roomConnections = new List<RoomConnectionType>();
        CreateWallObjects();
    }

    public void SetupRoom(RoomConnectionType entrance)
    {
        bool allPathsArePossible = true;
        int count = 1;
        do
        {
            Debug.Log("attempt number " + count++);
            this.entrance = entrance;
            walls = new TileType[maxXDimension, maxYDimension];
            foreach (GameObject go in wallObjects)
            {
                go.SetActive(false);
            }
            RandomlySetVariables();
            PlaceExteriorWalls();
            PlaceInteriorWalls();
            PositionGameObjects();
            AstarPath.active.Scan();

            allPathsArePossible = true;
            // Check to make sure all paths are possible
            GraphNode entranceNode = AstarPath.active.GetNearest(PositionOfEntrance(), NNConstraint.Default).node;
            foreach (RoomConnectionType connection in roomConnections)
            {
                if (connection == entrance) { continue; }
                GraphNode exitNode = AstarPath.active.GetNearest(PositionOfRoomConnection(connection), NNConstraint.Default).node;

                bool pathIsPossible = PathUtilities.IsPathPossible(entranceNode, exitNode);
                Debug.Log("Is there a path between " + entrance + " and " + connection + "?: " + pathIsPossible);
                if (!pathIsPossible) { allPathsArePossible = false; }
            }
        } while (!allPathsArePossible);
    }

    public Vector3Int PositionOfEntrance()
    {
        return PositionOfRoomConnection(entrance);
    }

    private Vector3Int PositionOfRoomConnection(RoomConnectionType roomConnection)
    {
        int x = 0, y = 0;
        switch (roomConnection)
        {
            case RoomConnectionType.North:
                x = maxXDimension / 2;
                y = maxYDimension - 1;
                break;
            case RoomConnectionType.South:
                x = maxXDimension / 2;
                y = 0;
                break;
            case RoomConnectionType.East:
                x = maxXDimension - 1;
                y = maxYDimension / 2;
                break;
            case RoomConnectionType.West:
                x = 0;
                y = maxYDimension / 2;
                break;

        }
        return new Vector3Int(x, y, 0);
    }

    private void RandomlySetVariables() 
    {
        ChooseRoomConnections();
    }

    private void ChooseRoomConnections() 
    {
        // Connections to other rooms
        roomConnections.Clear();
        roomConnections.Add(entrance);
        System.Array allPossibleRoomConnections = System.Enum.GetValues(typeof(RoomConnectionType));
        int numberOfOtherConnections = Random.Range(1, allPossibleRoomConnections.Length);
        while (numberOfOtherConnections > 0) 
        {
            RoomConnectionType roomConnection = (RoomConnectionType)Random.Range(0, allPossibleRoomConnections.Length);
            if (!roomConnections.Contains(roomConnection))
            {
                roomConnections.Add(roomConnection);
                numberOfOtherConnections--;
            }
        }
    }

    private void CreateWallObjects() 
    {
        int numberOfExteriorWalls = maxXDimension * 2 + maxYDimension * 2 - 4; // subtract 4 because corners are counted twice
        numberOfInteriorWalls = (int)((maxXDimension - 2) * (maxYDimension - 2) * density);
        Debug.Log("Number of interior walls: " + numberOfInteriorWalls);
        for (int i = 0; i < numberOfExteriorWalls + numberOfInteriorWalls; i++)
        {
            GameObject w = Instantiate(wallPrefab, gameObject.transform);
            w.SetActive(false);
            wallObjects.Add(w);
        }
    }

    private void PlaceExteriorWalls()
    {
        for (int x = 0; x < maxXDimension; x++)
        {
            for (int y = 0; y < maxYDimension; y++)
            {
                bool isOnEdge = x == maxXDimension - 1 || x == 0 || y == maxYDimension - 1 || y == 0;
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
            int x = Random.Range(1, maxXDimension - 1);
            int y = Random.Range(1, maxYDimension - 1);
            if (walls[x,y] != TileType.Wall)
            {
                walls[x, y] = TileType.Wall;
                occupiedSpots++;
            }
        }
    }

    private void PositionGameObjects() 
    {
        currentWall = 0;
        for (int x = 0; x < maxXDimension; x++)
        {
            for (int y = 0; y < maxYDimension; y++)
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