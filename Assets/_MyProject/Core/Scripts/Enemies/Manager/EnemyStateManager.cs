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

    private EnemyStatsManager _stats;
    private IEnemyState _currentState;
    private EnemyVision _vision;
    private EnemyAnimationManager _animManager;
    private CooldownTimer _delayTimer;
    private HealthSystem _healthSystem;
    private EnemyDropManager _dropManager;

    private bool _isBeingHit;
    private Material _mat;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        _vision = GetComponent<EnemyVision>();
        _animManager = GetComponent<EnemyAnimationManager>();
        _stats = GetComponent<EnemyStatsManager>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _dropManager = GetComponentInChildren<EnemyDropManager>();
    }
    private void OnEnable()
    {
        if (_healthSystem == null) return;
        _healthSystem.OnTakeDmg += BeingHit;
        _healthSystem.OnDeath += IsDeath;
    }

    private void OnDisable()
    {
        if (_healthSystem == null) return;
        _healthSystem.OnTakeDmg -= BeingHit;
        _healthSystem.OnDeath -= IsDeath;
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
        Agent.stoppingDistance = _stats.AttackRange.GetValue();
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

    private void BeingHit(DmgInfo dmgInfo)
    {
        if (!_stats.IsRunOutPoise(dmgInfo.PoiseDamage))
        {
            if (_currentState == new ChaseState() || _currentState == new AttackState()) return;
            else ChangeState(new AttackState());
        }
        else
        {
            if (_currentState == new HurtState()) return;
            ChangeState(new HurtState());
        }
    }

    private void IsDeath()
    {
        ChangeState(new DieState());
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
    public EnemyStatsManager GetStats() => _stats;
    public EnemyAnimationManager GetACController() => _animManager;
    public EnemyDropManager GetDropManager() => _dropManager;

}

public class PatrolState : IEnemyState
{
    private CooldownTimer _waitTimer = new CooldownTimer(2f); // 2f = Time delay 4 each destination. 
    private bool _isWaiting = false;
    private float _speed;
    public void EnterState(EnemyStateManager enemy)
    {
        _speed = enemy.GetStats().WalkSpeed.GetValue();
        enemy.SetupAgent(_speed);
        FindNewPatrolPoint(enemy);
        if (enemy.GetACController() != null) enemy.GetACController().EnableMovingAnim();
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
        if (enemy.GetACController() != null) enemy.GetACController().DisableMovingAnim();
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
    private Vector3 _offset;
    public void EnterState(EnemyStateManager enemy)
    {
        _speed = enemy.GetStats().RunSpeed.GetValue();
        enemy.SetupAgent(_speed);
        if (enemy.GetACController() != null) enemy.GetACController().EnableMovingAnim();
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
        RotateFaceToPlayer(enemy);
    }

    public void ExitState(EnemyStateManager enemy)
    {
        if (enemy.Agent.isOnNavMesh)
        {
            enemy.Agent.ResetPath();
        }
        if (enemy.GetACController() != null) enemy.GetACController().DisableMovingAnim();
    }

    private void RotateFaceToPlayer(EnemyStateManager enemy)
    {
        _offset = enemy.Player.position - enemy.transform.position;

        if (_offset.sqrMagnitude <= enemy.GetStats().AttackRange.GetValue() * enemy.GetStats().AttackRange.GetValue())
        {
            enemy.Agent.updateRotation = false; //

            Vector3 dirToPlayer = _offset.normalized;
            dirToPlayer.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRot, Time.deltaTime * enemy.GetBrainConfig().TurnSpeed);

            if (Vector3.Dot(enemy.transform.forward, dirToPlayer) >= 0.9)
            {
                enemy.ChangeState(new AttackState());
            }
        }
        else
        {
            enemy.Agent.updateRotation = true; //
        }
    }
}

public class AttackState : IEnemyState
{
    private Vector3 _offset;
    private CooldownTimer _attackUpdateTimer;
    private float _lastAttackTime;
    private float _actuallCooldown;
    public void EnterState(EnemyStateManager enemy)
    {
        _actuallCooldown = enemy.GetStats().DelayPerAttack.GetValue() / enemy.GetStats().Haste.GetValue();
        enemy.GetACController().EnableCombatAnim();
        enemy.Agent.updateRotation = false; //
        enemy.Agent.velocity = Vector3.zero;
    }
    public void UpdateState(EnemyStateManager enemy)
    {
        _offset = enemy.Player.position - enemy.transform.position;

        if (_offset.sqrMagnitude > enemy.GetStats().AttackRange.GetValue() * enemy.GetStats().AttackRange.GetValue() + 0.5f)
        {
            enemy.ChangeState(new ChaseState());
        }
        else
        {
            Vector3 dirToPlayer = _offset.normalized;
            dirToPlayer.y = 0;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(dirToPlayer), Time.deltaTime * enemy.GetBrainConfig().TurnSpeed);
            TriggerRandomAtack(enemy);
        }
    }
    public void ExitState(EnemyStateManager enemy)
    {
        enemy.GetACController().DisableCombatAnim();
        enemy.Agent.updateRotation = true; //
    }
    private void TriggerRandomAtack(EnemyStateManager enemy)
    {
        if (Time.time < _lastAttackTime + _actuallCooldown) return;
        _lastAttackTime = Time.time;
        int randomAttackIndex = Random.Range(1, (int)enemy.GetStats().QuantityOfAttack.GetValue() + 1);
        string attackStateName = "Attack_" + randomAttackIndex;

        enemy.GetACController().DoARandomAttack(attackStateName, enemy.GetStats().Haste.GetValue());
    }
}

public class HurtState : IEnemyState
{
    private float _lastTimeHurt;
    private float _actuallCooldown;
    private bool _isHurtOnce;
    private AnimatorStateInfo _stateInfo;
    public void EnterState(EnemyStateManager enemy)
    {
        enemy.GetACController().EnableCombatAnim(); // Should enable here to lock in combat mode.
        _actuallCooldown = enemy.GetStats().HurtDelay.GetValue();
        _isHurtOnce = false;
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        TriggerRandomHurt(enemy);
    }

    public void ExitState(EnemyStateManager enemy)
    {
        enemy.GetStats().RecoverPoise();
    }

    private void TriggerRandomHurt(EnemyStateManager enemy)
    {
        if (!_isHurtOnce)
        {
            int randomHurtIndex = Random.Range(1, (int)enemy.GetStats().QuantityOfHurt.GetValue() + 1);
            string attackStateName = "Hurt_" + randomHurtIndex;
            enemy.GetACController().DoTargetAnim(attackStateName);
            _isHurtOnce = true;
        }
        else
        {
            _stateInfo = enemy.GetACController().GetStateInfo();
            if (_stateInfo.normalizedTime >= 1.0f)
            {
                enemy.ChangeState(new AttackState());
            }
        }
    }
}

public class DieState: IEnemyState
{
    private bool _isDeath;
    public void EnterState(EnemyStateManager enemy)
    {
        _isDeath = false;
        enemy.GetDropManager().ExecuteDrop();
    }
    public void UpdateState(EnemyStateManager enemy)
    {
        if (_isDeath) return;
        enemy.GetACController().DoTargetAnim("Death");
        _isDeath = true;
    } 
    public void ExitState(EnemyStateManager enemy)
    {

    }
}


