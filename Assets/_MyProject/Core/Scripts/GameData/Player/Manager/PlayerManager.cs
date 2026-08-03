using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public PlayerInfo Player;
    public string BaseLayerAnimName = "Base Layer";

    public event Action BeingHit;
    public event Action DoneBeingHit;
    public event Action OnGetHit;

    private readonly int _animHurt = Animator.StringToHash("Hurt");
    private readonly int _animDeath = Animator.StringToHash("Death");

    private PlayerStatsManager _stats;
    private HealthSystem _healthSystem;

    private Animator _animator;
    private Coroutine _hitCoroutine = null;

    private PlayerInput _inputSystem;

    private void Awake()
    {
        Player.PlayerTransform = this.transform;
        _stats = GetComponent<PlayerStatsManager>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _animator = GetComponentInChildren<Animator>();
        _inputSystem = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnTakeDmg += GetHit;
            _healthSystem.OnDeath += PlayDeath;
        }
        GameManager.Instance.ChangeActionInputMap += SwitchActionMap;
        GameManager.Instance.RespawnPlayer += ResetPlayer;
        GameManager.Instance.ResetStatsPlayer += ResetPlayerStats;
    }
    private void OnDisable()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnTakeDmg -= GetHit;
            _healthSystem.OnDeath -= PlayDeath;
        }
        GameManager.Instance.ChangeActionInputMap -= SwitchActionMap;
        GameManager.Instance.RespawnPlayer -= ResetPlayer;
        GameManager.Instance.ResetStatsPlayer -= ResetPlayerStats;
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
    public void SwitchActionMap(ActionInputMapType type)
    {
        if (_inputSystem == null || _inputSystem.actions == null) return;

        _inputSystem.actions.FindActionMap("Player").Disable();
        _inputSystem.actions.FindActionMap("UI").Disable();
        //_inputSystem.actions.FindActionMap("Interaction").Disable();
        GameManager.Instance.GlobalInput.Interaction.Disable();

        switch (type)
        {
            case ActionInputMapType.Player:
                _inputSystem.actions.FindActionMap("Player").Enable();
                break;
            case ActionInputMapType.UI:
                _inputSystem.actions.FindActionMap("UI").Enable();
                break;
            case ActionInputMapType.Interaction:
                Debug.Log("Doi r ne");
                //GameManager.Instance.GlobalInput.Interaction.Enable();
                GameManager.Instance.GlobalInput.Interaction.Enable();
                break;
        }
    }
    private void PlayDeath()
    {
        if (_animator == null) return;
        _animator.CrossFade(_animDeath, 0.1f);
    }
    private void ResetPlayer()
    {
        ResetPlayerStats();
        _animator.Rebind();
        _animator.Update(0f);
    }
    private void ResetPlayerStats()
    {
        if (_healthSystem != null)
        {
            _healthSystem.ResetHP();
        }
        if (_stats != null)
        {
            _stats.ResetMana();
            _stats.ResetStamina();
        }

    }
}
