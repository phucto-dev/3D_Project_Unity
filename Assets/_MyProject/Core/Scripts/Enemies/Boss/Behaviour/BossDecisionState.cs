using UnityEditor;
using UnityEngine;

public class BossDecisionState : IBossState
{
    private string IdleBreatheAnimName = "IdleBreathe";
    private float offsetAngle = 6f;
    public void Enter(BossStateManager boss)
    {
        Debug.Log("sap vao TurnState ne");
        if (!boss.AlreadyTurn)
        {
            Debug.Log("TurnState ne");
            Vector3 offset = boss.Player.position - boss.transform.position;
            offset.y = 0;
            Vector3 dirToPlayer = offset.normalized;

            float liveAngleToPlayer = Vector3.Angle(boss.transform.forward, dirToPlayer);

            if (liveAngleToPlayer >= offsetAngle)
            {
                boss.ChangeState(new BossTurnState());
                return;
            }
        }
        int layerIndex = 0;
        AnimatorStateInfo currentState = boss.Anim.GetCurrentAnimatorStateInfo(layerIndex);
        AnimatorStateInfo nextState = boss.Anim.GetNextAnimatorStateInfo(layerIndex);
        bool isInTransition = boss.Anim.IsInTransition(layerIndex);

        bool isAlreadyIdle = currentState.IsName(IdleBreatheAnimName) ||
                             (isInTransition && nextState.IsName(IdleBreatheAnimName));

        if (!isAlreadyIdle)
        {
            boss.Anim.CrossFade(IdleBreatheAnimName, 0.1f);
        }

        float currentStamina = boss.GetStats().GetCurrentStamina();
        float maxStamina = boss.GetStats().GetMaxStamina();
        float staminaPercent = currentStamina / maxStamina;
        float randomValue = Random.Range(0f, 100f);
        float lowestStaminaMoveset = Mathf.Infinity+1;
        foreach (BossCombatInfo moveset in boss.BossCombatDataList.BossCombatStates)
        {
            float movesetStamina = moveset.StaminaConsume;
            if (movesetStamina < lowestStaminaMoveset) lowestStaminaMoveset = movesetStamina;
        }

        if (currentStamina < lowestStaminaMoveset)
        {
            boss.ChangeState(new BossGroundedIdleState());
            return;
        }

        if (staminaPercent >= 1f)
        {
            boss.ChangeState(new BossCombatState());
        }
        else if (staminaPercent >= 0.5f)
        {
            if (randomValue <= 70f) boss.ChangeState(new BossCombatState());
            else boss.ChangeState(new BossGroundedIdleState());
        }
        else
        {
            if (randomValue <= 40f) boss.ChangeState(new BossCombatState());
            else boss.ChangeState(new BossGroundedIdleState());
        }
    }
    public void UpdateState(BossStateManager boss)
    {

    }
    public void Exit(BossStateManager boss)
    {

    }
    public void OnActionTriggered(BossStateManager boss)
    {

    }
    public void OnAnimationEnded(BossStateManager boss)
    {

    }
}
