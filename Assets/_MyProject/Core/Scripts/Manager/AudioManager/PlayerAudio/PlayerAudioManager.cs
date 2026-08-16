using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioSource MovementAudioSource;
    public AudioSource VoiceAudioSource;
    public AudioSource SkillAudioSource;

    public AudioClip[] footSteps;
    public AudioClip[] jumpStep;
    public AudioClip[] hurt;

    public void PlayFootstep()
    {
        if (footSteps.Length == 0)
            return;

        int index = Random.Range(0, footSteps.Length);

        MovementAudioSource.PlayOneShot(
            footSteps[index],
            0.5f
        );
    }
    public void PlayJumpstep()
    {
        if (jumpStep.Length == 0)
            return;

        int index = Random.Range(0, jumpStep.Length);

        MovementAudioSource.PlayOneShot(
            jumpStep[index],
            0.5f
        );
    }
    public void PlayHurtSound()
    {
        if (hurt.Length == 0)
            return;

        int index = Random.Range(0, hurt.Length);

        VoiceAudioSource.PlayOneShot(
            hurt[index],
            1f
        );
    }
    public void PlaySkill(SoundConfigSO skillSound)
    {
        if (skillSound == null) return;

        AudioClip clipToPlay = skillSound.GetRandomClip();
        if (clipToPlay == null) return;

        SkillAudioSource.volume = skillSound.Volume;
        SkillAudioSource.pitch = Random.Range(skillSound.PitchMin, skillSound.PitchMax);

        if (skillSound.PlaybackType == SoundPlaybackType.Loop)
        {
            SkillAudioSource.loop = true;
            SkillAudioSource.clip = clipToPlay;
            SkillAudioSource.Play();
        }
        else
        {
            SkillAudioSource.loop = false;
            SkillAudioSource.PlayOneShot(clipToPlay, skillSound.Volume);
        }
    }
    public void StopSkillLoop()
    {
        if (SkillAudioSource.isPlaying && SkillAudioSource.loop)
        {
            SkillAudioSource.Stop();
            SkillAudioSource.loop = false;
        }
    }
}
