using UnityEngine;

public static class BossAttackFactory
{
    public static IBossAttackStrategy CreateStrategy(BossAttackType type)
    {
        switch (type)
        {
            case BossAttackType.DashAndBite:
                return new BossBite();
            case BossAttackType.CoreBeam:
                return new BossBlast();
            case BossAttackType.SummonStatues:
                return new BossSummonAttackStrategy();
            case BossAttackType.CarpetBombing:
                return null;
            default:
                return null;
        }
    }
}
