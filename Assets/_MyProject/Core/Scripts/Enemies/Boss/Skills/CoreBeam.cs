using UnityEngine;

public class CoreBeam : MonoBehaviour
{
    public PoolItemSO HitVFX;

    private BossCombatInfo _info;
    private ParticleNotifier _notifier;
    private CooldownTimer _timer;
    private bool _targetInside;
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
    private void Update()
    {
        if (_timer == null) return;
        if (!_timer.Tick()) return;

        //

    }
    public void SetUp(BossCombatInfo info)
    {
        _info = info;
        _timer = new CooldownTimer(info.TimeHitPerNumberSecond);
    }

    public void OnSkillEnd()    
    {
        Debug.Log("VFXID: " + _info.VFXID);
        PoolManager.Instance.Release(_info.VFXID, gameObject);
    }
}
