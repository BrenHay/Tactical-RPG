using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public bool lockMovement = false;
    
    float xMov = 0;
    float yMov = 0;

    Rigidbody rb;

    public Transform cursorPoint;

    public SelectionManager selection;

    private float spdMultiplier = 1.0f;

    public GameObject highlightedTile;
    public GameObject marker;

    TurnManager turnManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        selection = FindObjectOfType<SelectionManager>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 70 * Time.deltaTime, 0);

        xMov = Input.GetAxis("Horizontal");
        yMov = Input.GetAxis("Vertical");

        Ray ray = new Ray(cursorPoint.position, Vector3.down);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        
        if(hasHit)
        {
            if(hit.transform.tag == "Tile")
            {
                highlightedTile = hit.transform.gameObject;
                marker.transform.position = new Vector3(highlightedTile.transform.position.x, 0.55f, highlightedTile.transform.position.z);
                if(hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile)
                {
                    if (hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile.tag == "Enemy")
                    {
                        selection.OpenBattleForecast(hit.transform.gameObject.GetComponent<ShowCursor>().unitOnTile);
                    }     
                }
                else
                {
                    FindObjectOfType<BattleForecast>().CloseForecast();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            selection.SelectTile();
        }
    }

    private void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            spdMultiplier = 2.0f;
        }
        else
        {
            spdMultiplier = 1.0f;
        }
        if (!lockMovement)
            rb.velocity = new Vector3(xMov * 150 * spdMultiplier * Time.deltaTime, 0, yMov * 150 * spdMultiplier * Time.deltaTime);
        else
            rb.velocity = new Vector3(0, 0, 0);
    }
}
