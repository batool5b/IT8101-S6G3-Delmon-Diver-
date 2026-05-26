using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class LoadMainMenu : MonoBehaviour
{

    public PlayableDirector timeline;  
    public string nextScene = "MainMenu"; 

    void Start()
    {
        if (timeline != null)
        {
            timeline.stopped += OnTimelineFinished;
        }
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        SceneManager.LoadScene(nextScene);
    }

    void LoadNext()
    {
        SceneManager.LoadScene(nextScene);
    }
}
