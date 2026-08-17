using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("--- AUDIO MIXERS ---")]
    public AudioMixerGroup BgmMixerGroup;
    public AudioMixerGroup SfxMixerGroup;

    [Header("--- POOL SETTINGS ---")]
    public int SfxPoolSize = 15;

    [Header("--- BGM ---")]
    public SoundConfigSO BGM;
    public SoundConfigSO RespawnScreenBGM;

    private List<AudioSource> _sfxPool;
    private AudioSource _bgmSource;
    private SoundConfigSO _currentBGM;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        InitializeSystem();
    }
    private void OnEnable()
    {
        PlayDefaultBGM();
    }
    private void InitializeSystem()
    {
        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.outputAudioMixerGroup = BgmMixerGroup;
        _bgmSource.loop = true;
        _bgmSource.spatialBlend = 0f;

        _sfxPool = new List<AudioSource>();

        GameObject poolContainer = new GameObject("SFX_Pool_Container");
        poolContainer.transform.SetParent(this.transform);

        for (int i = 0; i < SfxPoolSize; i++)
        {
            GameObject sfxObj = new GameObject($"SFX_Source_{i}");
            sfxObj.transform.SetParent(poolContainer.transform);

            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = SfxMixerGroup;
            source.playOnAwake = false;

            _sfxPool.Add(source);
        }
    }
    public void PlaySFX(SoundConfigSO soundConfig, Vector3? position = null, float maxDistance = 100f)
    {
        if (soundConfig == null) return;

        AudioClip clipToPlay = soundConfig.GetRandomClip();
        if (clipToPlay == null) return;

        AudioSource source = GetAvailableSFXSource();

        source.clip = clipToPlay;
        source.volume = soundConfig.Volume;
        source.pitch = Random.Range(soundConfig.PitchMin, soundConfig.PitchMax);
        source.loop = false;

        if (position.HasValue)
        {
            source.spatialBlend = 1f;
            source.transform.position = position.Value;

            source.rolloffMode = AudioRolloffMode.Custom;
            source.maxDistance = maxDistance;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();
    }
    public AudioSource PlaySFXLoop(SoundConfigSO soundConfig, Vector3? position = null)
    {
        if (soundConfig == null) return null;

        AudioClip clipToPlay = soundConfig.GetRandomClip();
        if (clipToPlay == null) return null;

        AudioSource source = GetAvailableSFXSource();

        source.clip = clipToPlay;
        source.volume = soundConfig.Volume;
        source.pitch = Random.Range(soundConfig.PitchMin, soundConfig.PitchMax);

        source.loop = true;

        if (position.HasValue)
        {
            source.spatialBlend = 1f;
            source.transform.position = position.Value;
        }
        else
        {
            source.spatialBlend = 0f;
        }

        source.Play();

        return source;
    }
    public void StopSFXLoop(AudioSource loopSource)
    {
        if (loopSource != null && loopSource.isPlaying)
        {
            loopSource.Stop();

            loopSource.loop = false;
            loopSource.clip = null;
        }
    }
    public void PlayBGM(SoundConfigSO bgmConfig)
    {
        if (bgmConfig == null) return;

        if (_currentBGM == bgmConfig && _bgmSource.isPlaying) return;

        _currentBGM = bgmConfig;

        AudioClip clipToPlay = bgmConfig.GetRandomClip();
        if (clipToPlay == null) return;

        _bgmSource.clip = clipToPlay;
        _bgmSource.volume = bgmConfig.Volume;
        _bgmSource.pitch = 1f;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
        _currentBGM = null;
    }

    private AudioSource GetAvailableSFXSource()
    {
        for (int i = 0; i < _sfxPool.Count; i++)
        {
            if (!_sfxPool[i].isPlaying)
            {
                return _sfxPool[i];
            }
        }
        return _sfxPool[0];
    }
    public void ToggleMuteAll(bool isMute)
    {
        AudioListener.pause = isMute;
    }
    public void PlayDefaultBGM()
    {
        if (BGM != null) PlayBGM(BGM);
    }
    public void PlayRespawnScreenBGM()
    {
        if (RespawnScreenBGM != null) PlayBGM(RespawnScreenBGM);
    }

}
