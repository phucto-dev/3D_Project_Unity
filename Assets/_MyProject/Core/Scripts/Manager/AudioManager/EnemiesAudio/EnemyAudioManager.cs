using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [Header("--- SOUND CONFIG ---")]
    public SoundConfigSO AttackSound;
    public SoundConfigSO FootstepSound;
    public SoundConfigSO HurtSound;
    public SoundConfigSO DeathSound;

    [Header("--- SOUND POSITION ---")]
    public Transform FootPosition;
    public Transform MouthPosition;

    public void PlayFootstepSound()
    {
        if (FootPosition == null) return;
        if (FootstepSound == null) return;

        AudioManager.Instance.PlaySFX(FootstepSound, FootPosition.position);
    }
    public void PlayAttackSound()
    {
        if (AttackSound == null) return;
        if (MouthPosition == null) return;

        AudioManager.Instance.PlaySFX(AttackSound, MouthPosition.position);
    }
    public void PlayHurtSound()
    {
        if (HurtSound == null) return;
        if (MouthPosition == null) return;

        AudioManager.Instance.PlaySFX(HurtSound, MouthPosition.position);
    }
    public void PlayDeathSound()
    {
        if (DeathSound == null) return;
        if (MouthPosition == null) return;

        AudioManager.Instance.PlaySFX(DeathSound, MouthPosition.position);
    }
}
