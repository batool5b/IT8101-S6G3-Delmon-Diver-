using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
 
public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider Slider;

    public void Quit()
    {
        Application.Quit();
    }
 
    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("S", volume);
    }

}