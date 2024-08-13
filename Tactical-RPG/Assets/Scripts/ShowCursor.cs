using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    public GameObject cursor;
    public GameObject selectionCursor;

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
        cursor.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        cursor.gameObject.SetActive(false);
    }
}
