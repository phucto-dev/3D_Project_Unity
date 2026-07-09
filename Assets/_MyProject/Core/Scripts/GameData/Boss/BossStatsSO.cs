using UnityEngine;

[CreateAssetMenu(fileName = "BossStats", menuName = "GameData/Boss/Stats/BossStats")]
public class BossStatsSO : EnemyStats
{
    [Header("--- STAMINA ---")]
    [SerializeField]
    private StatData _totalBossStamina = new StatData(100f);
    public StatData TotalBossStamina => _totalBossStamina;
}
