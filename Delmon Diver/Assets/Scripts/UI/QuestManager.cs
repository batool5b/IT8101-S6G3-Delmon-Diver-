using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string description;
        public int currentProgress;
        public int targetAmount;
        public bool isCompleted;

        [HideInInspector]
        public QuestItemUI uiRow;
    }

    [Header("Prefab & Container")]
    public GameObject questRowPrefab;
    public Transform questListContainer;

    [Header("Quest List")]
    public List<Quest> activeQuests = new List<Quest>();

    [Header("Global Style Settings")]
    public Sprite uncheckedSprite; 
    public Sprite checkedSprite;   
    public Color activeColor = Color.white;
    public Color completedColor = new Color(0.5f, 1f, 0.5f); 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        yield return null; 
        SpawnAndRefreshUI();
    }

    [ContextMenu("Refresh UI")]
    public void SpawnAndRefreshUI()
    {
        if (questRowPrefab == null || questListContainer == null) return;

        // Clear old rows
        for (int i = questListContainer.childCount - 1; i >= 0; i--)
        {
            GameObject child = questListContainer.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        // Spawn rows
        foreach (Quest q in activeQuests)
        {
            GameObject row = Instantiate(questRowPrefab, questListContainer, false);
            
            RectTransform rect = row.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 60f); 
            }

            q.uiRow = row.GetComponent<QuestItemUI>();

            if (q.uiRow != null)
            {
                q.uiRow.Refresh(
                    q.description,
                    q.currentProgress,
                    q.targetAmount,
                    q.isCompleted,
                    uncheckedSprite,
                    checkedSprite,
                    activeColor,
                    completedColor
                );
            }
            else
            {
                Debug.LogError($"[QuestManager] '{row.name}' lacks a 'QuestItemUI' component!", this);
            }
        }

        Canvas.ForceUpdateCanvases();

        // Rebuild layout hierarchy
        for (int i = 0; i < questListContainer.childCount; i++)
        {
            RectTransform childRect = questListContainer.GetChild(i).GetComponent<RectTransform>();
            if (childRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(childRect);
            }
        }

        RectTransform containerRect = questListContainer.GetComponent<RectTransform>();
        if (containerRect != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);
        }
    }

    public void AddProgress(string questId, int amount = 1)
    {
        Quest q = activeQuests.Find(x => x.questId == questId);
        if (q == null) return;
        if (q.isCompleted) return;

        q.currentProgress = Mathf.Min(q.currentProgress + amount, q.targetAmount);

        if (q.currentProgress >= q.targetAmount)
        {
            q.isCompleted = true;
        }

        if (q.uiRow != null)
        {
            q.uiRow.Refresh(
                q.description,
                q.currentProgress,
                q.targetAmount,
                q.isCompleted,
                uncheckedSprite,
                checkedSprite,
                activeColor,
                completedColor
            );
        }
    }
}