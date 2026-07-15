using UnityEngine;

public static class BossAttackFactory
{
    public static IBossAttackStrategy CreateStrategy(BossCombatInfo combatInfo)
    {
        switch (combatInfo.AttackType)
        {
            case BossAttackType.DashAndBite:
                return new BossBite();
            case BossAttackType.CoreBeam:
                return new BossBlast();
            case BossAttackType.SummonStatues:
                return new BossSummonAttackStrategy(combatInfo);
            case BossAttackType.SkyFall:
                return new BossSummonAttackStrategy(combatInfo);
            default:
                return null;
        }
    }
}
