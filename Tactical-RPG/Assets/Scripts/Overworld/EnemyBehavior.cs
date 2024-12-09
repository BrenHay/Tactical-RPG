using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    SceneManagement scene;
    
    // Start is called before the first frame update
    void Start()
    {
        scene = FindObjectOfType<SceneManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // Send to battle scene that works hopefully
            scene.LoadRandomEncounter("RandomEncounter");
            Destroy(gameObject);
        }
    }
}
