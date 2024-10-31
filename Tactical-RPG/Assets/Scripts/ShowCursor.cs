using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCursor : MonoBehaviour
{
    public GameObject cursor;
    public GameObject selectionCursor;
    public GameObject tileToHighlight;
    public GameObject battleIndicator;

    public GameObject unitOnTile;

    public bool highlight;
    public bool indicate;

    public Tile tileInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (highlight)
        {
            tileToHighlight.SetActive(true);
        }
        else
            tileToHighlight.SetActive(false);

        if (Input.GetMouseButtonDown(1))
        {
            highlight = false;
        }

        if(indicate)
        {
            battleIndicator.SetActive(true);
        }
        else
        {
            battleIndicator.SetActive(false);
        }
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
