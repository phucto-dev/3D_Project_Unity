using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkill : MonoBehaviour
{
    public SkillDataSO[] SkillSlots;

    private PlayerInput _inputSystem;
    private InputAction[] _skillInputActions;
    private int maxSlotAmount = 5;
    private Transform _playerTransform;
    private PlayerStatsManager _playerStats;
    private HealthSystem _healthSystem;
    private PlayerStatsManager _statsManager;
    private void Awake()
    {
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _statsManager = GetComponent<PlayerStatsManager>();
        _inputSystem = GetComponent<PlayerInput>();
        _playerStats = GetComponent<PlayerStatsManager>();
        _playerTransform = this.transform;
        _skillInputActions = new InputAction[maxSlotAmount];
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
        SkillDataSO skill = SkillSlots[index];
        if (skill == null) return;
        if (!CheckSkillAvaiableToCast()) return;
        ExecuteSkill(skill);
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
                vfxAOEInstance.GetComponent<SkillVFXController>().Initialize(skillData, _playerStats);
                break;
            case SkillType.Buff:
                GameObject vfxBuffInstance = Instantiate(skillData.VFXPrefab, spawnPosition, _playerTransform.rotation, this.transform);
                vfxBuffInstance.GetComponent<SkillBuffController>().Initialize(skillData, _playerStats, _healthSystem, _statsManager);
                break;
        }
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
