using UnityEngine;
using UnityEngine.UI;

public class ToggleVisibilityController : MonoBehaviour
{
    public Toggle toggleA;
    public Toggle toggleB;

    public GameObject elementA;
    public GameObject elementB;

    private void OnEnable()
    {
        toggleA.onValueChanged.AddListener(OnToggleAChanged);
        toggleB.onValueChanged.AddListener(OnToggleBChanged);

        // Use SetIsOnWithoutNotify to set initial state without triggering listeners
        toggleA.SetIsOnWithoutNotify(true);
        toggleB.SetIsOnWithoutNotify(false);
        elementA.SetActive(true);
        elementB.SetActive(false);
    }

    private void OnDisable()
    {
        toggleA.onValueChanged.RemoveListener(OnToggleAChanged);
        toggleB.onValueChanged.RemoveListener(OnToggleBChanged);
    }

    private void OnToggleAChanged(bool isOn)
    {
        if (!isOn)
        {
            toggleA.SetIsOnWithoutNotify(true);
            return;
        }

        toggleB.SetIsOnWithoutNotify(false);
        elementA.SetActive(true);
        elementB.SetActive(false);
    }

    private void OnToggleBChanged(bool isOn)
    {
        if (!isOn)
        {
            toggleB.SetIsOnWithoutNotify(true);
            return;
        }

        toggleA.SetIsOnWithoutNotify(false);
        elementA.SetActive(false);
        elementB.SetActive(true);
    }
}