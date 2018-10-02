using System.Collections;
using System.Collections.Generic;

public class Room 
{
    public const int MaximumWidth = 20;
    public const int MaximumHeight = 13;
    
    public int width;
    public int height;
    public float density;
    public TileType[,] Walls;
    public RoomConnection[] Connections;

    public Room(int width, int height, float density, RoomConnection[] roomConnections)
    {
        this.width = System.Math.Min(width, MaximumWidth);
        this.height = System.Math.Min(height, MaximumHeight);
        this.density = density;
        this.Connections = roomConnections;
        //this.Walls = new
        setupWalls();
    }

    private void setupWalls()
    {

    }
}
