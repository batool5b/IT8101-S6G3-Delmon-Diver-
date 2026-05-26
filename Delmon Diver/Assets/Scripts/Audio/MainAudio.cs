using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainAudio : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        LoadVolume();
        MusicManager.Instance.PlayMusic("Ocean");
    }

public void LoadVolume()
{
    musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);
    sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0f);

    musicToggle.isOn = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
    sfxToggle.isOn = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

    // Apply to AudioMixer directly using saved slider values
    audioMixer.SetFloat("MusicVolume", musicToggle.isOn ? -80f : musicSlider.value);
    audioMixer.SetFloat("SFXVolume", sfxToggle.isOn ? -80f : sfxSlider.value);

    musicToggle.onValueChanged.Invoke(musicToggle.isOn);
    sfxToggle.onValueChanged.Invoke(sfxToggle.isOn);
}
}