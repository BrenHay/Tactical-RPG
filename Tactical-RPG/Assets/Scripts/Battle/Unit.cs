using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Stats stats;
    public string damageType;
    public string attackType;
    public bool canMove = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int GetPostion()
    {
        return new Vector2Int((int)(transform.position.x + 0.5f), (int)(transform.position.z + 0.5f));
    }
}
