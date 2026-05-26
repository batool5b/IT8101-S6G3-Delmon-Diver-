using UnityEngine;
using System.Collections;
using TMPro;

public class ParrotRandomFacts : MonoBehaviour
{
    public Transform player; //player reference

    [Header("Fact Time")]
    public float minFactTime = 30f; //minimum time between facts
    public float maxFactTime = 100f; //maximum time between facts
    public float messageDuration = 5f; //how long text stays on screen

    [Header("UI")]
    public GameObject dialoguePanel; //panel that contains the text
    public TMP_Text dialogueText; //text that shows parrot messages

    [Header("Sound")]
    public AudioSource audioSource; //audio source on parrot
    public AudioClip parrotSound;

    [Header("Distance Reaction")]
    public float farDistance = 7f; //when player is far
    public float closeDistance = 5f; //when player is close again

    [Header("Tag Reaction")]
    public bool enableTagReaction = true; //turn tag reaction on/off
    public float tagDetectionRadius = 6f; //how close parrot must be to object
    public float tagReactionCooldown = 6f; //time before another tag reaction can happen

    private float nextTagReactionTime = 0f;

    //last indexes
    private int lastFactIndex = -1; //last fact
    private int lastReactionIndex = -1; //last far reaction

    private bool alreadyReactedToFar = false; //checks if parrot already talked
    private Coroutine hideMessageCoroutine;

    public string[] facts =
    {
        "Did you know? Bahrain was once home to the ancient Dilmun civilization.",
        "Did you know? Bahrain is an archipelago made up of many islands.",
        "Did you know? Pearl diving was once a major tradition and source of income.",
        "Did you know? The Tree of Life is a rare living tree found in Bahrain’s desert.",
        "Did you know? Bahrain has a rich pearl-diving history from the days of natural pearls.",
        "Did you know? Bahrain is famous for its traditional textile and craftwork.",
        "Did you know? Dilmun is believed to be mentioned in some of the world’s oldest stories.",
        "Did you know? Pearl diving shaped Bahrain’s history and economy for centuries.",
        "Did you know? Bahrain’s pearl-diving era left stories and traditions that still influence culture today.",
        "Did you know? Traditional crafts in Bahrain were passed down through generations of artisans.",
        "Did you know? Bahrain was one of the first places in the Gulf to discover oil.",
        "Did you know? Before oil, Bahrain’s economy depended heavily on the sea.",
        "Did you know? The Tree of Life can survive without an obvious water source.",
        "Did you know? Bahrain’s location made it an important trading center in the Gulf.",
        "Did you know? Muharraq was once the capital of Bahrain before Manama.",
        "Did you know? Bahrain has over 30 natural and man-made islands.",
        "Did you know? The Dilmun civilization made Bahrain a key trade hub in ancient times.",
        "Did you know? Traditional Bahraini houses were designed to stay cool in hot weather.",
        "Did you know? The pearl industry declined after the discovery of oil.",
        "Did you know? Bahrain has a history that goes back more than 4,000 years."
    };

    public string[] reactionLines =
    {
        "Hey! Wait for me!",
        "You are getting too far!",
        "Slow down, I am coming!",
        "Do not leave me behind!",
        "I will fly closer to you!",
        "Stay close, I have more facts to tell you!",
        "Careful, I am following you!",
        "Wait! I am still here!"
    };

    [Header("Object Tag Reaction Lines")]
    public string[] obstacleLines =
    {
        "Squawk! Careful, there is an obstacle ahead!",
        "Watch out! That could damage the boat!",
        "Stay sharp, the sea is not empty!"
    };

    public string[] seaCreatureLines =
    {
        "Squawk! A sea creature is nearby!",
        "Careful! Something is moving in the water!",
        "Stay alert, I see a creature near us!"
    };

    public string[] islandLines =
    {
        "Squawk! I can see land ahead!",
        "That island looks important!",
        "We are getting closer to the island!"
    };

    void Start()
    {
        //hide UI when game starts
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        StartCoroutine(RandomFactLoop());
    }

    void Update()
    {
        if (player == null) return;

        //distance between parrot and player
        float distance = Vector3.Distance(transform.position, player.position);

        //if player becomes far, parrot talks once
        if (distance >= farDistance && alreadyReactedToFar == false)
        {
            SayRandomReaction();
            alreadyReactedToFar = true;
        }

        //if player comes close again, allow parrot to react next time
        if (distance <= closeDistance)
        {
            alreadyReactedToFar = false;
        }

        //check nearby tagged objects
        if (enableTagReaction)
        {
            CheckNearbyTaggedObjects();
        }
    }

    public void SayRandomFact()
    {
        if (facts.Length == 0) return;

        int randomIndex;
        int attempts = 0;

        //prevent same fact twice
        do
        {
            randomIndex = Random.Range(0, facts.Length);
            attempts++;
        }
        while (randomIndex == lastFactIndex && attempts < 10);

        lastFactIndex = randomIndex;

        string fact = facts[randomIndex];

        ShowMessage(fact);
        Debug.Log("Balbol: " + fact);

        PlaySound();
    }

    public void SayRandomReaction()
    {
        if (reactionLines.Length == 0) return;

        int randomIndex;
        int attempts = 0;

        //prevent same reaction twice
        do
        {
            randomIndex = Random.Range(0, reactionLines.Length);
            attempts++;
        }
        while (randomIndex == lastReactionIndex && attempts < 10);

        lastReactionIndex = randomIndex;

        string reaction = reactionLines[randomIndex];

        ShowMessage(reaction);
        Debug.Log("[Balbol] " + reaction);

        PlaySound();
    }

    void CheckNearbyTaggedObjects()
    {
        if (Time.time < nextTagReactionTime) return;

        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, tagDetectionRadius);

        foreach (Collider obj in nearbyObjects)
        {
            if (obj.CompareTag("SeaCreature"))
            {
                SayRandomLineFromArray(seaCreatureLines);
                nextTagReactionTime = Time.time + tagReactionCooldown;
                return;
            }

            if (obj.CompareTag("Obstacle"))
            {
                SayRandomLineFromArray(obstacleLines);
                nextTagReactionTime = Time.time + tagReactionCooldown;
                return;
            }

            if (obj.CompareTag("Island"))
            {
                SayRandomLineFromArray(islandLines);
                nextTagReactionTime = Time.time + tagReactionCooldown;
                return;
            }
        }
    }

    void SayRandomLineFromArray(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        int randomIndex = Random.Range(0, lines.Length);
        string line = lines[randomIndex];

        ShowMessage(line);
        Debug.Log("[Balbol] " + line);

        PlaySound();
    }

    void ShowMessage(string message)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        if (dialogueText != null)
        {
            dialogueText.text = "[Balbol] " + message;
        }

        //restart hide timer each time the parrot talks
        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }

        hideMessageCoroutine = StartCoroutine(HideMessageAfterDelay());
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    void PlaySound()
    {
        if (audioSource != null && parrotSound != null)
        {
            audioSource.PlayOneShot(parrotSound);
        }
    }

    IEnumerator RandomFactLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFactTime, maxFactTime));

            SayRandomFact();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tagDetectionRadius);
    }
}