using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int turnCount = 1;
    
    public List<GameObject> playerArmy;
    public List<GameObject> enemyArmy;

    public bool isPlayersTurn = true;
    bool showEnemyRange = false;

    GameObject cursor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        playerArmy.AddRange(GameObject.FindGameObjectsWithTag("Unit"));
        enemyArmy.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        cursor = GameObject.FindGameObjectWithTag("Cursor");
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayersTurn)
        {
            cursor.SetActive(true);
        }
        else
        {
            cursor.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            showEnemyRange = !showEnemyRange;
            HighlightEnemyRange();
        }

        if(!isPlayersTurn)
        {
            
            foreach(GameObject g in enemyArmy)
            {
                if(g.GetComponent<Unit>().canMove)
                {
                    g.GetComponent<Unit>().canMove = false;
                    g.GetComponent<EnemyAI>().MoveEnemy();
                    
                }
            }
            SwitchTurn();
        }
    }

    public void HighlightEnemyRange()
    {
        foreach(GameObject g in enemyArmy)
        {
            foreach(GameObject j in g.GetComponent<EnemyAI>().attackTiles)
            {
                if(showEnemyRange)
                {
                    j.GetComponent<ShowCursor>().dangerZone = true;                   
                }
                else
                {
                    j.GetComponent<ShowCursor>().dangerZone = false;
                }
            }
        }
    }

    public void CheckEndOfTurn()
    {
        foreach(GameObject g in playerArmy)
        {
            if(g.GetComponent<Unit>().canMove)
            {
                return;
            }
        }
        SwitchTurn();
    }

    public void SwitchTurn()
    {
        if(isPlayersTurn)
        {
            isPlayersTurn = false;
            ResetEnemyUnits();
            //SwitchTurn();
        }
        else
        {
            isPlayersTurn = true;
            turnCount++;
            ResetPlayerUnits();
            UpdateEnemyRange();
        }
    }

    void ResetPlayerUnits()
    {
        foreach(GameObject g in playerArmy)
        {
            g.GetComponent<Unit>().canMove = true;
        }
    }

    void ResetEnemyUnits()
    {
        
        foreach (GameObject g in enemyArmy)
        {
            g.GetComponent<Unit>().canMove = true;
        }
        //playerArmy = GameObject.FindGameObjectsWithTag("Player");
    }

    public void UpdateEnemyRange()
    {
        foreach (GameObject g in enemyArmy)
        {
            g.GetComponent<EnemyAI>().Unhighlight();
            g.GetComponent<EnemyAI>().GetRange();
            if (g.GetComponent<EnemyAI>().highlightUnitTiles)
            {
                g.GetComponent<EnemyAI>().Rehighlight();
            }
        }
        if (showEnemyRange)
            HighlightEnemyRange();
    }
}
