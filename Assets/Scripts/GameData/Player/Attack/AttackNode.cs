using UnityEngine;

[CreateAssetMenu(fileName = "AttackNode", menuName = "GameData/Player/AttackNode")]
public class AttackNode : ScriptableObject
{
    [Header("--- ANIMATOR ---")]
    public string AnimStateName;
    public float TransitionDuaration;

    [Header("--- COMBO BRANCHES ---")]
    public AttackNode nextLightAttack;
    public AttackNode nextHeavyAttack;
    public AttackNode nextAttack;
}
