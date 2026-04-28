using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "GameData/Enemies/Stats/EnemyStats")]
public class EnemyStats : BaseStatsSO
{
    [Header("--- ATTACK INFO ---")]
    [SerializeField]
    private int _numberOfAttack;
    public int NumberOfAttack => _numberOfAttack;

    [Header("--- ENEMY IDENTITY ---")]
    [SerializeField, Tooltip("Minion, Elite, Boss")]
    private EnemyRank rank = EnemyRank.Minion;
    public EnemyRank Rank => rank;

    [SerializeField]
    private FactionType faction = FactionType.Monster;
    public FactionType Faction => faction;

    [Header("--- REWARDS ---")]
    [SerializeField]
    private int xpYield = 50;
    public int XPYield => xpYield;

    // [SerializeField] private LootTableSO lootTable; 
}

public enum EnemyRank { Minion, Elite, Boss }
public enum FactionType { Player, Monster, NPC, Environment }
