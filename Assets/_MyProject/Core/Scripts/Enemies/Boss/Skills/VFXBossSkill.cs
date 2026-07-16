using UnityEngine;

public class VFXBossSkill : MonoBehaviour
{
    public PoolItemSO HitVFX;

    private BossCombatInfo _info;
    private ParticleNotifier _notifier;
    private void Awake()
    {
        _notifier = GetComponentInChildren<ParticleNotifier>();
    }
    private void OnEnable()
    {
        if (_notifier != null) _notifier.OnStopped += OnSkillEnd;
    }
    private void OnDisable()
    {
        if (_notifier != null) _notifier.OnStopped -= OnSkillEnd;
    }

    public void SetUp(BossCombatInfo info)
    {
        _info = info;
    }

    public void OnSkillEnd()
    {
        PoolManager.Instance.Release(_info.VFXID, gameObject);
    }
}
