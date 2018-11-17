using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SettlersEngine;
using ExtensionMethods;

public enum RoomConnection { North, South, East, West, Up, Down, NextLevel }

public class Room {
    public readonly Vector3Int location;
    public readonly int width;
    public readonly int length;
    public readonly float density;
    public readonly int numberOfEnemies;
    public readonly Tile[,] grid;
    public readonly List<RoomConnection> roomConnections;
    public readonly bool isEntrance;
    public bool notAccessible {
        get {
            return roomConnections.Count == 0;
        }
    }
    public bool? stairsFacingLeft;
    private List<Vector3Int> listOfEntrancePositoins {
        get {
            List<Vector3Int> list = new List<Vector3Int>();
            foreach (RoomConnection roomConnection in roomConnections) {
                list.Add(EntrancePositionOfRoomConnection(roomConnection));
            }
            return list;
        }
    }
    private Level level;

    public Room(Level level, Vector3Int location, int width, int length, float density, List<RoomConnection> roomConnections, bool isEntrance = false, int maxNumberOfEnemies = 2) {
        if (width <= 0 || length <= 0 || density < 0 || density > 1) {
            throw new System.ArgumentOutOfRangeException();
        }

        this.level = level;
        this.location = location;
        this.width = width;
        this.length = length;
        this.density = density;
        this.numberOfEnemies = Random.Range(0, maxNumberOfEnemies + 1);
        this.roomConnections = roomConnections;
        this.isEntrance = isEntrance;
        this.grid = new Tile[this.width, this.length];
        if (notAccessible) { return; }
        setupWalls();
        placeEnemies();

    }

    public Vector3Int EntrancePositionOfRoomConnection(RoomConnection roomConnection) {
        Vector3Int roomConnectionPosition = PositionOfRoomConnection(roomConnection);
        Vector3Int offset = Vector3Int.zero;
        switch (roomConnection) {
            case RoomConnection.North:
                offset = Vector3Int.down;
                break;
            case RoomConnection.South:
                offset = Vector3Int.up;
                break;
            case RoomConnection.East:
                offset = Vector3Int.left;
                break;
            case RoomConnection.West:
                offset = Vector3Int.right;
                break;
            case RoomConnection.Down:
            case RoomConnection.Up:
                // depends on if it is facing right or left
                offset = stairsFacingLeft.Value ? Vector3Int.left : Vector3Int.right;
                break;

        }

        return roomConnectionPosition + offset;
    }

    private void placeEnemies() {
        List<Vector2Int> emptySpots = new List<Vector2Int>();
        // dont allow enemies to spwn within 2 tils from the edges... we dont want the player walking into a room and instantly getting hit
        for (int x = 2; x < width-2; x++) {
            for (int y = 2; y < length-2; y++) {
                if (grid[x, y].type == TileType.Ground) {
                    emptySpots.Add(new Vector2Int(x, y));
                }
            }
        }

        emptySpots.Shuffle();
        for (int i = 0; i < numberOfEnemies; i++) {
            if (i >= emptySpots.Count) {
                Debug.LogError("You can't have " + numberOfEnemies + " enemies in a room with only " + emptySpots.Count + " empty spots.");
            }
            Vector2Int spot = emptySpots[i];
            grid[spot.x, spot.y].type = TileType.Enemy;
        }
    }

    private void setupWalls() {
        if (isEntrance) {
            placeExteriorWallsAndInitializeGrid();
            placeStairWalls();
            return;
        }

        // Create rooms until a valid one is complete
        bool allPathsArePossible = true;
        do {
            placeExteriorWallsAndInitializeGrid();
            placeStairWalls();
            placeInteriorWalls();

            SpatialAStar<Tile, System.Object> aStar = new SpatialAStar<Tile, System.Object>(grid);
            allPathsArePossible = true;

            // Check to make sure all paths are possible
            RoomConnection connection = roomConnections[0]; // if all can connect to this connection, then they can all connect to each other
            foreach (RoomConnection otherConnection in roomConnections) {
                if (connection == otherConnection) { continue; }

                LinkedList<Tile> path = aStar.Search(PositionOfRoomConnection(connection), PositionOfRoomConnection(otherConnection), null);
                bool pathIsPossible = path != null;
                if (!pathIsPossible) { allPathsArePossible = false; }
            }
        } while (!allPathsArePossible);
    }

