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
    CursorController cursor;
    TurnManager turnManager;

    List<GameObject> canMoveTo = new List<GameObject>();
    List<GameObject> canBattle = new List<GameObject>();

    List<GameObject> enemiesInRange = new List<GameObject>();
    int selectionIndex = 0;

    [SerializeField] private GameObject actionMenu;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathFinder = FindObjectOfType<PathFinder>();
        battleManager = FindObjectOfType<BattleManager>();
        cursor = FindObjectOfType<CursorController>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            selectedUnit.transform.position = new Vector3(tileWithUnit.transform.position.x, selectedUnit.transform.position.y, tileWithUnit.transform.position.z);
            unitSelected = false;
            selectedUnit = null;
            selectFoe = false;
            canMoveTo = new List<GameObject>();
            cursor.lockMovement = false;
            CloseMenu();
        }

        if(Input.GetKeyDown(KeyCode.A) && selectFoe)
        {
            --selectionIndex;
            if (selectionIndex < 0) selectionIndex = enemiesInRange.Count - 1;
            else selectionIndex %= enemiesInRange.Count;
            SelectEnemy();
        }
        if (Input.GetKeyDown(KeyCode.D) && selectFoe)
        {
            ++selectionIndex;
            if (selectionIndex < 0) selectionIndex = enemiesInRange.Count - 1;
            else selectionIndex %= enemiesInRange.Count;
            SelectEnemy();
        }
    }

    public void SelectTile()
    {
        Ray ray = new Ray(cursor.cursorPoint.position, Vector3.down);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit)
        {
            
            // Checks for a player when selecting a tile
            //
            if (hit.transform.tag == "Tile" && !menuOpen)
            {
                ShowCursor showCursor = hit.transform.gameObject.GetComponent<ShowCursor>();
                selectedTile = hit.transform.gameObject;

                // Moves the unit
                //
                if (unitSelected && !showCursor.unitOnTile || unitSelected && showCursor.unitOnTile == selectedUnit.gameObject)
                {
                    if (canMoveTo.Contains(hit.transform.gameObject))
                    {
                        Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);

                        selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.z);
                        cursor.lockMovement = true;

                        OpenMenu();
                        return;
                    }
                }

                // Selects the unit
                //
                if (showCursor.unitOnTile && !unitSelected)
                {
                    if (showCursor.unitOnTile.tag == "Unit" && showCursor.unitOnTile.GetComponent<Unit>().canMove)
                    {
                        selectedUnit = showCursor.unitOnTile.transform;
                        tileWithUnit = hit.transform.gameObject;
                        unitSelected = true;

                        canMoveTo = pathFinder.FindWalkablePaths(selectedUnit.gameObject);
                        return;
                    }
                }

            }

            // Does battle with enemy
            //
            if (selectFoe && canBattle.Contains(hit.transform.gameObject))
            {
                if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
                {
                    if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                    {
                        battleManager.Battle(selectedUnit.gameObject, hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile);
                        selectFoe = false;
                        ResetState();
                        turnManager.CheckEndOfTurn();
                        FindObjectOfType<BattleForecast>().CloseForecast();
                        cursor.lockMovement = false;
                        return;
                    }
                }
            }

            // Toggles enemies personal dangerzone
            if(hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
            {
                if(hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.GetComponent<EnemyAI>().HighlightUnitDangerTiles();
                    return;
                }
            }
        }
    }

    void ResetTiles()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        foreach(GameObject g in tiles)
        {
            ShowCursor tile = g.GetComponent<ShowCursor>();
            g.GetComponent<ShowCursor>().highlight = false;
            g.GetComponent<ShowCursor>().searched = false;
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

    public void OpenBattleForecast(GameObject foe)
    {
        if(selectFoe)
        {
            (int aHp, int oHp, int aDmg, int oDmg, int aHit, int oHit) = battleManager.ForecastDamage(selectedUnit.gameObject, foe);
            FindObjectOfType<BattleForecast>().OpenForecast(aHp, oHp, aDmg, oDmg, aHit, oHit);
        }
        else
        {
            FindObjectOfType<BattleForecast>().CloseForecast();
        }
    }

    public void battle()
    {
        selectFoe = true;
        actionMenu.SetActive(false);
        foreach(GameObject g in canBattle)
        {
            if (g.GetComponent<ShowCursor>().unitOnTile && g.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
            {
                GameObject enemy = g.GetComponent<ShowCursor>().unitOnTile;
                enemiesInRange.Add(g);
            }
        }
        cursor.transform.position = new Vector3(enemiesInRange[0].transform.position.x, cursor.gameObject.transform.position.y, enemiesInRange[0].transform.position.z);
    }

    void SelectEnemy()
    {
        cursor.transform.position = new Vector3(enemiesInRange[selectionIndex].transform.position.x, cursor.gameObject.transform.position.y, enemiesInRange[selectionIndex].transform.position.z);
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
        turnManager.CheckEndOfTurn();
        turnManager.UpdateEnemyRange();
        cursor.lockMovement = false;
    }

    private void ResetState()
    {
        tileWithUnit.GetComponent<ShowCursor>().unitOnTile = null;
        ResetTiles();
        selectedTile.GetComponent<ShowCursor>().unitOnTile = selectedUnit.gameObject;
        selectedUnit.GetComponent<Unit>().canMove = false;
        unitSelected = false;
        selectedUnit = null;
        tileWithUnit = null;
        enemiesInRange = new List<GameObject>();
        selectionIndex = 0;
        CloseMenu();
    }
}
