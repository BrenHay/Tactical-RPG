using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Vector2Int gridSize;
    [SerializeField] int unityGridSize;
    public int UnityGridSize { get { return unityGridSize; } }

    Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();
    Dictionary<Vector2Int, GameObject> Grid { get { return grid; } }

    private void Awake()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject g in tiles)
        {
            Vector2Int cords = new Vector2Int((int)(g.transform.position.x + 0.5f), (int)(g.transform.position.z + 0.5));
            grid.Add(cords, g);
        }

        

        //for (int x = 0; x < gridSize.x; x++)
        //{
        //    for(int y = 0; y < gridSize.y; y++)
        //    {
        //        Vector2Int cords = new Vector2Int(x, y);
        //        grid.Add(cords, new Tile(cords));

        //        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        //Vector3 position = new Vector3(cords.x * UnityGridSize, 0f, cords.y * UnityGridSize);
        //        //cube.transform.position = position;
        //        //cube.transform.SetParent(transform);
        //    }
        //}
    }

    public void UpdateDictionary()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject g in tiles)
        {
            Vector2Int cords = new Vector2Int((int)(g.transform.position.x + 0.5f), (int)(g.transform.position.z + 0.5));
            if (!GetTile(cords))
            {
                grid.Add(cords, g);
            }
        }
    }

    public GameObject GetTile(Vector2Int cords)
    {
        if(grid.ContainsKey(cords))
        {
            return grid[cords];
        }

        return null;
    }

    public void ResetTileSearchStatus()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject g in tiles)
        {
            g.GetComponent<ShowCursor>().searched = false;
        }
    }
}
