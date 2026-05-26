using UnityEngine;
using UnityEngine.UI;

public class SliderToggleController : MonoBehaviour
{
    public Toggle toggle;
    public Slider slider;

    private float _previousValue;

    void Awake() // Awake ensures listener is ready before any Start() fires
    {
        _previousValue = slider.value;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn) // Toggle ON → mute slider
        {
            _previousValue = slider.value;
            slider.value = -80f;
            slider.interactable = false;
        }
        else // Toggle OFF → restore slider
        {
            slider.interactable = true;
            slider.value = _previousValue;
        }
    }
}