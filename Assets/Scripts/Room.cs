using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SettlersEngine;

public enum RoomConnection { North, South, East, West }//, Up, Down, NextLevel }

public class Room 
{
    public const int MaximumWidth = 20;
    public const int MaximumHeight = 13;
    
    public int width;
    public int height;
    public float density;
    public Tile[,] Grid;
    public RoomConnection[] Connections;

    public Room(int width, int height, float density, RoomConnection[] roomConnections)
    {
        if (width <= 0 || height <= 0 || width > MaximumWidth || height > MaximumHeight || roomConnections.Length <= 0 || density < 0 || density > 1)
        {
            throw new ArgumentOutOfRangeException();
        }

        this.width = System.Math.Min(width, MaximumWidth);
        this.height = System.Math.Min(height, MaximumHeight);
        this.density = density;
        this.Connections = roomConnections;
        this.Grid = new Tile[this.width, this.height];
        setupWalls();
    }

    private void setupWalls()
    {
        // Create rooms until a valid one is complete
        bool allPathsArePossible = true;
        do
        {
            placeExteriorWalls();
            placeInteriorWalls();

            SpatialAStar<Tile, System.Object> aStar = new SpatialAStar<Tile, System.Object>(Grid);
            allPathsArePossible = true;

            // Check to make sure all paths are possible
            RoomConnection connection = Connections[0]; // if all can connect to this connection, then they can all connect to each other
            Vector3Int position1 = PositionOfRoomConnection(connection);
            foreach (RoomConnection otherConnection in Connections)
            {
                if (connection == otherConnection) { continue; }
                Vector3Int position2 = PositionOfRoomConnection(otherConnection);

                LinkedList<Tile> path = aStar.Search(PositionOfRoomConnection(connection), PositionOfRoomConnection(otherConnection), null);
                bool pathIsPossible = path != null;
                Debug.Log("Is there a path between " + connection + " and " + otherConnection + "?: " + pathIsPossible);
                if (!pathIsPossible) { allPathsArePossible = false; }
            }
        } while (!allPathsArePossible);
    }

    private void placeExteriorWalls()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Grid[x, y] = new Tile(TileType.Ground);
                bool isOnEdge = x == width - 1 || x == 0 || y == height - 1 || y == 0;
                if (isOnEdge)
                {
                    Grid[x, y].type = TileType.Wall;
                }
            }
        }

        foreach (RoomConnection connection in Connections)
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
            int x = UnityEngine.Random.Range(1, width - 1);
            int y = UnityEngine.Random.Range(1, height - 1);
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
                y = height - 1;
                break;
            case RoomConnection.South:
                x = width / 2;
                y = 0;
                break;
            case RoomConnection.East:
                x = width - 1;
                y = height / 2;
                break;
            case RoomConnection.West:
                x = 0;
                y = height / 2;
                break;

        }
        return new Vector3Int(x, y, 0);
    }

    public Vector3 PositionOfCenter() 
    {
        Vector3Int eastPosition = PositionOfRoomConnection(RoomConnection.East);
        Vector3Int westPosition = PositionOfRoomConnection(RoomConnection.West);

        return Vector3.Lerp(eastPosition, westPosition, 0.5f);
    }

    public int NumberOfExteriorWalls()
    {
        // TODO: Fix this?? i might actualyl need to subtract certain room connections to be more accurate
        return width * 2 + height * 2 - 4; // subtract 4 because corners are counted twice
    }

    public int NumberOfInteriorWalls()
    {
        return (int)((width - 2) * (height - 2) * density);
    }


}
