using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public Tilemap tilemap;
    public TileBase wall;
    GameObject go;

    // Use this for initialization
    void Start ()
    {
        go = new GameObject();
        Tile t = ScriptableObject.CreateInstance<Tile>();
        t.gameObject = go;
        t.color = Color.red;
        tilemap.SetTile(new Vector3Int(0, 0, 0), t);
        tilemap.SetTile(new Vector3Int(0, 2, 0), t);
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
