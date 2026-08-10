using UnityEngine;

public class ForestDragonAudioManager : MonoBehaviour
{
    public AudioSource MovementAudioSource;
    public AudioSource WingAudioSource;
    public AudioSource VoiceAudioSource;

    public AudioClip[] FootSteps;
    public AudioClip[] WingFlap;
    public AudioClip[] Breath;
    public AudioClip[] Bite;
    public AudioClip[] Roar;
    public AudioClip[] Hurt;
    public AudioClip[] Death;

    public void PlayFootstep()
    {
        if (FootSteps.Length == 0)
            return;

        int index = Random.Range(0, FootSteps.Length);

        MovementAudioSource.PlayOneShot(
            FootSteps[index],
            0.5f
        );
    }
    public void PlayBreath()
    {
        return;
        //if (Breath.Length == 0)
        //    return;

        //int index = Random.Range(0, Breath.Length);

        //VoiceAudioSource.PlayOneShot(
        //    Breath[index],
        //    1f
        //);
    }
    public void PlayWingFlap()
    {
        if (WingFlap.Length == 0)
            return;

        int index = Random.Range(0, WingFlap.Length);

        WingAudioSource.PlayOneShot(
            WingFlap[index],
            1f
        );
    }
    public void PlayBite()
    {
        if (Bite.Length == 0)
            return;

        int index = Random.Range(0, Bite.Length);

        VoiceAudioSource.PlayOneShot(
            Bite[index],
            1f
        );
    }
    public void PlayRoar()
    {
        if (Roar.Length == 0)
            return;

        int index = Random.Range(0, Roar.Length);

        VoiceAudioSource.PlayOneShot(
            Roar[index],
            1f
        );
    }
    public void PlayHurt()
    {
        if (Hurt.Length == 0)
            return;

        int index = Random.Range(0, Hurt.Length);

        VoiceAudioSource.PlayOneShot(
            Hurt[index],
            1f
        );
    }
    public void PlayDeath()
    {
        if (Death.Length == 0)
            return;

        int index = Random.Range(0, Death.Length);

        VoiceAudioSource.PlayOneShot(
            Death[index],
            1f
        );
    }
}

