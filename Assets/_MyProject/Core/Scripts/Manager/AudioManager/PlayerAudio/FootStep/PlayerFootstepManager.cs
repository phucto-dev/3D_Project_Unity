using UnityEngine;

public class PlayerFootstepManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] footSteps;
    public AudioClip[] jumpStep;

    public void PlayFootstep()
    {
        if (footSteps.Length == 0)
            return;

        int index = Random.Range(0, footSteps.Length);

        audioSource.PlayOneShot(
            footSteps[index],
            0.5f
        );
    }
    public void PlayJumpstep()
    {
        if (jumpStep.Length == 0)
            return;

        int index = Random.Range(0, jumpStep.Length);

        audioSource.PlayOneShot(
            jumpStep[index],
            0.5f
        );
    }
}
