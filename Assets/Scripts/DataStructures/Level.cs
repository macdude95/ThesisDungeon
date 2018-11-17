using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.Linq;

/* Notes about how levels are designed:
 * 3d array of rooms
 * start on top level in a room with openings on all sides
 * Determine critical path similar to spelunky mark brown… https://www.youtube.com/watch?v=Uqk5Zf0tw3o
 * 
 */

public class Level {
    public int width;
    public int length;
    public int height;
    private RoomBuilder[,,] roomBuilders;
    public Vector3Int entranceRoomLocation;
    public Room[,,] rooms;

    private class RoomBuilder {
        public HashSet<RoomConnection> roomConnections;
        public bool isOnCriticalPath;
        public Room room;
        public readonly bool isEntrance;
        public readonly Vector3Int location;
        public readonly int maxNumberOfEnemies;

        public RoomBuilder(int x, int y, int z, bool isOnCriticalPath, bool isEntrance = false, int maxNumberOfEnemies = 2) {
            this.isOnCriticalPath = isOnCriticalPath;
            this.roomConnections = new HashSet<RoomConnection>();
            this.isEntrance = isEntrance;
            this.location = new Vector3Int(x, y, z);
            this.maxNumberOfEnemies = maxNumberOfEnemies;
        }

        public Room BuildRoom(Level level, int width = 15, int length = 9, float density = 0.15f) {
            RoomConnection[] roomConnectionsArray = new RoomConnection[roomConnections.Count];
            roomConnections.CopyTo(roomConnectionsArray);
            this.room = new Room(level, location, width, length, density, roomConnections.ToList(), isEntrance, maxNumberOfEnemies);
            return this.room;
        }
    }

    public Level(int width = 3, int length = 3, int height = 3) {
        this.width = width;
        this.length = length;
        this.height = height;
        this.roomBuilders = new RoomBuilder[width, length, height];
        this.rooms = new Room[width, length, height];
        this.createCriticalPath();
        this.addMoreRooms();
        this.buildRooms();
    }

    public static Vector3Int getNextRoomLocation(Room currentRoom, RoomConnection roomConnection) {
        Vector3Int currentLocation = currentRoom.location;
        switch (roomConnection) {
            case RoomConnection.North:
                currentLocation.y++;
                break;
            case RoomConnection.South:
                currentLocation.y--;
                break;
            case RoomConnection.East:
                currentLocation.x++;
                break;
            case RoomConnection.West:
                currentLocation.x--;
                break;
            case RoomConnection.Down:
                currentLocation.z--;
                break;
            case RoomConnection.Up:
                currentLocation.z++;
                break;
        }
        return currentLocation;
    }

