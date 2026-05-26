using UnityEngine;
using TMPro;

public class DangerUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject dangerPanel; 
    public TextMeshProUGUI dangerText; 

    [Header("Message")]
    public string message = "DANGER! Sea creature nearby!";

    void Start()
    {
        //hide danger UI at the start
        HideDanger();
    }

    public void ShowDanger()
    {
        if (dangerPanel != null)
            dangerPanel.SetActive(true);

        if (dangerText != null)
            dangerText.text = message;
    }

    public void HideDanger()
    {
        if (dangerPanel != null)
            dangerPanel.SetActive(false);
    }
}
