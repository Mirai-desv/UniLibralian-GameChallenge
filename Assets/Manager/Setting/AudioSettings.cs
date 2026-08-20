using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SFXSlider;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    void Start()
    {
        // Load giá trị đã lưu, mặc định là 0.75 (75%)
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_KEY, 0.75f);
        float savedSFX = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        MusicSlider.value = savedMusic;
        SFXSlider.value = savedSFX;

        SetMusicVolume(savedMusic);
        SetSFXVolume(savedSFX);

        MusicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        // Slider chạy từ 0.0001 -> 1, convert sang decibel
        audioMixer.SetFloat(MUSIC_KEY, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFX_KEY, Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }
}