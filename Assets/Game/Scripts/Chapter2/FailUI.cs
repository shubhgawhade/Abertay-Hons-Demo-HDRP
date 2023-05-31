using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailUI : MonoBehaviour
{
    private AsyncOperation async;
    
    public static Action Load;
    
    // Start is called before the first frame update
    void Start()
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
    
    public void RetryButton()
    {
        GameManager.useSave = true;
        async.allowSceneActivation = true;
        Load();
        // SceneManager.LoadScene(GameManager.CurrentScene);
    }

    public void MainMenuButton()
    {
        GameManager.isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
