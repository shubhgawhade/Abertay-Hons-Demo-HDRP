using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject loadOrStartUI;
    
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
        if (GameManager.hasSave)
        {
            // ENABLE CONTINUE OR NEW GAME UI
            // SETS SCENE TO LOAD
            loadOrStartUI.SetActive(true);
        }
        else
        {
            async.allowSceneActivation = true;
        }
        // GameManager.CurrentScene += 1;
    }

    public void NewGame()
    {
        GameManager.CurrentScene = 2;
        GameManager.useSave = false;
        SceneManager.LoadScene(2);
        // SceneManager.UnloadSceneAsync(GameManager.CurrentScene);
        // GameManager.CurrentScene = 1;
        // StartCoroutine(LoadSceneAsync1());
        // async1.allowSceneActivation = true;
    }

    public void Continue()
    {
        GameManager.useSave = true;
        async.allowSceneActivation = true;
    }
}
