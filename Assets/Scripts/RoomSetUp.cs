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


public class RoomSetUp : MonoBehaviour
{

    public GameObject WallPrefab;
    private List<GameObject> wallObjects;
    private int currentWall = 0;

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

        // Put the wall gameobjects in the right positions
        PositionTiles();

        AstarPath.active.Scan();
    }

    public void SetupRoom(RoomConnection[] roomConnections, float density = 0.3f, int width = 11, int height = 11)
    {
        SetupRoom(new Room(width, height, density, roomConnections));
    }

    public Vector3 PositionOfCenter()
    {
        return room.PositionOfCenter();
    }

    private void SetWallObejctsToActive()
    {
        int totalNumberToSetActive = room.NumberOfExteriorWalls() + room.NumberOfInteriorWalls();
        foreach (GameObject wall in wallObjects)
        {
            if (totalNumberToSetActive > 0)
            {
                wall.SetActive(true);
                totalNumberToSetActive--;
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

    private void PositionTiles() 
    {
        currentWall = 0;
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.height; y++)
            {
                PositionTile(room.Grid[x, y].type, new Vector2Int(x, y));
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