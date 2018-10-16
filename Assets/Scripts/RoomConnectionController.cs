using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConnectionController : MonoBehaviour {

    public Sprite upstairsSprite;
    public Sprite downstairsSprite;
    private RoomConnection roomConnection;

    public void SetRoomConnection(RoomConnection roomConnection)
    {
        this.roomConnection = roomConnection;
        switch(roomConnection)
        {
            case RoomConnection.Up:
            case RoomConnection.Down:
                SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = roomConnection == RoomConnection.Up ? upstairsSprite : downstairsSprite; 
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject.FindWithTag("GameController").GetComponent<GameController>().enterRoomConnection(roomConnection);
        }
    }
}
