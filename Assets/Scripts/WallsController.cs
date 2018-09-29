using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: Idea for spread of the walls... doing a Poisson disc sampling 
// https://en.wikipedia.org/wiki/Supersampling#Poisson_disc 
// https://gamedev.stackexchange.com/questions/129642/how-to-randomly-place-entities-that-dont-overlap
// https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
// https://bost.ocks.org/mike/algorithms/
// Other note: after reading more about it probalby isn't super useful to me... we will see

public class WallsController : MonoBehaviour
{

    public GameObject wallPrefab;
    public int maxXDimension = 10;
    public int maxYDimension = 10;
    [Range(0,1)]
    public float density = 0.3f;
    private List<GameObject> wallObjects;
    private int numberOfInteriorWalls;
    private int currentWall = 0;
    private TileType[,] walls;
    private enum TileType { Ground, Wall };

    void Awake()
    {
        wallObjects = new List<GameObject>();
        walls = new TileType[maxXDimension, maxYDimension];
    }

    public void SetUpWalls()
    {
        CreateWallObjects();
        PlaceExteriorWalls();
        PlaceInteriorWalls();
        PositionGameObjects();
        AstarPath.active.Scan();
    }

    private void CreateWallObjects() 
    {
        int numberOfExteriorWalls = maxXDimension * 2 + maxYDimension * 2 - 4; // subtract 4 because corners are counted twice
        numberOfInteriorWalls = (int)((maxXDimension - 2) * (maxYDimension - 2) * density);
        Debug.Log("Number of interior walls: " + numberOfInteriorWalls);
        for (int i = 0; i < numberOfExteriorWalls + numberOfInteriorWalls; i++)
        {
            GameObject w = Instantiate(wallPrefab, gameObject.transform);
            wallObjects.Add(w);
        }
    }

    private void PlaceExteriorWalls()
    {
        for (int x = 0; x < maxXDimension; x++) {
            for (int y = 0; y < maxYDimension; y++) {
                if (x == maxXDimension - 1 || x == 0 || y == maxYDimension - 1 || y == 0)
                {
                    walls[x, y] = TileType.Wall;
                }
            }
        }

    }


    private void PlaceInteriorWalls()
    {
        int occupiedSpots = 0;
        while (occupiedSpots < numberOfInteriorWalls)
        {
            int x = Random.Range(1, maxXDimension - 2);
            int y = Random.Range(1, maxYDimension - 2);
            if (walls[x,y] != TileType.Wall)
            {
                walls[x, y] = TileType.Wall;
                occupiedSpots++;
            }
        }
    }

    private void PositionGameObjects() 
    {
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
                currentWall++;
                break;
        }
    }

}