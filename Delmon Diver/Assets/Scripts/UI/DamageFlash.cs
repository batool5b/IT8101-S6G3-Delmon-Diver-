using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    private Image img;
    private float timer = 0f;
    private float duration = 0.3f;

    private void Awake() 
    { 
        img = GetComponent<Image>(); 
    }
    
    public void Flash() 
    { 
        timer = duration; 
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (img != null)
            {
                img.color = new Color(1, 0, 0, (timer / duration) * 0.4f);
            }
        }
        else if (img != null && img.color.a > 0)
        {
            img.color = new Color(1, 0, 0, 0);
        }
    }
}
