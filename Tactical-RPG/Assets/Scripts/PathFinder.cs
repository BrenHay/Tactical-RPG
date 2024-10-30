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
            tile.GetComponent<ShowCursor>().highlight = true;
            tiles.Add(tile);
        }
        else
        {
            return tiles;
        }
        
        if (currentMov > 0)
        {
            // Search Up
            
            tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentMov - 1));
            // Search Down
            tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentMov - 1));
            // Search left
            tiles.AddRange(search(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentMov - 1));
            // Search Right
            tiles.AddRange(search(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentMov - 1));
        }
        return tiles;
    }
}
