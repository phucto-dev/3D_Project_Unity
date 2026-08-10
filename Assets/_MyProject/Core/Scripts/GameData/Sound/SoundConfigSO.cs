using UnityEngine;

public enum SoundPlaybackType
{
    OneShot,
    Loop
}

public enum SoundAttachType
{
    Caster,
    SkillObject,
    WorldPosition
}

[CreateAssetMenu(fileName = "NewSound", menuName = "GameData/Audio/Sound Config")]
public class SoundConfigSO : ScriptableObject
{
    [Header("--- CLIPS ---")]
    public AudioClip[] Clips;

    [Header("--- PLAYBACK ---")]
    public SoundPlaybackType PlaybackType = SoundPlaybackType.OneShot;

    public SoundAttachType AttachType = SoundAttachType.Caster;

    [Header("--- AUDIO SETTINGS ---")]
    [Range(0f, 1f)]
    public float Volume = 1f;

    [Range(0.1f, 3f)]
    public float PitchMin = 0.9f;

    [Range(0.1f, 3f)]
    public float PitchMax = 1.1f;

    public AudioClip GetRandomClip()
    {
        if (Clips == null || Clips.Length == 0)
            return null;

        return Clips[
            Random.Range(0, Clips.Length)
        ];
    }
}
