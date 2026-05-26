using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private TMP_Text levelNumberText;
    [SerializeField] private TMP_Text levelTitleText;
    [SerializeField] private Button skipButton;

    [Header("Loading Settings")]
    [SerializeField] private float fakeLoadTime = 3f;
    [SerializeField] private string sceneToLoad = "L1_SunkenShip";
    
    [Header("Tips Settings")]
    [SerializeField] private float tipChangeInterval = 4f;

    // Static cache so any script can trigger: LoadingManager.LoadNextScene("SceneName");
    public static string NextSceneName = "";

    [System.Serializable]
    public class LevelData
    {
        public string sceneName;
        public string levelNumber;
        public string levelTitle;
        public string[] levelTips;
    }

    // Pre-configured metadata for every scene in the game flow
    private static readonly System.Collections.Generic.Dictionary<string, LevelData> DefaultLevels = new()
    {
        { "MainMenu", new LevelData {
            sceneName = "MainMenu",
            levelNumber = "",
            levelTitle = "Main Menu",
            levelTips = new string[] {
                "Welcome to Delmon Diver! Adjust your resolution and audio settings in the options menu.",
                "Choose to take the ancient treasure at the end or return home as a humble diver."
            }
        }},
        { "IntroStory", new LevelData {
            sceneName = "IntroStory",
            levelNumber = "Prologue",
            levelTitle = "The Beginning",
            levelTips = new string[] {
                "Watch the cinematic story of how the diver embarked on this great voyage.",
                "The historic Persian Gulf has always been famous for pearling and sailing."
            }
        }},
        { "L1_BrokenBoat", new LevelData {
            sceneName = "L1_BrokenBoat",
            levelNumber = "Level 1",
            levelTitle = "The Sunken Ship",
            levelTips = new string[] {
                "Tip: Search the sunken ship wreckage for essential items.",
                "Tip: Watch your oxygen meter carefully while diving deep!",
                "Tip: Retrieve the items and swim to the surface before your oxygen runs out.",
                "Tip: Limited air means you must prioritize your targets and be fast."
            }
        }},
        { "L2_SmallIsland", new LevelData {
            sceneName = "L2_SmallIsland",
            levelNumber = "Level 2",
            levelTitle = "Small Island",
            levelTips = new string[] {
                "Tip: Collect floating resources around the tiny island in the Persian Gulf.",
                "Tip: Listen to your Parrot companion—he is witty and knowledgeable about Bahraini culture!",
                "Tip: Gather wood and stone to build a shelter and craft a small boat."
            }
        }},
        { "L3_OpenSea_ToMain", new LevelData {
            sceneName = "L3_OpenSea_ToMain",
            levelNumber = "Level 3",
            levelTitle = "Open Sea Journey",
            levelTips = new string[] {
                "Tip: Keep materials on hand to repair your boat during the journey.",
                "Tip: Beware of hazards like storms and violent waves in the open sea.",
                "Tip: Territorial sea creatures like sharks can attack—learn their movements!"
            }
        }},
        { "L4_MainIsland", new LevelData {
            sceneName = "L4_MainIsland",
            levelNumber = "Level 4.1",
            levelTitle = "Main Survival Island (Delmon)",
            levelTips = new string[] {
                "Tip: Hunt wildlife like gazelles, but stay alert for dangerous wolves and lions!",
                "Tip: Manage your food, shelter, and tool durability carefully to survive.",
                "Tip: Explore biomes thoroughly; you might find a secret treasure map!"
            }
        }},
        { "L4.2_Cave", new LevelData {
            sceneName = "L4.2_Cave",
            levelNumber = "Level 4.2",
            levelTitle = "The Cave (Optional)",
            levelTips = new string[] {
                "Tip: Deep water diving and cave exploration require excellent oxygen management.",
                "Tip: Watch out for ancient traps and puzzles protecting the treasure.",
                "Tip: Prepare for a tough boss battle against the mythical Treasure Keeper!"
            }
        }},
        { "L5_OpenSea_ToHome", new LevelData {
            sceneName = "L5_OpenSea_ToHome",
            levelNumber = "Level 5",
            levelTitle = "Open Sea Journey → Home",
            levelTips = new string[] {
                "Tip: Pirates will board and attack your boat—prepare your weapons!",
                "Tip: Navigate carefully through the final stretch of open sea.",
                "Tip: Upgrade your boat to withstand heavy waves and storm hazards."
            }
        }},
        { "GameEnding", new LevelData {
            sceneName = "GameEnding",
            levelNumber = "Epilogue",
            levelTitle = "The Final Destination",
            levelTips = new string[] {
                "Tip: Your choices determine your final destiny. Rich nokheda or humble diver?",
                "Tip: Did you find the ancient Delmon treasure?"
            }
        }}
    };

    /// <summary>
    /// Call this from any script to load a level using the Loading Screen.
    /// Example: LoadingManager.LoadNextScene("L2_SmallIsland");
    /// </summary>
    public static void LoadNextScene(string sceneName)
    {
        NextSceneName = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        // 1. Retrieve the cached target scene name
        if (!string.IsNullOrEmpty(NextSceneName))
        {
            sceneToLoad = NextSceneName;
        }

        // 2. Set the Level Title and Number texts based on the level database
        ConfigureLevelUI();

        // 3. Kick off async loading and tip rotation
        StartCoroutine(LoadSceneCoroutine());
        StartCoroutine(RotateTipsCoroutine());
    }

    private void ConfigureLevelUI()
    {
        LevelData currentLevelData = null;
        if (DefaultLevels.TryGetValue(sceneToLoad, out currentLevelData))
        {
            if (levelNumberText != null)
            {
                levelNumberText.text = currentLevelData.levelNumber;
            }
            if (levelTitleText != null)
            {
                levelTitleText.text = currentLevelData.levelTitle;
            }
        }
        else
        {
            // Fallback for custom or test scenes not defined in the dictionary
            if (levelNumberText != null)
            {
                levelNumberText.text = "Loading";
            }
            if (levelTitleText != null)
            {
                levelTitleText.text = sceneToLoad.Replace("_", " ");
            }
        }
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;
            float fakeProgress = Mathf.Clamp01(timer / fakeLoadTime);
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float progress = Mathf.Min(fakeProgress, realProgress);

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"Loading... {Mathf.FloorToInt(progress * 100)}%";
            }

            if (progress >= 1f && operation.progress >= 0.9f)
            {
                if (loadingText != null)
                {
                    loadingText.text = "Loading... 100%";
                }
                yield return new WaitForSeconds(0.5f);
                
                // Reset static cache for safety
                NextSceneName = ""; 
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator RotateTipsCoroutine()
    {
        string[] activeTips = null;

        // Try to get tips for the specific level
        LevelData currentLevelData = null;
        if (DefaultLevels.TryGetValue(sceneToLoad, out currentLevelData))
        {
            if (currentLevelData.levelTips != null && currentLevelData.levelTips.Length > 0)
            {
                activeTips = currentLevelData.levelTips;
            }
        }

        // Fallback global tips if none are defined
        if (activeTips == null || activeTips.Length == 0)
        {
            activeTips = new string[]
            {
                "Tip: Deep diving requires watching your oxygen closely.",
                "Tip: You can use your hotbar items to heal yourself."
            };
        }

        int lastIndex = -1;

        while (true)
        {
            int index = 0;
            if (activeTips.Length > 1)
            {
                do
                {
                    index = Random.Range(0, activeTips.Length);
                } while (index == lastIndex);
            }
            lastIndex = index;

            if (tipText != null)
            {
                yield return StartCoroutine(FadeText(1f, 0f, 0.5f));
                tipText.text = activeTips[index];
                yield return StartCoroutine(FadeText(0f, 1f, 0.5f));
            }

            yield return new WaitForSeconds(tipChangeInterval);
        }
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        if (tipText == null) yield break;

        float time = 0f;
        Color color = tipText.color;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            tipText.color = new Color(color.r, color.g, color.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
        tipText.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}