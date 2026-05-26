using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    //mainmenu
    public GameObject mainMenu;
    //public CanvasGroup mainMenuCanvas;

    //start
    //public GameObject startGame;

    //control
    public GameObject control;
    //public CanvasGroup controlCanvas;

    //settings
    public GameObject settings;
    //public CanvasGroup settingsCanvas;

    //credits
    public GameObject credits;
    //public CanvasGroup creditsCanvas;
    //public CreditsAutoScroll creditsScroll;

    //quit
    //public GameObject quit;
   
    
    public float fadeDuration = 0.5f;

    //private bool isTransitioning = false;


    //Methods
void Start()
{
    mainMenu.SetActive(true);
    credits.SetActive(false);
    control.SetActive(false);
    settings.SetActive(false);
    //quit.SetActive(false);

    //if (mainMenuCanvas != null) mainMenuCanvas.alpha = 1f;
    //else Debug.LogError("mainMenuCanvas is not assigned in MainMenuManager!");
    
    //if (creditsCanvas != null) creditsCanvas.alpha = 0f;
    //if (controlCanvas != null) controlCanvas.alpha = 0f;
    //if (settingsCanvas != null) settingsCanvas.alpha = 0f;
}
    public void StartGame()
    {
        LoadingManager.LoadNextScene("L1_BrokenBoat");
    }
    
    public void OpenControl()
    {
        mainMenu.SetActive(false);
        control.SetActive(true);
    }

    public void CloseControl()
    {
        mainMenu.SetActive(true);
        control.SetActive(false);
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        settings.SetActive(true);
    }

    public void CloseSettings()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }


   public void OpenCredits()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
        //creditsScroll.RestartCredits();
    }

    public void CloseCredits()
    {
        credits.SetActive(false);
        mainMenu.SetActive(true);
    } 

    public void QuitGame()
    {
        Application.Quit();
    }

    //smooth transitions
    IEnumerator FadePanels(GameObject fromPanel, GameObject toPanel, CanvasGroup fromGroup, CanvasGroup toGroup)
    {
        //isTransitioning = true;

        toPanel.SetActive(true);
        toGroup.alpha = 0f;

        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            fromGroup.alpha = Mathf.Lerp(1f, 0f, t);
            toGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        fromGroup.alpha = 0f;
        toGroup.alpha = 1f;

        fromPanel.SetActive(false);

        //isTransitioning = false;
    }
}
