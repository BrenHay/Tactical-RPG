using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public GridManager gridManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> FindWalkablePaths(GameObject unit)
    {
        gridManager = FindObjectOfType<GridManager>();

        int tempMov = unit.GetComponent<Unit>().stats.Mov;
        Vector2Int unitTransform = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));
        GameObject currentTile = gridManager.GetTile(unitTransform);
        currentTile.GetComponent<ShowCursor>().highlight = true;
        // Search Nodes

        return search(unitTransform, tempMov);
    }

    public List<GameObject> search(Vector2Int searchingFrom, int currentMov)
    {
        List<GameObject> tiles = new List<GameObject>();
        GameObject tile = gridManager.GetTile(searchingFrom);
        if(tile)
        {
            if (tile.GetComponent<ShowCursor>().tileInfo.node.type == "Wall")
            {
                return tiles;
            }
            if(tile.GetComponent<ShowCursor>().unitOnTile)
            {
                if(tile.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                {
                    return tiles;
                }
            }
            
            if(currentMov >= 0)
            {
                tiles.Add(tile);
                tile.GetComponent<ShowCursor>().highlight = true;
            }
                
        }
        else
        {
            return tiles;
        }
        
        if (currentMov > 0)
        {
            // Search Up

            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y + 1));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search Down
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y - 1));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search left
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x - 1, searchingFrom.y));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search Right
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x + 1, searchingFrom.y));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
        }
        return tiles;
    }

    public List<GameObject> FindBattleTiles(GameObject unit)
    {
        gridManager = FindObjectOfType<GridManager>();
        List<GameObject> tiles = new List<GameObject>();

        int range = unit.GetComponent<Unit>().stats.Range;
        Vector2Int unitTransform = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));
        GameObject currentTile = gridManager.GetTile(unitTransform);
        currentTile.GetComponent<ShowCursor>().highlight = true;
        if(range == 1)
        {
            GameObject tile;
            tile = gridManager.GetTile(new Vector2Int(unitTransform.x, unitTransform.y + 1));
            if (tile)
            {
                tiles.Add(tile);
            }
            tile = gridManager.GetTile(new Vector2Int(unitTransform.x, unitTransform.y - 1));
            if (tile)
            {
                tiles.Add(tile);
            }
            tile = gridManager.GetTile(new Vector2Int(unitTransform.x + 1, unitTransform.y));
            if (tile)
            {
                tiles.Add(tile);
            }
            tile = gridManager.GetTile(new Vector2Int(unitTransform.x - 1, unitTransform.y));
            if (tile)
            {
                tiles.Add(tile);
            }
        }

        foreach(GameObject g in tiles)
        {  
            g.GetComponent<ShowCursor>().indicate = true;
            
        }

        return tiles;
    }
}
