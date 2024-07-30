using System.Collections;
using System.Collections.Generic;
//using HomaGames.HomaBelly; //TODO HOMA
//using SupersonicWisdomSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PluginManager : MonoBehaviour
{
    //private void Awake()
    //{
        //SupersonicWisdom.Api.AddOnReadyListener(OnPluginReady);
        //SupersonicWisdom.Api.Initialize();
    //}

    private void OnPluginReady()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void Awake()
    {
        /*if (!HomaBelly.Instance.IsInitialized)
        {
            // Listen event for initialization
            Events.onInitialized += OnInitialized;
        }
        else*/ //TODO HOMA
        {
            // Homa Belly already initialized
            OnPluginReady();
        }
    }
        
    private void OnDisable()
    {
        //Events.onInitialized -= OnInitialized; //TODO HOMA
    }

    private void OnInitialized()
    {
        OnPluginReady();
        // Homa Belly initialized, call any Homa Belly method
    }
}