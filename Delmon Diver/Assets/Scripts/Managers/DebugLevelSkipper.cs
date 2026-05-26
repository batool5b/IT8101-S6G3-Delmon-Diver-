using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugLevelSkipper : MonoBehaviour
{
    private static DebugLevelSkipper instance;

    private readonly string[] levelOrder = new string[]
    {
        "MainMenu",
        "L1_BrokenBoat",
        "L2_SmallIsland",
        "L3_OpenSea_ToMain",
        "L4_MainIsland",
        "L4.2_Cave",
        "L5_OpenSea_ToHome",
        "GameEnding"
    };

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // This MUST be public and have NO variables inside the parentheses
    public void SkipLevelButtonAction()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        TriggerNextScene(currentScene);
    }

    private void TriggerNextScene(string currentSceneName)
    {
        int currentIndex = -1;

        for (int i = 0; i < levelOrder.Length; i++)
        {
            if (levelOrder[i] == currentSceneName)
            {
                currentIndex = i;
                break;
            }
        }

        int nextIndex = currentIndex + 1;

        if (nextIndex < levelOrder.Length)
        {
            Debug.Log($"[Debug] Skipping to {levelOrder[nextIndex]}");
            LoadingManager.LoadNextScene(levelOrder[nextIndex]);
        }
        else
        {
            LoadingManager.LoadNextScene("MainMenu");
        }
    }
}