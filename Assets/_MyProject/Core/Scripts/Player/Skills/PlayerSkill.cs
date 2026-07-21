using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillState
{
    Start,
    Smash,
    End
}
public class PlayerSkill : MonoBehaviour
{
    public SkillDataSO[] SkillSlots;
    [SerializeField] private float _blendTimeBetweenLayers = 0.2f;
    [SerializeField] private float _smashTime = 1.05f;

    [Header("--- ANIM ---")]
    public float TransitionDuaration = 0.2f;

    [Header("Animator Hashes")]
    private string _animStart = "CastStart";
    private string _animSmash = "CastSmash";
    private string _animEnd = "CastEnd";

    public event Action<bool> OnUsingSkill;

    private PlayerInput _inputSystem;
    private InputAction[] _skillInputActions;
    private int maxSlotAmount = 5;
    private Transform _playerTransform;
    private PlayerStatsManager _playerStats;
    private HealthSystem _healthSystem;
    private Animator _animator;
    private int _skillLayerIndex;
    private Coroutine _fadeLayerCoroutine;
    private bool _allowToUseSkill;

    private SkillState _currentState;
    private SkillDataSO _currentSkill;
    private void Awake()
    {
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _inputSystem = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStatsManager>();
        _playerTransform = this.transform;
        _skillInputActions = new InputAction[maxSlotAmount];
        _animator = GetComponentInChildren<Animator>();
        if (_animator != null)
        {
            _skillLayerIndex = _animator.GetLayerIndex("Skill");
        }
        if (_inputSystem == null) return;
        for (var i = 0; i < maxSlotAmount; i++)
        {
            _skillInputActions[i] = _inputSystem.actions[$"SkillSlot{i+1}"];
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < maxSlotAmount; i++)
        {
            if (_skillInputActions[i] == null) continue;
            int slotIndex = i;
            _skillInputActions[i].performed += ctx => HandleSkillCast(ctx, slotIndex);
        }
        _allowToUseSkill = true;
    }
    private void OnDisable()
    {
        for (int i = 0; i < maxSlotAmount; i++)
        {
            if (_skillInputActions[i] == null) continue;
            int slotIndex = i;
            _skillInputActions[i].performed -= ctx => HandleSkillCast(ctx, slotIndex);
        }
    }

    private void HandleSkillCast(InputAction.CallbackContext ctx, int index)
    {
        if (_playerStats == null) return;
        if (!_allowToUseSkill) return;
        SkillDataSO skill = SkillSlots[index];
        if (skill == null) return;
        if (!_playerStats.IsEnoughMana(skill.ManaCost)) return;
        if (CheckSkillAvaiableToCast())
        {
            float time = 0f;
            if (skill.SkillType == SkillType.Hold) time = skill.HoldCastingTime;
            else time = skill.CastTime;
            StartCoroutine(ExecuteSkillRoutine(time, skill));
        }
    }

    private bool CheckSkillAvaiableToCast()
    {

        return true;
    }
    private void ExecuteSkill(SkillDataSO skillData)
    {
        if (_playerStats == null) return;
        Vector3 spawnPosition = CalculateSpawnPosition(skillData);

        switch (skillData.SkillType)
        {
            case SkillType.AOE:
                GameObject vfxAOEInstance = Instantiate(skillData.VFXPrefab, spawnPosition, _playerTransform.rotation);
                vfxAOEInstance.GetComponent<SkillVFXController>().Initialize(skillData, _playerStats, this);
                break;
            case SkillType.Buff:
                GameObject vfxBuffInstance = Instantiate(skillData.VFXPrefab, spawnPosition, _playerTransform.rotation, this.transform);
                vfxBuffInstance.GetComponent<SkillBuffController>().Initialize(skillData, _playerStats, _healthSystem);
                break;
            case SkillType.Hold:
                GameObject vfxHoldInstance = Instantiate(skillData.VFXPrefab, spawnPosition, _playerTransform.rotation);
                vfxHoldInstance.GetComponent<SkillVFXController>().Initialize(skillData, _playerStats, this);
                break;
        }
    }
    private bool CheckCurrentSkillUseable()
    {
        return true;
    }
    public void ChangeState(SkillState state)
    {
        _currentState = state;
        AnimSkillControll(state);
    }
    public void AnimSkillControll(SkillState state)
    {
        switch (state)
        {
            case SkillState.Start:
                _animator.SetLayerWeight(_skillLayerIndex, 1f);
                _animator.CrossFadeInFixedTime(_animStart, TransitionDuaration);
                break;
            case SkillState.Smash:
                _animator.CrossFadeInFixedTime(_animSmash, TransitionDuaration);
                StartCoroutine(SmashTimerRoutine());
                break;
            case SkillState.End:
                _animator.CrossFadeInFixedTime(_animEnd, TransitionDuaration);
                SkillDone();
                break;
        }
    }
    private void SkillDone()
    {
        _currentState = SkillState.End;
        if (_fadeLayerCoroutine == null)
            _fadeLayerCoroutine = StartCoroutine(FadeOutSkillLayer(_blendTimeBetweenLayers));
        OnUsingSkill?.Invoke(false);
        _allowToUseSkill = true;
    }
    private IEnumerator ExecuteSkillRoutine(float duration, SkillDataSO skillData)
    {
        OnUsingSkill?.Invoke(true);
        _allowToUseSkill = false;
        ChangeState(SkillState.Start);

        yield return new WaitForSeconds(duration);

        ExecuteSkill(skillData);
        if (skillData.SkillType != SkillType.Hold)
        {
            SkillDone();
        }
    }
    private IEnumerator FadeOutSkillLayer(float fadeDuration)
    {
        float startWeight = _animator.GetLayerWeight(_skillLayerIndex);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / fadeDuration;

            float currentWeight = Mathf.Lerp(startWeight, 0f, percent);
            _animator.SetLayerWeight(_skillLayerIndex, currentWeight);

            yield return null;
        }

        _animator.SetLayerWeight(_skillLayerIndex, 0f);
        _fadeLayerCoroutine = null;
    }
    private IEnumerator SmashTimerRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _smashTime)
        {
            elapsedTime += Time.deltaTime;
            float percent = elapsedTime / _smashTime;

            yield return null;
        }

        ChangeState(SkillState.End);
    }
    private Vector3 CalculateSpawnPosition(SkillDataSO skillData)
    {
        if (skillData.targetingMode == SkillTargetingMode.Self)
        {
            return _playerTransform.position;
        }
        else
        {
            return _playerTransform.position + (_playerTransform.forward * skillData.forwardOffsetDistance);
        }
    }
}
