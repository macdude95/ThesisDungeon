using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEntranceController : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<GameController>().loadNewLevel();
        }
    }
}
