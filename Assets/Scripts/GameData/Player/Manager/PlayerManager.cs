using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInfo Player;
    public string BaseLayerAnimName = "Base Layer";

    public event Action BeingHit;
    public event Action DoneBeingHit;
    public event Action OnGetHit;

    private readonly int _animHurt = Animator.StringToHash("Hurt");

    private PlayerStatsManager _stats;
    private HealthSystem _healthSystem;

    private Animator _animator;
    private Coroutine _hitCoroutine = null;

    private void Awake()
    {
        Player.PlayerTransform = this.transform;
        _stats = GetComponent<PlayerStatsManager>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnTakeDmg += GetHit;
        }
    }
    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnTakeDmg -= GetHit;
        }
    }

    public void GetHit(DmgInfo dmgInfo)
    {
        OnGetHit?.Invoke();
        if (_stats.IsRunOutPoise(dmgInfo.PoiseDamage))
        {
            if (_hitCoroutine != null) StopCoroutine(_hitCoroutine);
            _hitCoroutine = StartCoroutine(BeHitCoroutine());
        }
    }

    private IEnumerator BeHitCoroutine()
    {
        BeingHit?.Invoke();
        _animator.CrossFade(_animHurt, 0.1f);
        yield return new WaitForSeconds(0.2f);

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        float waitTime = stateInfo.length - 0.1f;

        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        DoneBeingHit?.Invoke();
        _stats.RecoverPoise();
    }
}
