using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RoomController : MonoBehaviour
{
    public GameObject RoomConnectionPrefab;
    public GameObject EnemyPrefab;
    public Sprite WallSprite;
    public Sprite WallWithNothingBelowSprite;
    public Sprite GroundSprite;
    public Room room;
    private List<GameObject> wallObjects;
    private List<GameObject> roomConnectionObjects;
    private List<GameObject> groundObjects;
    private List<GameObject> enemyObjects;

    void Awake()
    {
        wallObjects = new List<GameObject>();
        roomConnectionObjects = new List<GameObject>();
        groundObjects = new List<GameObject>();
        enemyObjects = new List<GameObject>();
    }

    public void SetupRoom(Room room)
    {
        this.room = room;
        if (room.notAccessible)
        {
            return;
        }

        // Put the wall gameobjects in the right positions
        CreateTiles();
        CreateRoomConnectionBoxColliders();
    }

    private void CreateRoomConnectionBoxColliders() {
        foreach(RoomConnection roomConnection in room.roomConnections)
        {
            Vector3Int offset = room.PositionOfRoomConnection(roomConnection);
            GameObject roomConnectionObject = Instantiate(RoomConnectionPrefab, transform);
            roomConnectionObject.transform.position = offset;
            roomConnectionObject.GetComponent<RoomConnectionController>().SetRoomConnection(roomConnection, room.stairsFacingLeft);
            roomConnectionObjects.Add(roomConnectionObject);
        }
    }

    private void CreateTiles() 
    {
        for (int x = 0; x < room.width; x++)
        {
            for (int y = 0; y < room.length; y++)
            {
                PositionTile(room.grid[x, y].type, new Vector2Int(x, y));
            }
        }
    }

    private void PositionTile(TileType type, Vector2Int position2D) 
    {
        Vector3Int position = new Vector3Int(position2D.x, position2D.y, 0);
        switch (type)
        {
            case TileType.Enemy:
                enemyObjects.Add(Instantiate(EnemyPrefab, position,Quaternion.identity, transform));
                groundObjects.Add(createChildGameObject(type.ToString(), position, GroundSprite));
                break;
            case TileType.Ground:
                groundObjects.Add(createChildGameObject(type.ToString(), position, GroundSprite));
                break;
            case TileType.Wall:
                Sprite useThisSprite;
                if (position.y == 0)
                {
                    useThisSprite = WallWithNothingBelowSprite;
                }
                else
                {
                    TileType tileBelowType = room.grid[position.x, position.y - 1].type;
                    if (tileBelowType == TileType.Wall || tileBelowType == TileType.Upstairs) {
                        useThisSprite = WallSprite;
                    }
                    else 
                    {
                        useThisSprite = WallWithNothingBelowSprite;
                    }
                }
                GameObject wall = createChildGameObject(type.ToString(), position, useThisSprite);
                BoxCollider2D boxCollider = wall.AddComponent<BoxCollider2D>();
                boxCollider.usedByComposite = true;
                wallObjects.Add(wall);
                break;
        }
    }

    private GameObject createChildGameObject(string childObjectName, Vector3Int position, Sprite sprite = null)
    {
        GameObject childGameObject = new GameObject(childObjectName);
        childGameObject.transform.parent = gameObject.transform;
        childGameObject.transform.position = position;
        if (sprite != null)
        {
            SpriteRenderer spriteRenderer = childGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
        }
        return childGameObject;
    }

}