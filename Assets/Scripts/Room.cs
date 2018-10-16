using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SettlersEngine;

public enum RoomConnection { North, South, East, West, Up, Down, NextLevel }

public class Room 
{
    public const int MaximumWidth = 20;
    public const int MaximumLength = 13;

    public readonly Vector3Int location;
    public int width;
    public int length;
    public float density;
    public Tile[,] Grid;
    public List<RoomConnection> roomConnections;
    public bool isEntrance;
    public bool notAccessible {
        get {
            return roomConnections.Count == 0;
        }
    }

    public Room(Vector3Int location, int width, int length, float density, List<RoomConnection> roomConnections, bool isEntrance = false)
    {
        if (width <= 0 || length <= 0 || width > MaximumWidth || length > MaximumLength || density < 0 || density > 1)
        {
            throw new System.ArgumentOutOfRangeException();
        }

        this.location = location;
        this.width = System.Math.Min(width, MaximumWidth);
        this.length = System.Math.Min(length, MaximumLength);
        this.density = density;
        this.roomConnections = roomConnections;
        this.isEntrance = isEntrance;
        this.Grid = new Tile[this.width, this.length];
        if (notAccessible) { return; }
        setupWalls();
    }

    private void setupWalls()
    {
        if (isEntrance)
        {
            placeExteriorWallsAndInitializeGrid();
            placeStairs();
            return;
        }

        // Create rooms until a valid one is complete
        bool allPathsArePossible = true;
        do
        {
            placeExteriorWallsAndInitializeGrid();
            placeStairs();
            placeInteriorWalls();

            SpatialAStar<Tile, System.Object> aStar = new SpatialAStar<Tile, System.Object>(Grid);
            allPathsArePossible = true;

            // Check to make sure all paths are possible
            RoomConnection connection = roomConnections[0]; // if all can connect to this connection, then they can all connect to each other
            foreach (RoomConnection otherConnection in roomConnections)
            {
                if (connection == otherConnection) { continue; }

                LinkedList<Tile> path = aStar.Search(PositionOfRoomConnection(connection), PositionOfRoomConnection(otherConnection), null);
                bool pathIsPossible = path != null;
                if (!pathIsPossible) { allPathsArePossible = false; }
            }
        } while (!allPathsArePossible);
    }

    private void placeStairs()
    {
        if (!hasStairs()) { return; }
        Vector3Int center = PositionOfCenter();
        Grid[center.x, center.y].type = roomConnections.Contains(RoomConnection.Up) ? TileType.Upstairs : TileType.Downstairs;
        for (int i = -1; i <= 1; i++) 
        {
            Grid[center.x + i, center.y + 1].type = TileType.Wall;
            Grid[center.x + i, center.y - 1].type = TileType.Wall;
        }
        int xPositionOfBackWall = Random.Range(0, 2) == 0 ? center.x + 1 : center.x - 1;
        Grid[xPositionOfBackWall, center.y].type = TileType.Wall;
    }

    private bool hasStairs() {
        return roomConnections.Contains(RoomConnection.Up) || roomConnections.Contains(RoomConnection.Down);
    }

    private void placeExteriorWallsAndInitializeGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < length; y++)
            {
                if (Grid[x,y] == null)
                {
                    Grid[x, y] = new Tile(TileType.Ground);
                }
                else {
                    Grid[x, y].type = TileType.Ground;
                }
                bool isOnEdge = x == width - 1 || x == 0 || y == length - 1 || y == 0;
                if (isOnEdge)
                {
                    Grid[x, y].type = TileType.Wall;
                }
            }
        }

        foreach (RoomConnection connection in roomConnections)
        {
            Vector3Int connectionPosition = PositionOfRoomConnection(connection);
            Grid[connectionPosition.x, connectionPosition.y].type = TileType.Ground;
        }
    }

    private void placeInteriorWalls()
    {
        int occupiedSpots = 0;
        while (occupiedSpots < NumberOfInteriorWalls())
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, length - 1);
            if (Grid[x, y].type == TileType.Ground)
            {
                Grid[x, y].type = TileType.Wall;
                occupiedSpots++;
            }
        }
    }

    public Vector3Int PositionOfRoomConnection(RoomConnection roomConnection)
    {
        int x = 0, y = 0;
        switch (roomConnection)
        {
            case RoomConnection.North:
                x = width / 2;
                y = length - 1;
                break;
            case RoomConnection.South:
                x = width / 2;
                y = 0;
                break;
            case RoomConnection.East:
                x = width - 1;
                y = length / 2;
                break;
            case RoomConnection.West:
                x = 0;
                y = length / 2;
                break;
            case RoomConnection.Up:
            case RoomConnection.Down:
            case RoomConnection.NextLevel:
                x = width / 2;
                y = length / 2;
                break;


        }
        return new Vector3Int(x, y, 0);
    }

    public Vector3Int PositionOfCenter() 
    {
        return PositionOfRoomConnection(RoomConnection.Up);
    }

    public int NumberOfExteriorWalls()
    {
        int total = width * 2 + length * 2 - 4; // subtract 4 because corners are counted twice
        int numOfExteriorConnections = 0;
        RoomConnection[] exteriorConnections = { RoomConnection.North, RoomConnection.South, RoomConnection.East, RoomConnection.West };
        foreach (RoomConnection rc in exteriorConnections)
        {
            if (roomConnections.Contains(rc))
            {
                numOfExteriorConnections += 1;
            }
        }
        return total - numOfExteriorConnections;
    }

    public int NumberOfInteriorWalls()
    {   int totelNum = (int)((width - 2) * (length - 2) * density);
        int numOfUpOrDownWalls = hasStairs() ? 9 : 0;
        return totelNum - numOfUpOrDownWalls;
    }

    public static RoomConnection oppositeOfRoomConnection(RoomConnection roomConnection)
    {
        switch (roomConnection)
        {
            case RoomConnection.North:
                return RoomConnection.South;
            case RoomConnection.South:
                return RoomConnection.North;
            case RoomConnection.East:
                return RoomConnection.West;
            case RoomConnection.West:
                return RoomConnection.East;
            case RoomConnection.Up:
                return RoomConnection.Down;
            case RoomConnection.Down:
                return RoomConnection.Up;
            default:
                return RoomConnection.NextLevel;
        }
    }

}
