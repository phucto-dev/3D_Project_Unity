using UnityEngine;
using UnityEngine.AI;

public interface IEnemyState
{
    void EnterState(EnemyStateManager context);
    void UpdateState(EnemyStateManager context);
    void ExitState(EnemyStateManager context);
}

public class EnemyStateManager : MonoBehaviour
{
    [Header("Ref")]
    public NavMeshAgent Agent { get; private set; }
    public Transform Player { get; set; }

    [Header("Stats")]
    [SerializeField] private float _attackRange;

    [Header("Patrol Settings")]
    [SerializeField] private float _patrolRadius;

    [Header("Patrol Settings")]
    [SerializeField] private float _limitChaseRange;

    private IEnemyState _currentState;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();

        if (Agent == null) return;
        ChangeState(new PatrolState());
    }

    private void FixedUpdate()
    {
        _currentState.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    public void ChaseRepeating()
    {
        InvokeRepeating(nameof(SetDestinationF), -1, 0.2f);
    }
    private void SetDestinationF()
    {
        Agent.SetDestination(Player.position);
    }
    public float GetAttackRange() => _attackRange;

}

public class PatrolState : IEnemyState
{
    public void EnterState(EnemyStateManager enemy)
    {
        //enemy.Agent.SetDestination(Target.position);
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.Player == null) return;
        if (Vector3.Distance(enemy.transform.position, enemy.Player.position) < enemy.GetAttackRange())
        {
            enemy.ChangeState(new ChaseState());
        }
    }

    public void ExitState(EnemyStateManager enemy) 
    {

    }
}

public class ChaseState: IEnemyState
{
    public void EnterState(EnemyStateManager enemy)
    {
        enemy.ChaseRepeating();
    }

    public void UpdateState(EnemyStateManager enemy)
    {

    }

    public void ExitState(EnemyStateManager enemy)
    {

    }
    private void SetDestinationF(NavMeshAgent agent, Transform player)
    {
        agent.SetDestination(player.position);
    }
}


