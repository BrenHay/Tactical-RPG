using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTile : MonoBehaviour
{
    public GameObject[] tileOptions;
    
    // Start is called before the first frame update
    void Start()
    {
        //.Log("instantiate");
        Instantiate(tileOptions[Random.Range(0, tileOptions.Length)], transform.position, transform.rotation);
        FindObjectOfType<GridManager>().UpdateDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
