using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    Transform selectedUnit;
    GameObject tileWithUnit;
    GameObject selectedTile;
    bool unitSelected = false;
    bool selectOpponent = false;
    bool menuOpen = false;
    bool selectFoe = false;

    GridManager gridManager;
    PathFinder pathFinder;
    BattleManager battleManager;

    List<GameObject> canMoveTo = new List<GameObject>();
    List<GameObject> canBattle = new List<GameObject>();

    [SerializeField] private GameObject actionMenu;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = FindObjectOfType<PathFinder>();
        battleManager = FindObjectOfType<BattleManager>();
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
                if(hit.transform.tag == "Tile" && !menuOpen)
                {
                    ShowCursor showCursor = hit.transform.gameObject.GetComponent<ShowCursor>();
                    selectedTile = hit.transform.gameObject;

                    if (unitSelected && !showCursor.unitOnTile || unitSelected && showCursor.unitOnTile == selectedUnit.gameObject)
                    {

                        if (canMoveTo.Contains(hit.transform.gameObject))
                        {
                            Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);
                            //Vector3 startCords = new Vector3((int)selectedUnit.position.x, (int)selectedUnit.position.y) / gridManager.UnityGridSize;

                            selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.z);


                            OpenMenu();
                        }
                    }

                    if (showCursor.unitOnTile && !unitSelected)
                    {
                        if(showCursor.unitOnTile.tag == "Unit" && showCursor.unitOnTile.GetComponent<Unit>().canMove)
                        {
                            selectedUnit = showCursor.unitOnTile.transform;
                            tileWithUnit = hit.transform.gameObject;
                            unitSelected = true;

                            canMoveTo = pathFinder.FindWalkablePaths(selectedUnit.gameObject);
                        }
                    }
                    
                }
                if(selectFoe && canBattle.Contains(hit.transform.gameObject))
                {
                    if(hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
                    {
                        if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                        {
                            battleManager.Battle(selectedUnit.gameObject, hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile);
                            CloseMenu();
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            selectedUnit.transform.position = new Vector3(tileWithUnit.transform.position.x, selectedUnit.transform.position.y, tileWithUnit.transform.position.z);
            unitSelected = false;
            selectedUnit = null;
            canMoveTo = new List<GameObject>();
            CloseMenu();

        }
    }

    void ResetTiles()
    {
        foreach (GameObject g in canMoveTo)
        {
            g.GetComponent<ShowCursor>().highlight = false;
        }
        foreach(GameObject g in canBattle)
        {
            g.GetComponent<ShowCursor>().indicate = false;
        }
    }

    void OpenMenu()
    {
        menuOpen = true;
        ResetTiles();
        canBattle = pathFinder.FindBattleTiles(selectedUnit.gameObject);
        actionMenu.SetActive(true);
    }

    void CloseMenu()
    {
        menuOpen = false;
        actionMenu.SetActive(false);
        ResetTiles();
    }

    public void battle()
    {
        selectFoe = true;
    }

    public void wait()
    {
        tileWithUnit.GetComponent<ShowCursor>().unitOnTile = null;
        ResetTiles();
        selectedTile.GetComponent<ShowCursor>().unitOnTile = selectedUnit.gameObject;
        selectedUnit.GetComponent<Unit>().canMove = false;
        unitSelected = false;
        selectedUnit = null;
        tileWithUnit = null;
        CloseMenu();
    }
}
