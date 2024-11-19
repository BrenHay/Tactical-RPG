using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int turnCount = 1;
    
    GameObject[] playerArmy;
    GameObject[] enemyArmy;

    public bool isPlayersTurn = true;

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
        }
        else
        {
            isPlayersTurn = true;
            turnCount++;
        }
    }
}
