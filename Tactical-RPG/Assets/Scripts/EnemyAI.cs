using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GridManager gridManager;
    PathFinder pathfinder;
    public List<GameObject> attackTiles;

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
        foreach(GameObject g in attackTiles)
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

    public void Rehighlight()
    {
        foreach(GameObject g in attackTiles)
        {
            g.GetComponent<ShowCursor>().unitDanger = true;    
        }
    }

    public void MoveEnemy()
    {
        List<GameObject> nearbyEnemyTiles = new List<GameObject>();
        foreach(GameObject g in attackTiles)
        {
            Debug.Log(g.transform.position);
            if(g.GetComponent<ShowCursor>().unitOnTile)
            {
                if (g.GetComponent<ShowCursor>().unitOnTile.tag == "Unit")
                {

                    nearbyEnemyTiles.Add(g);
                    
                }
            }
        }

        if(nearbyEnemyTiles.Count == 0 && !isAggresive)
        {
            return;
        }
        // Choose enemy to attack
        GameObject tileToAttack = null;
        BattleManager battle = FindObjectOfType<BattleManager>();
        int currentBattleDamage = 0;
        foreach(GameObject g in nearbyEnemyTiles)
        {
            if(!tileToAttack)
            {
                tileToAttack = g;
                currentBattleDamage = battle.SimulateDamage(gameObject, g.GetComponent<ShowCursor>().unitOnTile);
            }
            else
            {
                int possibleBattleDamage = battle.SimulateDamage(gameObject, g.GetComponent<ShowCursor>().unitOnTile);
                if(possibleBattleDamage > currentBattleDamage)
                {
                    currentBattleDamage = possibleBattleDamage;
                    tileToAttack = g;
                }
            }
        }

        // Now Move to the closest available battle tile and battle!
        GameObject tileToMoveTo = null;
        foreach(GameObject g in attackTiles)
        {
            if(Vector3.Distance(g.transform.position, tileToAttack.transform.position) == GetComponent<Unit>().stats.Range)
            {
                if(!g.GetComponent<ShowCursor>().unitOnTile)
                {
                    tileToMoveTo = g;
                    break;
                }else
                {
                    if(g.GetComponent<ShowCursor>().unitOnTile == gameObject)
                    {
                        tileToMoveTo = g;
                        break;
                    }
                }
            }
        }

        gridManager.GetTile(new Vector2Int((int)(transform.position.x + 0.5),(int)(transform.position.z + 0.5))).GetComponent<ShowCursor>().unitOnTile = null;
        transform.position = new Vector3(tileToMoveTo.transform.position.x, transform.position.y, tileToMoveTo.transform.position.z);
        tileToMoveTo.GetComponent<ShowCursor>().unitOnTile = gameObject;
        battle.Battle(gameObject, tileToAttack.GetComponent<ShowCursor>().unitOnTile);
        GetComponent<Unit>().canMove = false;
    }
}
