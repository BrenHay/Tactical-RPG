using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public GameObject selectedTile;
    public GameObject selectedUnit;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectTile(GameObject tile)
    {
       if(selectedTile)
       {
          selectedTile.GetComponent<Tile>().selected = false;
       }
       tile.GetComponent<Tile>().selected = true;
        if (!selectedUnit && tile.GetComponent<Tile>().unitOnTile)
        {
            selectedUnit = tile.GetComponent<Tile>().unitOnTile;
        }
        else if(selectedUnit && !tile.GetComponent<Tile>().unitOnTile)
        {
            selectedTile.GetComponent<Tile>().unitOnTile = null;
            tile.GetComponent<Tile>().placeUnit(selectedUnit);
            tile.GetComponent<Tile>().unitOnTile = selectedUnit;
            selectedUnit = null;
            selectedTile = null;
        }
        selectedTile = tile;
    }
}
