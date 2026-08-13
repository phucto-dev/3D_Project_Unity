using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettingsSlider : MonoBehaviour
{
    [Header("--- AUDIO MIXER ---")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        float bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        _bgmSlider.value = bgmVolume;
        _sfxSlider.value = sfxVolume;

        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);

        _bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        _audioMixer.SetFloat("BGMVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat(BGM_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFXVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    private float LinearToDecibel(float value)
    {
        if (value <= 0.0001f)
            return -80f;

        return Mathf.Log10(value) * 20f;
    }

    private void OnDestroy()
    {
        _bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        _sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
