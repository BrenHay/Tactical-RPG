using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tile
{
    public Vector2Int cords;
    public Node node;
    public bool walkable;
    public bool explored;
    public bool path;
    public Tile connectTo;
    public Tile(Vector2Int cords)
    {
        this.cords = cords;
        if (node.type == "wall")
        {
            walkable = false;
        }
        else
        {
            walkable = true;
        }
    }
}
