using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RoomController : MonoBehaviour {
    public GameObject RoomConnectionPrefab;
    public GameObject[] EnemyPrefabs;
    public Sprite WallSprite;
    public Sprite WallWithNothingBelowSprite;
    public Sprite GroundSprite;
    public Room room;
    private GameObject wallsContainer;
    private GameObject roomConnectionsContainer;
    private GameObject groundContainer;
    private GameObject enemiesContainer;

    void Awake() {
        wallsContainer = createChildGameObject("Walls");
        roomConnectionsContainer = createChildGameObject("RoomConnections");
        groundContainer = createChildGameObject("Ground");
        enemiesContainer = createChildGameObject("Enemies");
    }

    public void SetupRoom(Room room) {
        this.room = room;
        if (room.notAccessible) {
            return;
        }

        // Put the wall gameobjects in the right positions
        CreateTiles();
        CreateRoomConnectionBoxColliders();
    }

    private void CreateRoomConnectionBoxColliders() {
        foreach (RoomConnection roomConnection in room.roomConnections) {
            Vector3Int offset = room.PositionOfRoomConnection(roomConnection);
            GameObject roomConnectionObject = Instantiate(RoomConnectionPrefab, offset, Quaternion.identity, roomConnectionsContainer.transform);
            roomConnectionObject.GetComponent<RoomConnectionController>().SetRoomConnection(roomConnection, room.stairsFacingLeft);
        }
    }

    private void CreateTiles() {
        for (int x = 0; x < room.width; x++) {
            for (int y = 0; y < room.length; y++) {
                PositionTile(room.grid[x, y].type, new Vector2Int(x, y));
            }
        }
    }

    private void PositionTile(TileType type, Vector2Int position2D) {
        Vector3Int position = new Vector3Int(position2D.x, position2D.y, 0);
        switch (type) {
            case TileType.Enemy:
                int enemyIndex = Random.Range(0, EnemyPrefabs.Length);
                Instantiate(EnemyPrefabs[enemyIndex], position, Quaternion.identity, enemiesContainer.transform);
                createChildGameObject(type.ToString(), position, groundContainer.transform, GroundSprite);
                break;
            case TileType.Ground:
                createChildGameObject(type.ToString(), position, groundContainer.transform, GroundSprite);
                break;
            case TileType.Wall:
                Sprite useThisSprite;
                if (position.y == 0) {
                    useThisSprite = WallWithNothingBelowSprite;
                } else {
                    TileType tileBelowType = room.grid[position.x, position.y - 1].type;
                    if (tileBelowType == TileType.Wall || tileBelowType == TileType.Upstairs) {
                        useThisSprite = WallSprite;
                    } else {
                        useThisSprite = WallWithNothingBelowSprite;
                    }
                }
                GameObject wall = createChildGameObject(type.ToString(), position, wallsContainer.transform, useThisSprite);
                BoxCollider2D boxCollider = wall.AddComponent<BoxCollider2D>();
                boxCollider.usedByComposite = true;
                break;
        }
    }

    private GameObject createChildGameObject(string childObjectName, Vector3Int? position = null, Transform parent = null, Sprite sprite = null) {
        GameObject childGameObject = new GameObject(childObjectName);
        childGameObject.transform.parent = parent == null ? gameObject.transform : parent;
        childGameObject.transform.position = position.HasValue ? position.Value : Vector3Int.zero;
        if (sprite != null) {
            SpriteRenderer spriteRenderer = childGameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            switch (childObjectName) {
                case "Ground":
                    spriteRenderer.sortingLayerName = "Ground";
                    break;
                case "Wall":
                    spriteRenderer.sortingLayerName = "Walls";
                    break;
            }
        }
        return childGameObject;
    }

}