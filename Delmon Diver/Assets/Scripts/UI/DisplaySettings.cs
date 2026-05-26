using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DisplaySettings : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullScreenToggle;

    Resolution[] AllResolutions;
    bool isFullScreen;
    int SelectedResolution;

    List<Resolution> SelectedResolutionList = new List<Resolution>();

    void Start()
    {
        AllResolutions = Screen.resolutions;

        SelectedResolutionList.Clear();
        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
        }

        ResDropDown.ClearOptions();
        ResDropDown.AddOptions(resolutionStringList);
        LoadSettings();
    }

    public void changeResolution()
    {
        if (SelectedResolutionList == null || SelectedResolutionList.Count == 0) return;

        SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, isFullScreen);
        SaveSettings();
    }

    public void ChangeFullScreen()
    {
        if (SelectedResolutionList == null || SelectedResolutionList.Count == 0) return;

        isFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, isFullScreen);
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("Resolution", SelectedResolution);
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        isFullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1;
        FullScreenToggle.SetIsOnWithoutNotify(isFullScreen);

        SelectedResolution = PlayerPrefs.GetInt("Resolution", SelectedResolutionList.Count - 1);
        ResDropDown.SetValueWithoutNotify(SelectedResolution);
        ResDropDown.RefreshShownValue();

        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, isFullScreen);
    }
}