using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Level1SequenceManager : MonoBehaviour
{
    [Header("Objectives")]
    public int totalItemsToCollect = 3;
    public int collectedItemsCount = 0;
    public bool reachedSurface = false;

    [Header("UI References")]
    public TextMeshProUGUI objectiveText;
    public GameObject winPanel;
    public GameObject losePanel;

    private void Start()
    {
        UpdateObjectiveUI("Bottom of the sea. Retrieve your 3 essential items from the wreckage!");
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    public void ItemCollected()
    {
        collectedItemsCount++;
        if (collectedItemsCount < totalItemsToCollect)
        {
            UpdateObjectiveUI($"Items: {collectedItemsCount}/{totalItemsToCollect}. Keep searching the wreckage!");
        }
        else
        {
            UpdateObjectiveUI("All items retrieved! SWIM TO THE SURFACE NOW!");
        }
    }

    public void ReachSurface()
    {
        if (collectedItemsCount >= totalItemsToCollect)
        {
            reachedSurface = true;
            UpdateObjectiveUI("Escaped! Level 1 Complete.");
            if (winPanel) winPanel.SetActive(true);
            Debug.Log("Level 1 Complete!");
        }
        else
        {
            UpdateObjectiveUI("You can't leave without your items! Go back down!");
        }
    }

    public void PlayerDrowned()
    {
        UpdateObjectiveUI("You ran out of air...");
        if (losePanel) losePanel.SetActive(true);
    }

    private void UpdateObjectiveUI(string newText)
    {
        if (objectiveText != null)
        {
            objectiveText.text = "Objective: " + newText;
        }
        Debug.Log("Objective Update: " + newText);
    }
}
