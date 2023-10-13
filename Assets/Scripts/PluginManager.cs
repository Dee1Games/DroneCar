using System.Collections;
using System.Collections.Generic;
//using SupersonicWisdomSDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PluginManager : MonoBehaviour
{
    private void Awake()
    {
        //SupersonicWisdom.Api.AddOnReadyListener(OnPluginReady);
        //SupersonicWisdom.Api.Initialize();
    }

    private void OnPluginReady()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}