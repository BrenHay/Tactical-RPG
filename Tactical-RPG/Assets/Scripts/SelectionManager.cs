using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    Transform selectedUnit;
    GameObject tileWithUnit;
    bool unitSelected = false;
    bool selectOpponent = false;
    GridManager gridManager;
    PathFinder pathFinder;
    List<GameObject> canMoveTo = new List<GameObject>();
    List<GameObject> canBattle = new List<GameObject>();

    [SerializeField] private GameObject actionMenu;

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
                    ShowCursor showCursor = hit.transform.gameObject.GetComponent<ShowCursor>();
                   
                    if(showCursor.unitOnTile && !unitSelected)
                    {
                        if(showCursor.unitOnTile.tag == "Unit")
                        {
                            selectedUnit = showCursor.unitOnTile.transform;
                            tileWithUnit = hit.transform.gameObject;
                            unitSelected = true;

                            canMoveTo = pathFinder.FindWalkablePaths(selectedUnit.gameObject);
                        }
                    }
                    if (unitSelected && !showCursor.unitOnTile)
                    {
                        if (canMoveTo.Contains(hit.transform.gameObject) && !hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
                        {
                            Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);
                            Vector3 startCords = new Vector3((int)selectedUnit.position.x, (int)selectedUnit.position.y) / gridManager.UnityGridSize;

                            selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.z);

                            showCursor.unitOnTile = selectedUnit.gameObject;
                            OpenMenu();
                            //ResetTiles();
                            //tileWithUnit.GetComponent<ShowCursor>().unitOnTile = null;
                            //unitSelected = false;
                            //selectedUnit = null;
                            //tileWithUnit = null;
                        }
                    }
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

    void OpenMenu()
    {
        canBattle = pathFinder.FindBattleTiles(selectedUnit.gameObject);
        actionMenu.SetActive(true);
        ResetTiles();
    }
}
