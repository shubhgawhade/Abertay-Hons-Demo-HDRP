using UnityEngine;

public class Chapter2UI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private CharacterControl playerCharacterControl;
    
    private void Update()
    {
        if (GameManager.isPaused)
        {
            pauseMenu.SetActive(true);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }
    

    public void StopInspecting()
    {
        playerCharacterControl.currentInteractable.GetComponent<InspectableInteractables>().StopInspecting();
    }

    public void Resume()
    {
        GameManager.isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        GameManager.isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
