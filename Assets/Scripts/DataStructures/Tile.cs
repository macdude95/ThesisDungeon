using System.Collections;
using System.Collections.Generic;
using SettlersEngine;

public enum TileType { Ground, Wall, Upstairs, Downstairs, Enemy };

public class Tile : IPathNode<System.Object> {
    public TileType type;

    public Tile(TileType type) {
        this.type = type;
    }

    public bool IsWalkable(System.Object unused) {
        return type != TileType.Wall;
    }
}