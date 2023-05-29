using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chapter2UI : MonoBehaviour
{
    public static Action Load;

    public void RetryButton()
    {
        SceneManager.LoadScene(GameManager.CurrentScene);
        Load();
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(0);
    }
}