    private void buildRooms() {
        for (int z = height - 1; z >= 0; z--) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    rooms[x, y, z] = roomBuilders[x, y, z].BuildRoom(this);
                }
            }
        }
    }

    private void createCriticalPath() {
        // Create entrance room
        int x = Random.Range(0, width);
        int y = Random.Range(0, length);
        int z = height - 1; // NOTE: z is the vertical axis
        entranceRoomLocation = new Vector3Int(x, y, z);
        RoomBuilder entranceRoomBuilder = new RoomBuilder(x, y, z, true, true, 0);
        roomBuilders[x, y, z] = entranceRoomBuilder;

        RoomBuilder currentRoomBuilder = entranceRoomBuilder;
        while (z >= 0) // technically this condition will never be met because we break part way through the loop
        {
            // randomly move north, south, east, west, or down
            List<RoomConnection> possilbeConnections = new List<RoomConnection>();
            // first condition makes sure we don't go outside of dimensions, and second makes sure we don't go back to the critical path
            if (x > 0 && roomBuilders[x - 1, y, z] == null) { possilbeConnections.Add(RoomConnection.West); }
            if (y > 0 && roomBuilders[x, y - 1, z] == null) { possilbeConnections.Add(RoomConnection.South); }
            if (x < width - 1 && roomBuilders[x + 1, y, z] == null) { possilbeConnections.Add(RoomConnection.East); }
            if (y < length - 1 && roomBuilders[x, y + 1, z] == null) { possilbeConnections.Add(RoomConnection.North); }
            if (!currentRoomBuilder.roomConnections.Contains(RoomConnection.Up) && currentRoomBuilder != entranceRoomBuilder) { possilbeConnections.Add(RoomConnection.Down); }
            RoomConnection randomRoomConnection = possilbeConnections[Random.Range(0, possilbeConnections.Count)];
            // adjust x, y, or z value
            switch (randomRoomConnection) {
                case RoomConnection.North:
                    y++;
                    break;
                case RoomConnection.South:
                    y--;
                    break;
                case RoomConnection.East:
                    x++;
                    break;
                case RoomConnection.West:
                    x--;
                    break;
                case RoomConnection.Down:
                    z--;
                    break;
            }

            // If we try to exit out the bottom of the level, then we are done
            if (z < 0) { break; }

            currentRoomBuilder.roomConnections.Add(randomRoomConnection);
            RoomBuilder nextRoom = new RoomBuilder(x, y, z, true);
            roomBuilders[x, y, z] = nextRoom;
            nextRoom.roomConnections.Add(Room.oppositeOfRoomConnection(randomRoomConnection));

            currentRoomBuilder = nextRoom;
        }

        // z is negative so now this room is the exit room
        currentRoomBuilder.roomConnections.Add(RoomConnection.NextLevel);
    }

    private void addMoreRooms() {
        // fill in all the blanks in the "rooms"
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                for (int z = 0; z < height; z++) {
                    RoomBuilder roomBuilder = roomBuilders[x, y, z];
                    if (roomBuilder == null) {
                        roomBuilder = new RoomBuilder(x, y, z, false);
                        roomBuilders[x, y, z] = roomBuilder;
                    }

                    if (roomBuilder.isOnCriticalPath) {
                        roomBuilder.roomConnections.UnionWith(getPossilbeConnectionsAtPositionOnThisFloor(x, y));
                    } else {
                        List<RoomConnection> possibleRoomConnections = getPossilbeConnectionsAtPositionOnThisFloor(x, y);
                        possibleRoomConnections.Shuffle();
                        int numberOfConnections = Random.Range(0, possibleRoomConnections.Count);
                        for (int i = 0; i < numberOfConnections; i++) {
                            roomBuilder.roomConnections.Add(possibleRoomConnections[i]);
                        }
                    }
                }
            }
        }

        // clean up. If room 1 connects to room 2 but not vice versa, remove that room connection

        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < length; y++) {
                    RoomBuilder currentRoom = roomBuilders[x, y, z];
                    // check adjacent rooms in x, y, and z dimensions in the positive direction
                    RoomBuilder eastRoom = x + 1 < width ? roomBuilders[x + 1, y, z] : null;
                    RoomBuilder northRoom = y + 1 < length ? roomBuilders[x, y + 1, z] : null;
                    if (eastRoom != null) {
                        if ((currentRoom.roomConnections.Contains(RoomConnection.East) && !eastRoom.roomConnections.Contains(RoomConnection.West))
                            || (!currentRoom.roomConnections.Contains(RoomConnection.East) && eastRoom.roomConnections.Contains(RoomConnection.West))) {
                            currentRoom.roomConnections.Remove(RoomConnection.East);
                            eastRoom.roomConnections.Remove(RoomConnection.West);
                        }
                    }
                    if (northRoom != null) {
                        if ((currentRoom.roomConnections.Contains(RoomConnection.North) && !northRoom.roomConnections.Contains(RoomConnection.South))
                            || (!currentRoom.roomConnections.Contains(RoomConnection.North) && northRoom.roomConnections.Contains(RoomConnection.South))) {
                            currentRoom.roomConnections.Remove(RoomConnection.North);
                            northRoom.roomConnections.Remove(RoomConnection.South);
                        }
                    }
                }
            }
        }
    }

    private List<RoomConnection> getPossilbeConnectionsAtPositionOnThisFloor(int x, int y) {
        List<RoomConnection> possibleRoomConnections = new List<RoomConnection>();
        if (x > 0) { possibleRoomConnections.Add(RoomConnection.West); }
        if (y > 0) { possibleRoomConnections.Add(RoomConnection.South); }
        if (x < width - 1) { possibleRoomConnections.Add(RoomConnection.East); }
        if (y < length - 1) { possibleRoomConnections.Add(RoomConnection.North); }
        return possibleRoomConnections;
    }

    public void printLevel() {
        for (int z = height - 1; z >= 0; z--) {
            Debug.Log("Floor: " + z);
            for (int y = 0; y < length; y++) {
                for (int x = 0; x < width; x++) {
                    Level.RoomBuilder roomBuilder = roomBuilders[x, y, z];
                    Debug.Log("Room at " + x + ", " + y + ", " + z + ". Critical path? " + roomBuilder.isOnCriticalPath);
                    if (roomBuilder.isEntrance) {
                        Debug.Log(" this is the entrance");
                    }
                    string roomConnections = "";
                    foreach (RoomConnection rc in roomBuilder.roomConnections) {
                        roomConnections += rc + ", ";
                    }
                    Debug.Log(roomConnections);
                }
            }
        }
    }

}
