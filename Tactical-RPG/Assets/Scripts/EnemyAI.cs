using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GridManager gridManager;
    PathFinder pathfinder;
    List<GameObject> attackTiles;

    public bool isAggresive;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        pathfinder = FindObjectOfType<PathFinder>();
        GetRange();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetRange()
    {
        attackTiles = pathfinder.FindEnemyRange(gameObject);
    }
}