    private void placeStairWalls() {
        if (!hasStairs()) { return; }
        Vector3Int center = PositionOfCenter();
        grid[center.x, center.y].type = roomConnections.Contains(RoomConnection.Up) ? TileType.Upstairs : TileType.Downstairs;
        for (int i = -1; i <= 1; i++) {
            grid[center.x + i, center.y + 1].type = TileType.Wall;
            grid[center.x + i, center.y - 1].type = TileType.Wall;
        }
        // If the room connection is DOWN, then randomly choose left or right. If UP then it depends on the above room
        this.stairsFacingLeft = roomConnections.Contains(RoomConnection.Down) ? Random.Range(0, 2) == 0 : !level.rooms[location.x, location.y, location.z + 1].stairsFacingLeft;

        int xPositionOfBackWall = stairsFacingLeft.Value ? center.x + 1 : center.x - 1;
        grid[xPositionOfBackWall, center.y].type = TileType.Wall;
    }

    private bool hasStairs() {
        return roomConnections.Contains(RoomConnection.Up) || roomConnections.Contains(RoomConnection.Down);
    }

    private void placeExteriorWallsAndInitializeGrid() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                if (grid[x, y] == null) {
                    grid[x, y] = new Tile(TileType.Ground);
                } else {
                    grid[x, y].type = TileType.Ground;
                }
                bool isOnEdge = x == width - 1 || x == 0 || y == length - 1 || y == 0;
                if (isOnEdge) {
                    grid[x, y].type = TileType.Wall;
                }
            }
        }

        foreach (RoomConnection connection in roomConnections) {
            Vector3Int connectionPosition = PositionOfRoomConnection(connection);
            grid[connectionPosition.x, connectionPosition.y].type = TileType.Ground;
        }
    }

    private void placeInteriorWalls() {
        int occupiedSpots = 0;
        List<Vector3Int> entrancePositions = listOfEntrancePositoins;
        while (occupiedSpots < NumberOfInteriorWalls()) {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, length - 1);
            if (grid[x, y].type == TileType.Ground && !listOfEntrancePositoins.Contains(new Vector3Int(x, y, 0))) {
                grid[x, y].type = TileType.Wall;
                occupiedSpots++;
            }
        }
    }

    public Vector3Int PositionOfRoomConnection(RoomConnection roomConnection) {
        int x = 0, y = 0;
        switch (roomConnection) {
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

    public Vector3Int PositionOfCenter() {
        return PositionOfRoomConnection(RoomConnection.Up);
    }

    public int NumberOfExteriorWalls() {
        int total = width * 2 + length * 2 - 4; // subtract 4 because corners are counted twice
        int numOfExteriorConnections = 0;
        RoomConnection[] exteriorConnections = { RoomConnection.North, RoomConnection.South, RoomConnection.East, RoomConnection.West };
        foreach (RoomConnection rc in exteriorConnections) {
            if (roomConnections.Contains(rc)) {
                numOfExteriorConnections += 1;
            }
        }
        return total - numOfExteriorConnections;
    }

    public int NumberOfInteriorWalls() {
        int totelNum = (int)((width - 2) * (length - 2) * density);
        int numOfUpOrDownWalls = hasStairs() ? 9 : 0;
        return totelNum - numOfUpOrDownWalls;
    }

    public static RoomConnection oppositeOfRoomConnection(RoomConnection roomConnection) {
        switch (roomConnection) {
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
