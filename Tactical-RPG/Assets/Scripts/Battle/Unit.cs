using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Unit : MonoBehaviour
{
    public Stats stats;
    public string damageType;
    public string attackType;
    public bool canMove = true;

    public GameObject walkIndicator;
    public GameObject battleIndicator;

    private List<GameObject> spawnedIndicators = new List<GameObject>();

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

    public void spawnTiles(List<GameObject> tiles)
    {
        Debug.Log(tiles.Count);
        tiles.Select(x => x.transform).Distinct();
        List<Transform> transforms = tiles.Select(x => x.transform).Distinct().ToList();
        Debug.Log(transforms.Count);
        //List<GameObject> toSpawn = tiles.Select(x => x.transform.position).Distinct();
        foreach(Transform t in transforms)
        {
            Debug.Log(t.transform.position);
            Vector3 transform = new Vector3(t.transform.position.x, t.transform.position.y + 0.53f, t.transform.position.z);
            spawnedIndicators.Add(Instantiate(walkIndicator, transform, t.transform.rotation));
        }
    }

    public void SetTilesInactive()
    {
        foreach(GameObject g in spawnedIndicators)
        {
            g.SetActive(false);
        }
    }

    public void SetTilesActive()
    {
        foreach (GameObject g in spawnedIndicators)
        {
            g.SetActive(true);
        }
    }

    public void resetIndicators()
    {
        foreach(GameObject g in spawnedIndicators)
        {
            Destroy(g);
        }
        spawnedIndicators.Clear();
    }
}
