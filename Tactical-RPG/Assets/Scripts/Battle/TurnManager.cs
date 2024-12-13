using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public int turnCount = 1;

    public GameObject lossScreen;
    public GameObject winScreen;
    
    public List<GameObject> playerArmy;
    public List<GameObject> enemyArmy;

    public bool isPlayersTurn = true;
    public bool battleOver = false;

    public bool returnAfterWin;
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

        
    }

    void EnemyTurn()
    {
        if (!isPlayersTurn)
        {
            foreach (GameObject g in enemyArmy)
            {
                if (g.GetComponent<Unit>().canMove)
                {
                    g.GetComponent<Unit>().canMove = false;
                    g.GetComponent<EnemyAI>().MoveEnemy();

                }
            }
            CheckEndOfBattle();
            SwitchTurn();
        }
    }

    public void HighlightEnemyRange()
    {
        foreach(GameObject g in enemyArmy)
        {
            if(!g.GetComponent<EnemyAI>().defeated)
            {
                foreach (GameObject j in g.GetComponent<EnemyAI>().attackTiles)
                {
                    if (showEnemyRange)
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
    }

    public void CheckEndOfTurn()
    {
        CheckEndOfBattle();
        foreach (GameObject g in playerArmy)
        {
            if(g.GetComponent<Unit>().canMove)
            {
                return;
            }
        }
        SwitchTurn();
    }

    void CheckEndOfBattle()
    {
        foreach (GameObject j in enemyArmy)
        {
            if (!j.GetComponent<EnemyAI>().defeated)
            {
                return;
            }
        }
        if(playerArmy.Count < 1)
        {
            lossScreen.SetActive(true);
            return;
        }
        battleOver = true;
        cursor.SetActive(false);
        if(returnAfterWin)
        {
            Scene currentScene = SceneManager.GetSceneAt(1);
            FindObjectOfType<SceneManagement>().UnloadScene(currentScene.name);
        }
        else
        {
            winScreen.SetActive(true);
        }
    }

    public void SwitchTurn()
    {
        if (isPlayersTurn)
        {
            isPlayersTurn = false;
            EnemyTurn();
            ResetEnemyUnits();
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
            if(!g.GetComponent<EnemyAI>().defeated)
                g.GetComponent<Unit>().canMove = true;
        }
    }

    public void UpdateEnemyRange()
    {
        foreach (GameObject g in enemyArmy)
        {
            g.GetComponent<EnemyAI>().Unhighlight();
            g.GetComponent<EnemyAI>().GetRange();
            if (g.GetComponent<EnemyAI>().highlightUnitTiles && !g.GetComponent<EnemyAI>().defeated)
            {
                g.GetComponent<EnemyAI>().Rehighlight();
            }
        }
        if (showEnemyRange)
            HighlightEnemyRange();
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
