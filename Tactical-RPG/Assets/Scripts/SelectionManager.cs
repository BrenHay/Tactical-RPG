using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    Transform selectedUnit;
    bool unitSelected = false;
    GridManager gridManager;
    PathFinder pathFinder;
    List<GameObject> canMoveTo = new List<GameObject>();

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = FindObjectOfType<PathFinder>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if(hasHit)
            {
                if(hit.transform.tag == "Tile")
                {
                    if(unitSelected)
                    {
                        if(canMoveTo.Contains(hit.transform.gameObject))
                        {
                            Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);
                            Vector3 startCords = new Vector3((int)selectedUnit.position.x, (int)selectedUnit.position.y) / gridManager.UnityGridSize;

                            selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.z);

                            ResetTiles();
                            unitSelected = false;
                            selectedUnit = null;
                        }                          
                    }
                }

                if(hit.transform.tag == "Unit")
                {
                    if(unitSelected)
                    {
                        ResetTiles();
                    }
                    selectedUnit = hit.transform;
                    unitSelected = true;

                    canMoveTo = pathFinder.FindWalkablePaths(hit.transform.gameObject);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            unitSelected = false;
            selectedUnit = null;
            canMoveTo = new List<GameObject>();
        }
    }

    void ResetTiles()
    {
        foreach (GameObject g in canMoveTo)
        {
            g.GetComponent<ShowCursor>().highlight = false;
        }
    }
}
