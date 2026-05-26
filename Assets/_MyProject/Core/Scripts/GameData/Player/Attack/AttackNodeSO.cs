using UnityEngine;

[CreateAssetMenu(fileName = "AttackNode", menuName = "GameData/Player/AttackNode")]
public class AttackNodeSO : ScriptableObject
{
    [Header("--- ANIMATOR ---")]
    public string AnimStateName;
    public float TransitionDuaration = 0.1f;
    public bool HasRecoveryAnim = false;

    [Header("--- SWING TIMING ---")]
    [Range(0,1)] public float SwingTiming;

    [Header("--- COMBO BRANCHES ---")]
    public AttackNodeSO NextLightAttack;
    public AttackNodeSO NextHeavyAttack;
    public AttackNodeSO NextAttack;

    public float DamageMultiplier = 1f;
}
