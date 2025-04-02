using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectionManager : MonoBehaviour
{
    enum selectionState { selectUnit, unitSelected, menuOpen, selectFoe };

    selectionState currentState = selectionState.selectUnit;

    Transform selectedUnit;
    GameObject tileWithUnit;
    GameObject selectedTile;
    bool unitSelected = false;

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
        
        if (Input.GetMouseButtonDown(1)) // Go back one state
        {
            goBackState();
        }

        if(Input.GetKeyDown(KeyCode.A) && currentState == selectionState.selectFoe) // Select other foes by pressing A or D
        {
            --selectionIndex;
            if (selectionIndex < 0) selectionIndex = enemiesInRange.Count - 1;
            else selectionIndex %= enemiesInRange.Count;
            SelectEnemy();
        }
        if (Input.GetKeyDown(KeyCode.D) && currentState == selectionState.selectFoe)
        {
            ++selectionIndex;
            if (selectionIndex < 0) selectionIndex = enemiesInRange.Count - 1;
            else selectionIndex %= enemiesInRange.Count;
            SelectEnemy();
        }
    }

    // Manages the current state of the Selection Manager
    private void goBackState()
    {
        switch(currentState)
        {
            case selectionState.unitSelected:
                selectedUnit.GetComponent<Unit>().resetIndicators();
                selectedUnit = null;
                canMoveTo = new List<GameObject>();
                cursor.lockMovement = false;
                canBattle = new List<GameObject>();
                ResetTiles();
                currentState = selectionState.selectUnit;
                break;

            case selectionState.menuOpen:
                selectedUnit.transform.position =
                selectedUnit.transform.position = new Vector3(tileWithUnit.transform.position.x, selectedUnit.transform.position.y, tileWithUnit.transform.position.z);
                CloseMenu();
                selectedUnit.GetComponent<Unit>().SetTilesActive();
                cursor.lockMovement = false;
                currentState = selectionState.unitSelected;
                break;

            case selectionState.selectFoe:
                cursor.transform.position = new Vector3(selectedUnit.transform.position.x, cursor.transform.position.y, selectedUnit.transform.position.z);
                OpenMenu();
                enemiesInRange.Clear();
                selectionIndex = 0;
                currentState = selectionState.menuOpen;
                Debug.Log("Select foe");
                break;
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
            if (hit.transform.tag == "Tile")
            {
                ShowCursor showCursor = hit.transform.gameObject.GetComponent<ShowCursor>();
                selectedTile = hit.transform.gameObject;

                switch (currentState)
                {
                    case selectionState.selectUnit:
                        if (showCursor.unitOnTile)
                        {
                            if (showCursor.unitOnTile.tag == "Unit" && showCursor.unitOnTile.GetComponent<Unit>().canMove)
                            {
                                selectedUnit = showCursor.unitOnTile.transform;
                                tileWithUnit = hit.transform.gameObject;

                                canMoveTo = pathFinder.FindWalkablePaths(selectedUnit.gameObject);
                                selectedUnit.GetComponent<Unit>().spawnTiles(canMoveTo);
                                currentState = selectionState.unitSelected;
                            }
                        }
                        break;

                    case selectionState.unitSelected:
                        if (!showCursor.unitOnTile || showCursor.unitOnTile == selectedUnit.gameObject)
                        {
                            if (canMoveTo.Contains(hit.transform.gameObject))
                            {
                                Vector3 targetCords = new Vector3(hit.transform.position.x, 0.60f, hit.transform.position.z);

                                selectedUnit.transform.position = new Vector3(targetCords.x, selectedUnit.position.y, targetCords.z);
                                cursor.lockMovement = true;

                                selectedUnit.GetComponent<Unit>().SetTilesInactive();

                                OpenMenu();
                                currentState = selectionState.menuOpen;
                            }
                        }
                        break;

                    case selectionState.selectFoe:
                        if (canBattle.Contains(hit.transform.gameObject))
                        {
                            if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
                            {
                                if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                                {
                                    battleManager.Battle(selectedUnit.gameObject, hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile);
                                    ResetState();
                                    turnManager.CheckEndOfTurn();
                                    FindObjectOfType<BattleForecast>().CloseForecast();
                                    cursor.lockMovement = false;
                                    currentState = selectionState.selectUnit;
                                }
                            }
                        }
                        break;
                }
                
            }

            // Toggles enemies personal dangerzone
            if (hit.transform.tag == "Tile" && hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
            {
                if(hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                {
                    hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.GetComponent<EnemyAI>().HighlightUnitDangerTiles();
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
        ResetTiles();
        canBattle = pathFinder.FindBattleTiles(selectedUnit.gameObject);
        actionMenu.SetActive(true);
    }

    void CloseMenu()
    {
        actionMenu.SetActive(false);
        ResetTiles();
    }

    public void OpenBattleForecast(GameObject foe)
    {
        if(currentState == selectionState.selectFoe)
        {
            (int aHp, int oHp, int aDmg, int oDmg, int aHit, int oHit) = battleManager.ForecastDamage(selectedUnit.gameObject, foe);
            FindObjectOfType<BattleForecast>().OpenForecast(selectedUnit.gameObject, foe, aHp, oHp, aDmg, oDmg, aHit, oHit);
        }
        else
        {
            FindObjectOfType<BattleForecast>().CloseForecast();
        }
    }

    public void battle()
    {
        foreach(GameObject g in canBattle)
        {
            if(g.GetComponent<ShowCursor>().unitOnTile && g.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
            {
                currentState = selectionState.selectFoe;
            }
        }

        if(currentState == selectionState.selectFoe)
        {
            actionMenu.SetActive(false);
            foreach (GameObject g in canBattle)
            {
                GameObject tile = gridManager.GetTile(new Vector2Int((int)(g.transform.position.x + 0.5f), (int)(g.transform.position.z + 0.5f)));
                if (tile.GetComponent<ShowCursor>().unitOnTile && tile.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                {
                    GameObject enemy = tile.GetComponent<ShowCursor>().unitOnTile;
                    enemiesInRange.Add(g);
                }
            }
            cursor.transform.position = new Vector3(enemiesInRange[0].transform.position.x, cursor.gameObject.transform.position.y, enemiesInRange[0].transform.position.z);
        }       
    }

    void SelectEnemy()
    {
        cursor.transform.position = new Vector3(enemiesInRange[selectionIndex].transform.position.x, cursor.gameObject.transform.position.y, enemiesInRange[selectionIndex].transform.position.z);
    }

    public void wait()
    {
        selectedUnit.GetComponent<Unit>().resetIndicators();
        tileWithUnit.GetComponent<ShowCursor>().unitOnTile = null;
        ResetTiles();
        selectedTile.GetComponent<ShowCursor>().unitOnTile = selectedUnit.gameObject;
        selectedUnit.GetComponent<Unit>().canMove = false;
        selectedUnit = null;
        tileWithUnit = null;
        CloseMenu();
        turnManager.CheckEndOfTurn();
        turnManager.UpdateEnemyRange();
        cursor.lockMovement = false;
        currentState = selectionState.selectUnit;
    }

    private void ResetState()
    {
        tileWithUnit.GetComponent<ShowCursor>().unitOnTile = null;
        ResetTiles();
        selectedTile.GetComponent<ShowCursor>().unitOnTile = selectedUnit.gameObject;
        selectedUnit.GetComponent<Unit>().canMove = false;
        selectedUnit = null;
        tileWithUnit = null;
        enemiesInRange = new List<GameObject>();
        selectionIndex = 0;
        CloseMenu();
    }
}
