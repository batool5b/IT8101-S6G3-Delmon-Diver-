using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public Slider oxygenSlider;
    public PlayerOxygen playerOxygen;

    private void Start()
    {
        if (playerOxygen == null)
            playerOxygen = Object.FindAnyObjectByType<PlayerOxygen>();

        if (playerOxygen != null)
        {
            Debug.Log($"[HUDManager] Linked to PlayerOxygen on {playerOxygen.gameObject.name}");
            if (oxygenSlider != null)
            {
                playerOxygen.OnOxygenChanged.AddListener(UpdateOxygenSlider);
                Debug.Log($"[HUDManager] Linked to OxygenSlider {oxygenSlider.name}");
            }
            
            playerOxygen.OnDrowned.AddListener(OnPlayerDrowned);
        }
        else
        {
            Debug.LogError("[HUDManager] Could not find PlayerOxygen in scene.");
        }
    }

    private void UpdateOxygenSlider(float percent)
    {
        if (oxygenSlider != null)
        {
            oxygenSlider.value = percent;
            // Debug.Log($"HUD: Oxygen updated to {percent}");
        }
    }

    private void OnPlayerDrowned()
    {
        var manager = Object.FindAnyObjectByType<Level1SequenceManager>();
        if (manager != null)
        {
            manager.PlayerDrowned();
        }
    }

    private void OnDestroy()
    {
        if (playerOxygen != null)
        {
            playerOxygen.OnOxygenChanged.RemoveListener(UpdateOxygenSlider);
            playerOxygen.OnDrowned.RemoveListener(OnPlayerDrowned);
        }
    }
}
