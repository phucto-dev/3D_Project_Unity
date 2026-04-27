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
    [Header("--- REF ---")]
    public NavMeshAgent Agent { get; private set; }
    public Transform Player { get; private set; }
    public bool SeePlayer { get; private set; }

    [Header("--- BRAIN CONFIG ---")]
    [SerializeField] private EnemyBrainConfigSO _brainConfig;

    [Header("--- STATS ---")]
    [SerializeField] private EnemyStats _stats;

    private IEnemyState _currentState;
    private EnemyVision _vision;
    private CooldownTimer _delayTimer;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();
    }
    private void Start()
    {
        _delayTimer = new CooldownTimer(_brainConfig.SightDelay);
        if (Agent == null) return;
        if (_vision == null)
        {
            Debug.Log("EnemyStateManager: _vision null");
            return;
        }
        Player = _vision.Player.PlayerTransform;
        SeePlayer = _vision.CanSeePlayer();
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        CheckSawPlayer();
        _currentState.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        _currentState?.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }

    private void CheckSawPlayer()
    {
        if (_delayTimer.Tick())
        {
            SeePlayer = _vision.CanSeePlayer();
        }
    }
    public void SetupAgent(float speed, float angularSpeed = 80f, float acceleration = 4f)
    {
        Agent.speed = speed;
        Agent.angularSpeed = angularSpeed;
        Agent.acceleration = acceleration;
    }

    public EnemyBrainConfigSO GetBrainConfig() => _brainConfig;
    public EnemyStats GetStats() => _stats;

}

public class PatrolState : IEnemyState
{
    private CooldownTimer _waitTimer = new CooldownTimer(2f); // 2f = Time delay 4 each destination. 
    private bool _isWaiting = false;
    private float _speed;
    public void EnterState(EnemyStateManager enemy)
    {
        _speed = enemy.GetStats().WalkSpeed.BaseValue;
        enemy.SetupAgent(_speed);
        FindNewPatrolPoint(enemy);
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.Player == null) return;
        if (enemy.SeePlayer)
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
    private float _speed;
    public void EnterState(EnemyStateManager enemy)
    {
        _speed = enemy.GetStats().RunSpeed.BaseValue;
        enemy.SetupAgent(_speed);
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.Player == null) return;

        if (!enemy.SeePlayer)
        {
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


