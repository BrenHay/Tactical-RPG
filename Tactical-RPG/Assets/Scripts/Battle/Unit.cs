using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    public Stats stats;
    public string damageType;
    public string attackType;
    public bool canMove = true;

    public TextMeshPro hpText;
    
    // Start is called before the first frame update
    void Start()
    {
        SetTile();
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = "HP: " + stats.currentHp;
    }

    public Vector2Int GetPostion()
    {
        return new Vector2Int((int)(transform.position.x + 0.5f), (int)(transform.position.z + 0.5f));
    }

    public void SetTile()
    {
        FindObjectOfType<GridManager>().GetTile(GetPostion()).GetComponent<ShowCursor>().unitOnTile = gameObject;
    }
}
