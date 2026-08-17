using Unity.VisualScripting;
using UnityEngine;

public class VFXBossAttack : MonoBehaviour
{
    public SoundConfigSO SFXSound;
    public float SoundRange = 33;

    private BossCombatInfo _combatInfo;
    private BossStatsManager _stats;
    private bool _activate;
    private CooldownTimer timer;
    private DmgInfo _dmgInfo;
    private HealthSystem _playerHealth;
    private void OnEnable()
    {
        timer = null;
    }
    private void OnDisable()
    {
        _playerHealth = null;
        _activate = false;
        timer = null;
    }
    private void Update()
    {
        if (!_activate) return;
        if (timer == null) return;
        if (timer.Tick())
        {
            if (_playerHealth != null)
            {
                _playerHealth.TakeDmg(_dmgInfo);
            }
        }
    }
    public void VFXSetSkillInfo(BossCombatInfo info, BossStatsManager stats)
    {
        _combatInfo = info;
        _stats = stats;
        float tick = info.TimeHitPerNumberSecond;
        if (tick >= 0)
        {
            timer = new CooldownTimer(tick);
        }
        if (SFXSound == null) return;
        AudioManager.Instance.PlaySFX(SFXSound, transform.position, SoundRange);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            _playerHealth = other.GetComponent<HealthSystem>();

            if (_playerHealth != null)
            {
                Debug.Log("Deall: " + _dmgInfo.Amount);
                SetCurrentDmgInfo();
                _playerHealth.TakeDmg(_dmgInfo);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            _playerHealth = null;
            _activate = false;
            timer = null;
        }
    }
    private void SetCurrentDmgInfo()
    {
        _dmgInfo.Amount = _stats.AttackPower.GetValue() * _combatInfo.DmgHitMultiple;
        _dmgInfo.PoiseDamage = _stats.PoiseDamage.GetValue();
    }
}
