using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GridManager gridManager;
    PathFinder pathfinder;
    List<GameObject> attackTiles;

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
        Debug.Log("Highlight");
        GameObject[] otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject g in attackTiles)
        {
            if(g.GetComponent<ShowCursor>().unitDanger)
            {
                highlightUnitTiles = false;
                g.GetComponent<ShowCursor>().unitDanger = false;
            }
            else
            {
                highlightUnitTiles = true;
                g.GetComponent<ShowCursor>().unitDanger = true;
            }
        }

        foreach(GameObject g in otherEnemies)
        {
            if(g.GetComponent<EnemyAI>().highlightUnitTiles)
            {
                g.GetComponent<EnemyAI>().Rehighlight();
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

    void HighlightDangerTiles()
    {

    }
}
