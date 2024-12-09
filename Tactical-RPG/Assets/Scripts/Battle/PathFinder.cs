using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public GridManager gridManager;

    bool found = false;
    
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

        return search(unitTransform, tempMov, unit.GetComponent<Unit>().stats.Range);
    }

    public List<GameObject> search(Vector2Int searchingFrom, int currentMov, int range)
    {
        List<GameObject> tiles = new List<GameObject>();
        GameObject tile = gridManager.GetTile(searchingFrom);
        if(tile)
        {
            if (tile.GetComponent<ShowCursor>().tileInfo.node.type == "Wall")
            {
                SearchBattle(new Vector2Int(searchingFrom.x, searchingFrom.y), range);
                
                return tiles;
            }
            if(tile.GetComponent<ShowCursor>().unitOnTile)
            {
                if(tile.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                {
                    SearchBattle(new Vector2Int(searchingFrom.x, searchingFrom.y), range);
                    return tiles;
                }
            }
            
            if(currentMov >= 0)
            {
                tiles.Add(tile);
                tile.GetComponent<ShowCursor>().highlight = true;
                tile.GetComponent<ShowCursor>().searched = true;
                tile.GetComponent<ShowCursor>().indicate = false;
            }   
        }
        else
        {
            return tiles;
        }
        
        if (currentMov >= 0)
        {
            // Search Up
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y + 1));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search Down
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y - 1));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search left
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x - 1, searchingFrom.y));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search Right
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x + 1, searchingFrom.y));
            if(tile)
            {
                tiles.AddRange(search(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
        }
        if(currentMov < 0)
        {
            SearchBattle(new Vector2Int(searchingFrom.x, searchingFrom.y), range);
        }
        return tiles;
    }

    void SearchBattle(Vector2Int searchingFrom, int range)
    {
        GameObject tile = gridManager.GetTile(searchingFrom);
        if(tile)
        {
            if (tile.GetComponent<ShowCursor>().highlight)
                return;

            if (range > 0)
            {
                SearchBattle(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), range - 1);
                SearchBattle(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), range - 1);
                SearchBattle(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), range - 1);
                SearchBattle(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), range - 1);
            }
            else
                return;

            tile.GetComponent<ShowCursor>().indicate = true;
            tile.GetComponent<ShowCursor>().searched = true;
        }
    }

    public List<GameObject> FindBattleTiles(GameObject unit)
    {
        gridManager = FindObjectOfType<GridManager>();
        List<GameObject> tiles = new List<GameObject>();
        int range = unit.GetComponent<Unit>().stats.Range;
        Vector2Int unitTransform = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));
        GameObject currentTile = gridManager.GetTile(unitTransform);
        currentTile.GetComponent<ShowCursor>().highlight = true;


        tiles = GetBattleTiles(unit, unitTransform, range);

        foreach(GameObject g in tiles)
        {  
            g.GetComponent<ShowCursor>().indicate = true;
            
        }

        return tiles;
    }

    public List<GameObject> GetBattleTiles(GameObject unit, Vector2Int searchingFrom, int currentRange)
    {
        GameObject tile = gridManager.GetTile(searchingFrom);
        List<GameObject> tiles = new List<GameObject>();
        Vector2Int unitTransform = unit.GetComponent<Unit>().GetPostion();

        if(tile && currentRange >= 0)
        {
            if(unit.GetComponent<Unit>().attackType == "ranged")
            {
                if(Mathf.CeilToInt(Vector2Int.Distance(unitTransform, searchingFrom)) >= 2 && !tile.GetComponent<ShowCursor>().searched)
                {
                    tiles.Add(tile);
                    tile.GetComponent<ShowCursor>().searched = true;
                }
            }
            else if(Mathf.CeilToInt(Vector2Int.Distance(unitTransform, searchingFrom)) > 0 && !tile.GetComponent<ShowCursor>().searched)
            {
                tiles.Add(tile);
                tile.GetComponent<ShowCursor>().searched = true;
            }
        }
        else
        {
            return tiles;
        }

        if(currentRange > 0)
        {
            // Search up
            tiles.AddRange(GetBattleTiles(unit, new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentRange - 1));
            // Search Down
            tiles.AddRange(GetBattleTiles(unit, new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentRange - 1));
            // Search Left
            tiles.AddRange(GetBattleTiles(unit, new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentRange - 1));
            // Search Right
            tiles.AddRange(GetBattleTiles(unit, new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentRange - 1));
        }
        
        return tiles;
    }

    public List<GameObject> FindEnemyRange(GameObject unit)
    {
        gridManager = FindObjectOfType<GridManager>();

        int tempMov = unit.GetComponent<Unit>().stats.Mov;
        Vector2Int unitTransform = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));

        // Search Nodes
        return SearchEnemyRange(unitTransform, tempMov, unit.GetComponent<Unit>().stats.Range);
    }

    public List<GameObject> SearchEnemyRange(Vector2Int searchingFrom, int currentMov, int range)
    {
        List<GameObject> tiles = new List<GameObject>();
        GameObject tile = gridManager.GetTile(searchingFrom);
        
        if (tile)
        {
            if (tile.GetComponent<ShowCursor>().tileInfo.node.type == "Wall")
            {
                tiles.AddRange(SearchEnemyBattle(searchingFrom, range));
                return tiles;
            }
            if (tile.GetComponent<ShowCursor>().unitOnTile)
            {
                if (tile.GetComponent<ShowCursor>().unitOnTile.tag == "Unit")
                {
                    tiles.AddRange(SearchEnemyBattle(searchingFrom, range));
                    return tiles;
                }
            }

            if (currentMov >= 0)
            {
                if(!tile.GetComponent<ShowCursor>().searched)
                    tiles.Add(tile);
                tile.GetComponent<ShowCursor>().searched = true;
            }
        }
        else
        {
            return tiles;
        }

        if (currentMov >= 0)
        {
            // Search Up
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y + 1));
            if (tile)
            {
                tiles.AddRange(SearchEnemyRange(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search Down
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y - 1));
            if (tile)
            {
                tiles.AddRange(SearchEnemyRange(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search left
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x - 1, searchingFrom.y));
            if (tile)
            {
                tiles.AddRange(SearchEnemyRange(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
            // Search Right
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x + 1, searchingFrom.y));
            if (tile)
            {
                tiles.AddRange(SearchEnemyRange(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost, range));
            }
        }

        if (currentMov < 0)
        {
            tiles.AddRange(SearchEnemyBattle(new Vector2Int(searchingFrom.x, searchingFrom.y), range));
        }
        return tiles;
    }

    List<GameObject> SearchEnemyBattle(Vector2Int searchingFrom, int range)
    {
        GameObject tile = gridManager.GetTile(searchingFrom);
        List<GameObject> tiles = new List<GameObject>();
        if (tile)
        {
            if (range > 0)
            {
                if(!tile.GetComponent<ShowCursor>().searched)
                {
                    tiles.Add(tile);
                }
                
                tiles.AddRange(SearchEnemyBattle(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), range - 1));
                tiles.AddRange(SearchEnemyBattle(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), range - 1));
                tiles.AddRange(SearchEnemyBattle(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), range - 1));
                tiles.AddRange(SearchEnemyBattle(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), range - 1));

                tile.GetComponent<ShowCursor>().searched = true;
            }
            else
                return tiles;          

        }
        return tiles;
    }

    public List<GameObject> FindWalkableTiles(GameObject unit)
    {
        gridManager = FindObjectOfType<GridManager>();

        int tempMov = unit.GetComponent<Unit>().stats.Mov;
        Vector2Int unitTransform = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));

        // Search Nodes
        return GetWalkableTiles(unitTransform, tempMov);
    }

    public List<GameObject> GetWalkableTiles(Vector2Int searchingFrom, int currentMov)
    {
        List<GameObject> tiles = new List<GameObject>();
        GameObject tile = gridManager.GetTile(searchingFrom);

        if (tile)
        {
            if (tile.GetComponent<ShowCursor>().tileInfo.node.type == "Wall")
            {
                return tiles;
            }
            if (tile.GetComponent<ShowCursor>().unitOnTile)
            {
                if (tile.GetComponent<ShowCursor>().unitOnTile.tag == "Unit")
                {
                    return tiles;
                }
            }

            if (currentMov >= 0)
            {
                if (!tile.GetComponent<ShowCursor>().searched)
                    tiles.Add(tile);
                tile.GetComponent<ShowCursor>().searched = true;
            }
        }
        else
        {
            return tiles;
        }

        if (currentMov >= 0)
        {
            // Search Up
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y + 1));
            if (tile)
            {
                tiles.AddRange(GetWalkableTiles(new Vector2Int(searchingFrom.x, searchingFrom.y + 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search Down
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x, searchingFrom.y - 1));
            if (tile)
            {
                tiles.AddRange(GetWalkableTiles(new Vector2Int(searchingFrom.x, searchingFrom.y - 1), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search left
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x - 1, searchingFrom.y));
            if (tile)
            {
                tiles.AddRange(GetWalkableTiles(new Vector2Int(searchingFrom.x - 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
            // Search Right
            tile = gridManager.GetTile(new Vector2Int(searchingFrom.x + 1, searchingFrom.y));
            if (tile)
            {
                tiles.AddRange(GetWalkableTiles(new Vector2Int(searchingFrom.x + 1, searchingFrom.y), currentMov - tile.GetComponent<ShowCursor>().tileInfo.node.moveCost));
            }
        }
        return tiles;
    }

    public List<GameObject> PathToClosestEnemy(GameObject unit)
    {
        Vector2Int unitPos = new Vector2Int((int)(unit.transform.position.x + 0.5f), (int)(unit.transform.position.z + 0.5f));
        List<GameObject> path = FindPath(unitPos);
        return path;
    }

    List<GameObject> FindPath(Vector2Int start)
    {
        Queue<(Vector2Int position, List<GameObject> path)> queue = new Queue<(Vector2Int, List<GameObject>)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Enqueue the starting position with an empty path
        queue.Enqueue((start, new List<GameObject>()));

        while (queue.Count > 0)
        {
            var (currentPosition, currentPath) = queue.Dequeue();

            // Get the current tile
            GameObject tile = gridManager.GetTile(currentPosition);

            if (tile == null) continue;

            // If already visited, skip
            if (visited.Contains(currentPosition)) continue;
            visited.Add(currentPosition);

            // Add the current tile to the path
            currentPath.Add(tile);

            ShowCursor tileInfo = tile.GetComponent<ShowCursor>();

            // Check if it's a wall
            if (tileInfo.tileInfo.node.type == "Wall") continue;

            // Check if there's an enemy unit
            if (tileInfo.unitOnTile != null && tileInfo.unitOnTile.tag == "Unit")
            {
                return currentPath; // Return the shortest path as soon as we find an enemy
            }

            // Enqueue neighbors
            Vector2Int[] neighbors = {
            new Vector2Int(currentPosition.x, currentPosition.y + 1), // Up
            new Vector2Int(currentPosition.x, currentPosition.y - 1), // Down
            new Vector2Int(currentPosition.x - 1, currentPosition.y), // Left
            new Vector2Int(currentPosition.x + 1, currentPosition.y)  // Right
            };

            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    // Clone the current path and enqueue
                    List<GameObject> newPath = new List<GameObject>(currentPath);
                    queue.Enqueue((neighbor, newPath));
                }
            }
        }

        return new List<GameObject>(); // Return an empty list if no path is found
    }

}
