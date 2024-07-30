using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private GameObject cursorHighlight;
    private GameObject selectionHighlight;
    public GameObject selectionManager;
    public GameObject unitOnTile;
    public bool selected = false;
    
    // Start is called before the first frame update
    void Start()
    {
        cursorHighlight = gameObject.transform.GetChild(0).gameObject;
        selectionHighlight = gameObject.transform.GetChild(1).gameObject;
        selectionManager = GameObject.FindGameObjectWithTag("SelectionManager");
        if(unitOnTile)
        {
            unitOnTile.transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y + 1, transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(selected)
        {
            selectionHighlight.active = true;
            cursorHighlight.active = false;

        }else
        {
            selectionHighlight.active = false;
        }
    }

    public void placeUnit(GameObject unit)
    {
        unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        unitOnTile = unit;
        selected = false;
    }

    private void OnMouseEnter()
    {
        if(!selected)
        {
            cursorHighlight.active = true;
        }
    }

    private void OnMouseExit()
    {
       cursorHighlight.active = false;
    }

    private void OnMouseDown()
    {
        selectionManager.GetComponent<SelectionManager>().selectTile(gameObject);
    }
}
