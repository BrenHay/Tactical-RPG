using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int turnCount = 1;
    
    GameObject[] playerArmy;
    GameObject[] enemyArmy;

    public bool isPlayersTurn = true;
    bool showEnemyRange = false;

    GameObject cursor;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        playerArmy = GameObject.FindGameObjectsWithTag("Unit");
        enemyArmy = GameObject.FindGameObjectsWithTag("Enemy");
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
            HighlightEnemyRange();
        }

        if(!isPlayersTurn)
        {
            foreach(GameObject g in enemyArmy)
            {
                if(g.GetComponent<Unit>().canMove)
                {
                    g.GetComponent<EnemyAI>().MoveEnemy();
                    g.GetComponent<Unit>().canMove = false;
                }
            }
            SwitchTurn();
        }
    }

    void HighlightEnemyRange()
    {
        foreach(GameObject g in enemyArmy)
        {
            foreach(GameObject j in g.GetComponent<EnemyAI>().attackTiles)
            {
                if(showEnemyRange)
                {
                    j.GetComponent<ShowCursor>().dangerZone = false;
                }
                else
                {
                    j.GetComponent<ShowCursor>().dangerZone = true;
                }
            }
        }
        showEnemyRange = !showEnemyRange;
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
            //SwitchTurn();
        }
        else
        {
            isPlayersTurn = true;
            turnCount++;
            ResetPlayerUnits();
            foreach(GameObject g in enemyArmy)
            {
                g.GetComponent<EnemyAI>().GetRange();
            }
        }
    }

    void ResetPlayerUnits()
    {
        foreach(GameObject g in playerArmy)
        {
            g.GetComponent<Unit>().canMove = true;
        }
    }
}
