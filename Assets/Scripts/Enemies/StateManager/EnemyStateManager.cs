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
    public Transform Player { get; private set; }
    public bool SeePlayer { get; private set; }

    [Header("Brain Config")]
    [SerializeField] private EnemyBrainConfig _brainConfig;

    private IEnemyState _currentState;
    private EnemyVision _vision;

    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();
        if (Agent == null) return;
        if (_vision == null)
        {
            Debug.Log("EnemyStateManager: _vision null");
            return;
        }
        Player = _vision.PlayerTarget;
        SeePlayer = _vision.CanSeePlayer();
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    public void SetPlayer(Transform player)
    {
        Player = player;
    }

    public EnemyBrainConfig GetBrainConfig() => _brainConfig;

}

public class PatrolState : IEnemyState
{
    private CooldownTimer _waitTimer = new CooldownTimer(2f); // 2f = Time delay 4 each destination. 
    private bool _isWaiting = false;
    public void EnterState(EnemyStateManager enemy)
    {
        FindNewPatrolPoint(enemy);
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.Player == null) return;
        if (Vector3.Distance(enemy.transform.position, enemy.Player.position) < enemy.GetBrainConfig().PatrolRadius)
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance)
        {
            if (!_isWaiting)
            {
                _isWaiting = true;
                _waitTimer = new CooldownTimer(2f);
            }
            if (_isWaiting && _waitTimer.Tick())
            {
                FindNewPatrolPoint(enemy);
                _isWaiting = false;
            }
        }

    }

    public void ExitState(EnemyStateManager enemy) 
    {

    }

    private void FindNewPatrolPoint(EnemyStateManager enemy)
    {
        Vector3 randomDir = Random.insideUnitSphere * enemy.GetBrainConfig().PatrolRadius;
        randomDir += enemy.transform.position;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, enemy.GetBrainConfig().PatrolRadius, NavMesh.AllAreas))
        {
            enemy.Agent.SetDestination(hit.position);
        }
    }
}

public class ChaseState: IEnemyState
{
    private CooldownTimer _pathUpdateTimer = new CooldownTimer(0.2f);
    public void EnterState(EnemyStateManager enemy)
    {
        
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.Player == null) return;

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.Player.position);
        if (distanceToPlayer > enemy.GetBrainConfig().LimitChaseRange)
        {
            enemy.SetPlayer(null);
            enemy.ChangeState(new PatrolState());
            return;
        }

        if (_pathUpdateTimer.Tick())
        {
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.SetDestination(enemy.Player.position);
            }
        }
    }

    public void ExitState(EnemyStateManager enemy)
    {
        if (enemy.Agent.isOnNavMesh)
        {
            enemy.Agent.ResetPath();
        }
    }
}


