using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    private void OnEnable()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("SoundVolume", soundSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.Save();
    }
}