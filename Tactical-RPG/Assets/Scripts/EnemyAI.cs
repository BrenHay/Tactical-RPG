using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GridManager gridManager;
    PathFinder pathfinder;
    public List<GameObject> attackTiles;
    public List<GameObject> movableTiles;

    public bool highlightUnitTiles;
    public bool isAggresive;
    public bool findRange = true;
    
    // Start is called before the first frame update
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<PathFinder>();
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(findRange)
        {
            GetRange();
            findRange = false;
        }
    }

    public void GetRange()
    {
        attackTiles = pathfinder.FindEnemyRange(gameObject);
        foreach (GameObject g in attackTiles)
        {
            g.GetComponent<ShowCursor>().searched = false;
        }
        movableTiles = pathfinder.FindWalkableTiles(gameObject);

        foreach (GameObject g in movableTiles)
        {
            g.GetComponent<ShowCursor>().searched = false;
        }

    }

    public void HighlightUnitDangerTiles()
    {
        GameObject[] otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject g in attackTiles)
        {
            if(highlightUnitTiles)
            {
                g.GetComponent<ShowCursor>().unitDanger = false;
            }
            else
            {
                g.GetComponent<ShowCursor>().unitDanger = true;
            }
        }
        highlightUnitTiles = !highlightUnitTiles;

        foreach(GameObject g in otherEnemies)
        {
            if(g.GetComponent<EnemyAI>().highlightUnitTiles)
            {
                if(g != gameObject)
                {
                    g.GetComponent<EnemyAI>().Rehighlight();
                }    
            }
        }
    }

    public void Unhighlight()
    {
        foreach(GameObject g in attackTiles)
        {
            if(highlightUnitTiles)
            {
                g.GetComponent<ShowCursor>().unitDanger = false;
            }
            if(g.GetComponent<ShowCursor>().dangerZone)
            {
                g.GetComponent<ShowCursor>().dangerZone = false;
            }
        }
    }

    public void Rehighlight()
    {
        foreach(GameObject g in attackTiles)
        {
            g.GetComponent<ShowCursor>().unitDanger = true;    
        }
    }

    public void MoveEnemy()
    {
        BattleManager battle = FindObjectOfType<BattleManager>();

        (GameObject tileToMoveTo, GameObject tileToAttack) = FindMoveTile();
        if(tileToMoveTo == null && tileToAttack == null)
        {
            return;
        }
        gridManager.GetTile(new Vector2Int((int)(transform.position.x + 0.5),(int)(transform.position.z + 0.5))).GetComponent<ShowCursor>().unitOnTile = null;
        transform.position = new Vector3(tileToMoveTo.transform.position.x, transform.position.y, tileToMoveTo.transform.position.z);
        tileToMoveTo.GetComponent<ShowCursor>().unitOnTile = gameObject;
        if(tileToAttack)
        {
            battle.Battle(gameObject, tileToAttack.GetComponent<ShowCursor>().unitOnTile);
        }
        
        GetComponent<Unit>().canMove = false;
        Unhighlight();
    }

    (GameObject, GameObject) FindMoveTile()
    {
        // Find Enemies in range
        List<GameObject> nearbyEnemyTiles = new List<GameObject>();
        foreach (GameObject g in attackTiles)
        {
            if (g.GetComponent<ShowCursor>().unitOnTile)
            {
                if (g.GetComponent<ShowCursor>().unitOnTile.tag == "Unit")
                {
                    nearbyEnemyTiles.Add(g);
                }
            }
        }

        if (nearbyEnemyTiles.Count == 0 && !isAggresive)
        {
            return (null, null);
        }

        // Choose enemy to attack
        GameObject tileToAttack = null;
        BattleManager battle = FindObjectOfType<BattleManager>();
        int currentBattleDamage = 0;
        foreach (GameObject g in nearbyEnemyTiles)
        {
            if (!tileToAttack)
            {
                tileToAttack = g;
                currentBattleDamage = battle.SimulateDamage(gameObject, g.GetComponent<ShowCursor>().unitOnTile);
            }
            else
            {
                int possibleBattleDamage = battle.SimulateDamage(gameObject, g.GetComponent<ShowCursor>().unitOnTile);
                if (possibleBattleDamage > currentBattleDamage)
                {
                    currentBattleDamage = possibleBattleDamage;
                    tileToAttack = g;
                }
            }
        }

        Vector2Int opponentPos = new Vector2Int((int)(tileToAttack.transform.position.x + 0.5f), (int)(tileToAttack.transform.position.z + 0.5f));

        // Now Move to the closest available battle tile and battle!
        GameObject tileToMoveTo = null;
        foreach (GameObject g in movableTiles)
        {

            Vector2Int tilePos = new Vector2Int((int)(g.transform.position.x + 0.5f), (int)(g.transform.position.z + 0.5f));
            Debug.Log(Vector2Int.Distance(tilePos, opponentPos));
            if (Mathf.CeilToInt(Vector2Int.Distance(tilePos, opponentPos)) == GetComponent<Unit>().stats.Range)
            {
                if (!g.GetComponent<ShowCursor>().unitOnTile)
                {
                    tileToMoveTo = g;
                    break;
                }
                else
                {
                    if (g.GetComponent<ShowCursor>().unitOnTile == gameObject)
                    {
                        tileToMoveTo = g;
                        break;
                    }
                }
            }
        }
        return (tileToMoveTo, tileToAttack);
    }
}
