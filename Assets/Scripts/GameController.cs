using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public WallsController wallsController;

    private void Awake()
    {
        wallsController.SetUpWalls();
    }
}
