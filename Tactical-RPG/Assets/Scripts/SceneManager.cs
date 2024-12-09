using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    static SceneManagement scene;

    public GameObject currentOverWorld;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if(!scene)
        {
            scene = GetComponent<SceneManagement>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableOverworld()
    {
        currentOverWorld.SetActive(false);
    }

    void EnableOverworld()
    {
        currentOverWorld.SetActive(true);
    }

    public void LoadRandomEncounter(string sceneName)
    {
        DisableOverworld();
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync("RandomEncounter");
        EnableOverworld();
    }
}
