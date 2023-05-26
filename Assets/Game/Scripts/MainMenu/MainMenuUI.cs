using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    private AsyncOperation async;
    
    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
        async.allowSceneActivation = false;
    }
    
    private IEnumerator LoadSceneAsync()
    {
        async = SceneManager.LoadSceneAsync(GameManager.CurrentScene);
        while (!async.isDone)
        {
            yield return null;
        }
    }

    public void StartGame()
    {
        async.allowSceneActivation = true;

        // GameManager.CurrentScene += 1;
    }
}
