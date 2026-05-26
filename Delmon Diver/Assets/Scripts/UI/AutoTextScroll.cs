using UnityEngine;

public class CreditsAutoScroll : MonoBehaviour
{
    public float speed = 30f;
    public float delay = 0.7f;

    public RectTransform content;     // Content (moving)
    public RectTransform viewport;    // View area

    private bool startScrolling = false;
    private Vector2 startPosition;

    void Start()
    {
        startPosition = content.anchoredPosition;
        ResetCredits();
    }

    void OnEnable()
    {
        ResetCredits();
    }

    void ResetCredits()
    {
        content.anchoredPosition = startPosition;
        startScrolling = false;
        CancelInvoke();
        Invoke(nameof(StartScrolling), delay);
    }

    void StartScrolling()
    {
        startScrolling = true;
    }

    public void RestartCredits()
    {
        ResetCredits();
    }

    void Update()
    {
        if (!startScrolling) return;

        content.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        float contentBottom = content.anchoredPosition.y;
        float maxScroll = content.rect.height - viewport.rect.height;

        if (contentBottom >= maxScroll)
        {
            startScrolling = false;
        }
    }
}