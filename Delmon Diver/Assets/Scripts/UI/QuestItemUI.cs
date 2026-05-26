using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text descriptionText;
    public TMP_Text progressText;
    public Image checkImage;

    [Header("Optional Debug")]
    public bool debugLogs = true;

    public void Refresh(
        string description,
        int current,
        int target,
        bool completed,
        Sprite uncheckedSprite,
        Sprite checkedSprite,
        Color activeColor,
        Color completedColor)
    {
        // ==============================
        // SAFETY CHECKS
        // ==============================
        if (descriptionText == null)
        {
            Debug.LogError($"[{name}] Description Text is NULL! Please assign it in the Inspector.", this);
            return;
        }

        // Force root row active and reset scale
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        // ==============================
        // CHECK IMAGE
        // ==============================
        if (checkImage != null)
        {
            checkImage.gameObject.SetActive(true);
            checkImage.enabled = true;

            if (uncheckedSprite != null && checkedSprite != null)
            {
                checkImage.sprite = completed ? checkedSprite : uncheckedSprite;
            }
        }

        // ==============================
        // DESCRIPTION TEXT (Includes Count & Target)
        // ==============================
        descriptionText.gameObject.SetActive(true);
        descriptionText.enabled = true;
        
        // Fix: Appending target and current count directly into the description
        descriptionText.text = $"{description}";
        descriptionText.color = completed ? completedColor : activeColor;
        
        descriptionText.ForceMeshUpdate(true);

        // ==============================
        // PROGRESS TEXT (Separate Right-Aligned Label)
        // ==============================
        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            progressText.enabled = true;

            progressText.text = $"{current} / {target}";
            progressText.color = completed ? completedColor : activeColor;
            
            progressText.ForceMeshUpdate(true);

            // Anti-Crush Safeguard: Force a layout priority if layout elements exist
            LayoutElement progLayout = progressText.GetComponent<LayoutElement>();
            if (progLayout != null)
            {
                progLayout.enabled = true;
                if (progLayout.minWidth <= 0) progLayout.minWidth = 60f; // Stops it from shrinking to 0 width
            }
        }

        if (debugLogs)
        {
            Debug.Log($"[{name}] UI Refreshed -> Desc: {descriptionText.text}");
        }
    }
}