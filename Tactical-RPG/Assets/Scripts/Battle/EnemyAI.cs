using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    GridManager gridManager;
    PathFinder pathfinder;
    public List<GameObject> attackTiles;
    public List<GameObject> movableTiles;

    public List<GameObject> dangerArea;
    public List<GameObject> personalRange;

    [SerializeField] private GameObject dangerTile;
    [SerializeField] private GameObject rangeTile;

    public bool highlightUnitTiles;
    public bool isAggresive;
    public bool findRange = true;
    public bool defeated = false;
    
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
        ResetTiles();
        attackTiles = pathfinder.FindEnemyRange(gameObject);
        foreach (GameObject g in attackTiles)
        {
            g.GetComponent<ShowCursor>().searched = false;
        }
        List<Transform> transforms = attackTiles.Select(x => x.transform).Distinct().ToList();
        movableTiles = pathfinder.FindWalkableTiles(gameObject);

        foreach (GameObject g in movableTiles)
        {
            g.GetComponent<ShowCursor>().searched = false;
        }

        foreach(Transform t in transforms)
        {
            Vector3 transform = new Vector3(t.transform.position.x, t.transform.position.y + 0.532f, t.transform.position.z);
            Vector3 transform2 = new Vector3(t.transform.position.x, t.transform.position.y + 0.533f, t.transform.position.z);
            dangerArea.Add(Instantiate(dangerTile, transform, t.rotation));
            personalRange.Add(Instantiate(rangeTile, transform2, t.rotation));
        }

    }

    private void ResetTiles()
    {
        foreach(GameObject g in dangerArea)
        {
            Destroy(g);
        }
    }

    public void HighlightUnitDangerTiles()
    {
        highlightUnitTiles = !highlightUnitTiles;

        foreach (GameObject g in personalRange)
        {
            if (highlightUnitTiles)
            {
                g.SetActive(true);
            }
            else
            {
                g.SetActive(false);
            }
        }
    }

    public void Unhighlight()
    {
        foreach(GameObject g in attackTiles)
        {
            g.GetComponent<ShowCursor>().dangerZone = false;
            g.GetComponent<ShowCursor>().unitDanger = false;
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
        GetComponent<Unit>().canMove = false;
        Unhighlight();
        if (tileToAttack)
        {
            isAggresive = true;
            Debug.Log("Battle");
            battle.Battle(gameObject, tileToAttack.GetComponent<ShowCursor>().unitOnTile);
        }       
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
        else if (isAggresive && nearbyEnemyTiles.Count == 0)
        {
            GameObject closestEnemy;

            List<GameObject> path = pathfinder.PathToClosestEnemy(gameObject);
            GameObject moveTo = null;
            foreach (GameObject g in path)
            {
                if (movableTiles.Contains(g))
                    moveTo = g;
            }
            return (moveTo, null);
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

        Vector2Int opponentPos;

        if (tileToAttack)
        {
            opponentPos = new Vector2Int((int)(tileToAttack.transform.position.x + 0.5f), (int)(tileToAttack.transform.position.z + 0.5f));
        }
        else
            return (null, null);
        

        // Now Move to the closest available battle tile and battle!
        GameObject tileToMoveTo = null;
        foreach (GameObject g in movableTiles)
        {

            Vector2Int tilePos = new Vector2Int((int)(g.transform.position.x + 0.5f), (int)(g.transform.position.z + 0.5f));
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

    public void DefeatEnemy()
    {
        Vector2Int position = GetComponent<Unit>().GetPostion();
        gridManager.GetTile(position).GetComponent<ShowCursor>().unitOnTile = null;
        gameObject.SetActive(false);
        defeated = true;
        Unhighlight();
    }

    IEnumerator moveToEnemy(GameObject tile)
    {
        Vector3 targetTransform = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
        while(transform.position != targetTransform)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform, 2.0f * Time.deltaTime);
            yield return null;
        }
        
        yield return null;
    }
}
