using UnityEngine;
using UnityEngine.SceneManagement; //needed to load the next scene

public class LoadLevel4 : MonoBehaviour
{
    [Header("Next Level")]
    public string nextSceneName; //name of the next scene

    [Header("Level Complete UI")]
    public GameObject levelCompletePanel; //panel shown when level is complete
    public float delayBeforeLoad = 2f; //delay before loading next scene

    private bool levelFinished = false; //prevents finishing more than once

    void Start()
    {
        //hide the panel at the start
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //check when the boat enters the finish trigger
        CheckFinish(other);
    }

    private void OnTriggerStay(Collider other)
    {
        //also works if the boat starts already inside the trigger
        CheckFinish(other);
    }

    void CheckFinish(Collider other)
    {
        //do nothing if level already finished
        if (levelFinished) return;

        //check if the boat or its main parent has the Player tag
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            levelFinished = true;

            //show level complete UI
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }

            //load next scene after delay
            Invoke(nameof(LoadNextLevel), delayBeforeLoad);
        }
    }

    void LoadNextLevel()
    {
        //load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}