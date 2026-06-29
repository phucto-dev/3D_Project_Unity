using UnityEngine;

public static class BossAttackFactory
{
    public static IBossAttackStrategy CreateStrategy(BossAttackType type)
    {
        switch (type)
        {
            case BossAttackType.DashAndBite:
                return new BossBite();
            case BossAttackType.GroundFireBreath:
                return null;
            case BossAttackType.CarpetBombing:
                return null;
            default:
                return null;
        }
    }
}
