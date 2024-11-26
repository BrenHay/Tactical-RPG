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
        Debug.Log("Move Enemy");
        GameObject closestUnitTile = null;
        foreach(GameObject g in attackTiles)
        {
            Debug.Log("Attack Tile");
            if(g.GetComponent<ShowCursor>().unitOnTile)
            {
                Debug.Log("Unit On Tile");
                if (g.GetComponent<ShowCursor>().unitOnTile.tag == "Unit")
                {
                    Debug.Log("unitOnTile = Unit");
                    if(!closestUnitTile)
                    {
                        closestUnitTile = g;
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, g.transform.position) < Vector3.Distance(transform.position, closestUnitTile.transform.position))
                        {
                            closestUnitTile = g;
                        }
                    }
                    
                }
            }
        }
        GameObject closestMovableTile = null;
        foreach(GameObject g in attackTiles)
        {
            if(Vector3.Distance(g.transform.position, closestUnitTile.transform.position) == gameObject.GetComponent<Unit>().stats.Range)
            {
                if(!g.GetComponent<ShowCursor>().unitOnTile)
                {
                    closestMovableTile = g;
                }
            }
        }

        transform.position = new Vector3(closestMovableTile.transform.position.x, transform.position.y, closestMovableTile.transform.position.z);
        GetComponent<Unit>().canMove = false;
    }
}
