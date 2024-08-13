using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Stats stats;
    public GameObject tileCurrentlyOn;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        tileCurrentlyOn.GetComponentInChildren<ShowCursor>().cursor.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        tileCurrentlyOn.GetComponentInChildren<ShowCursor>().cursor.gameObject.SetActive(false);
    }
}
