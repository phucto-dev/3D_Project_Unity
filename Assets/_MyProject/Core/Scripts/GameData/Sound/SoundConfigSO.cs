using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "GameData/Audio/Sound Config")]
public class SoundConfigSO : ScriptableObject
{
    public AudioClip[] Clips;

    [Range(0f, 1f)] public float Volume = 1f;
    [Range(0.1f, 3f)] public float PitchMin = 0.9f;
    [Range(0.1f, 3f)] public float PitchMax = 1.1f;

    public AudioClip GetRandomClip()
    {
        if (Clips == null || Clips.Length == 0) return null;
        return Clips[Random.Range(0, Clips.Length)];
    }
}
