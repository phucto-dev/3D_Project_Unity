using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "GameData/Enemies/Stats/EnemyStats")]
public class EnemyStats : BaseStatsSO
{
    [Header("--- ATTACK INFO ---")]
    [SerializeField]
    private int _numberOfAttack;

    [Header("--- HURT INFO ---")]
    [SerializeField]
    private int _quantityOfHurt;
    [SerializeField]
    private StatData _hurtDelay = new StatData(2f);
    public StatData HurtDelay => _hurtDelay;
    public int NumberOfAttack => _numberOfAttack;
    public int QuantityOfHurt => _quantityOfHurt;

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
