using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnectionController : MonoBehaviour {

    public Sprite upstairsSprite;
    public Sprite downstairsSprite;
    public Sprite levelExitSprite;
    private RoomConnection roomConnection;
    private SpriteRenderer spriteRenderer;

    public void SetRoomConnection(RoomConnection roomConnection, bool? stairsFacingLeft)
    {
        this.roomConnection = roomConnection;
        switch(roomConnection)
        {
            case RoomConnection.Up:
            case RoomConnection.Down:
            case RoomConnection.NextLevel:
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Items";
                break;
        }
        switch(roomConnection)
        {
            case RoomConnection.Up:
                spriteRenderer.sprite = upstairsSprite;
                spriteRenderer.flipX = !stairsFacingLeft.Value;
                break;
            case RoomConnection.Down:
                spriteRenderer.sprite = downstairsSprite;
                spriteRenderer.flipX = stairsFacingLeft.Value;
                break;
            case RoomConnection.NextLevel:
                spriteRenderer.sprite = levelExitSprite;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<GameController>().levelController.enterRoomConnection(roomConnection);
        }
    }
}
