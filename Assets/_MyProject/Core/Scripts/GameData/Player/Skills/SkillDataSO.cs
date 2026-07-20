using UnityEngine;

public enum SkillTargetingMode
{
    Self,
    ForwardOffset,
}
public enum SkillType
{
    Hold,
    AOE,
    Buff,
}
[CreateAssetMenu(fileName = "NewSkill", menuName = "GameData/Player/Skills")]
public class SkillDataSO : ScriptableObject
{
    [Header("--- INFO ---")]
    public string SkillID;
    public string SkillName;
    public SkillType SkillType;
    public GameObject VFXPrefab;

    [Header("--- LIFECYCLE ---")]
    public float ManaCost = 0f;
    public float CastTime = 0f;
    public float ActiveDuration = 0f;
    public bool IsToggle = false;
    public float CooldownTime = 5f;
    public float HoldCastingTime = 0f;

    [Header("--- DMG & RANGE ---")]
    public bool HasBuff = false;
    public float DmgScaleMultiplier = 1f;
    public float PoiseDamge = 10f;
    public float RangeScaleMultiplier = 1f;

    [Header("--- BUFF ---")]
    public bool GainInvincible;
    public bool GainSuperArmor;
    public bool GainHealthRegenPassive;
    public bool RecoveryHealth;
    public float IncreaseDmgMultiplier;
    public float IncreaseDefMultiplier;
    public float IncreasePoiseDefMultiplier;
    public float HealRegenAmount;
    public float HealRegenPassiveAmount;
    public float BuffDuration;

    [Header("--- INTERACTION TICK ---")]
    [Tooltip("0.5 = 2 HIT PER SECOND")]
    public float TickInterval = 0.5f;

    [Header("--- LAYER ---")]
    public LayerMask TargetLayer;

    [Header("--- OFFSET ---")]
    public SkillTargetingMode targetingMode = SkillTargetingMode.ForwardOffset;

    [Tooltip("Only for ForwardOffset skill type.")]
    public float forwardOffsetDistance = 2f;
}
