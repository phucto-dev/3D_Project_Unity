using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

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
    [field: SerializeField] public PlayerInfo PlayerInformation { get; private set; }
    public Transform Player { get; private set; }
    public bool SeePlayer { get; private set; }

    [Header("--- BRAIN CONFIG ---")]
    [SerializeField] private EnemyBrainConfigSO _brainConfig;

    public EntitySpawnInfo SpawnInfo { get; private set; }
    public float LastAttackTime { get; set; }
    public event Action<bool> PingDespawnSignal;
    public event Action<bool> DoTryDespawn;
    public event Action<GameObject> PingDeath;

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
        //Player = _vision.Player.PlayerTransform;
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
        Agent.stoppingDistance = _stats.AttackRange.GetValue();
        SeePlayer = _vision.CanSeePlayer();
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
        CheckSawPlayer();
        //Debug.Log("Current: " + _currentState.ToString());
        if (!Agent.enabled) return;
        _currentState.UpdateState(this);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (!Agent.enabled) return;
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
        GameEventManager.TriggerObjectiveAction(ObjectiveEventType.KillEnemy, true, "KillAny", 1);
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
    public void SetSpawnInfo(EntitySpawnInfo spawn)
    {
        SpawnInfo = spawn;
    }
    public void InvokeDespawnPing(bool value)
    {
        PingDespawnSignal?.Invoke(value);
    }
    public void InvokeDoTryDespawn()
    {
        DoTryDespawn?.Invoke(false);
    }
    public void DoForceSeePlayer()
    {
        _vision.ForceSeePlayer();
    }
    public void InvokePingDeath()
    {
        PingDeath?.Invoke(this.gameObject);
    }
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
        enemy.InvokeDespawnPing(true);
        enemy.InvokeDoTryDespawn();
    }

    public void UpdateState(EnemyStateManager enemy)
    {
        if (!enemy.Agent.enabled) return;
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
        enemy.InvokeDespawnPing(false);
    }

    private void FindNewPatrolPoint(EnemyStateManager enemy)
    {
        EntitySpawnInfo spawnInfo = enemy.SpawnInfo;

        float targetRadius = enemy.SpawnInfo == null ? enemy.GetBrainConfig().PatrolRadius : enemy.SpawnInfo.PatrolRadius;
        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * targetRadius;
        randomDir += spawnInfo.SpawnPos;

        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, spawnInfo.PatrolRadius, NavMesh.AllAreas))
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
        if (!enemy.Agent.enabled) return;
        if (enemy.Player == null) return;

        if (!enemy.SeePlayer)
        {
            enemy.ChangeState(new GoBackToSpawnState());
            //enemy.ChangeState(new PatrolState());
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

public class GoBackToSpawnState: IEnemyState
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
        if (!enemy.Agent.enabled) return;
        EntitySpawnInfo spawnInfo = enemy.SpawnInfo;
        if (spawnInfo == null) return;

        Vector3 flatCurrentPos = new Vector3(enemy.transform.position.x, 0f, enemy.transform.position.z);
        Vector3 flatSpawnPos = new Vector3(spawnInfo.SpawnPos.x, 0f, spawnInfo.SpawnPos.z);

        float currentDistance = Vector3.Distance(flatCurrentPos, flatSpawnPos);

        if (currentDistance <= spawnInfo.PatrolRadius - 2f)
        {
            enemy.ChangeState(new PatrolState());
            return;
        }
        if (_pathUpdateTimer.Tick())
        {
            if (enemy.Agent.isOnNavMesh)
            {
                enemy.Agent.SetDestination(enemy.SpawnInfo.SpawnPos);
            }
        }
        RotateFaceToSpawn(enemy);
    }
    public void ExitState(EnemyStateManager enemy)
    {
        if (enemy.Agent.isOnNavMesh)
        {
            enemy.Agent.ResetPath();
        }
        if (enemy.GetACController() != null) enemy.GetACController().DisableMovingAnim();
    }
    private void RotateFaceToSpawn(EnemyStateManager enemy)
    {
        _offset = enemy.SpawnInfo.SpawnPos - enemy.transform.position;

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
    private bool _hasAttacked;
    private AnimatorStateInfo _stateInfo;
    private bool _hadDoneAttack;
    public void EnterState(EnemyStateManager enemy)
    {
        _hasAttacked = false;
        _hadDoneAttack = false;
        _actuallCooldown = enemy.GetStats().DelayPerAttack.GetValue() / enemy.GetStats().Haste.GetValue();
        enemy.GetACController().EnableCombatAnim();
        enemy.Agent.updateRotation = false; //
        enemy.Agent.velocity = Vector3.zero;
        enemy.Agent.isStopped = true;
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

            if (_hasAttacked) //
            {
                _stateInfo = enemy.GetACController().GetStateInfo();
                if (_stateInfo.normalizedTime >= 0.95f)
                {
                    if (_hadDoneAttack == false)
                    {
                        enemy.LastAttackTime = Time.time;
                        _hadDoneAttack = true;
                    }
                    else
                    {
                        if (Time.time >= enemy.LastAttackTime + _actuallCooldown)
                        {
                            _hasAttacked = false;
                            _hadDoneAttack = false;
                        }
                    }
                    //enemy.ChangeState(new AttackCooldownState());
                }
                return;
            }

            if (Time.time >= enemy.LastAttackTime + _actuallCooldown)
            {
                TriggerRandomAtack(enemy);
                _hasAttacked = true;
            }
        }
    }
    public void ExitState(EnemyStateManager enemy)
    {
        enemy.GetACController().DisableCombatAnim();
        enemy.Agent.updateRotation = true; //
        enemy.Agent.isStopped = false;
    }
    private void TriggerRandomAtack(EnemyStateManager enemy)
    {
        //if (Time.time < _lastAttackTime + _actuallCooldown) return;
        //_lastAttackTime = Time.time;
        int randomAttackIndex = UnityEngine.Random.Range(1, (int)enemy.GetStats().QuantityOfAttack.GetValue() + 1);
        string attackStateName = "Attack_" + randomAttackIndex;

        enemy.GetACController().DoARandomAttack(attackStateName, enemy.GetStats().Haste.GetValue());
    }
}

public class AttackCooldownState: IEnemyState
{
    private CooldownTimer _pathUpdateTimer = new CooldownTimer(0.2f);
    private float _actuallCooldown;
    private float _lastAttackTime;
    private float _safeDistance;
    private float _safeNumberRange = 0.5f;
    private float _safeRangeDistance = 2f;
    private float _rangeToChaseAgain = 0.5f;
    public void EnterState(EnemyStateManager enemy)
    {
        _actuallCooldown = enemy.GetStats().DelayPerAttack.GetValue() / enemy.GetStats().Haste.GetValue();
        _lastAttackTime = Time.time;

        _safeDistance = enemy.GetStats().AttackRange.GetValue() + _safeRangeDistance;
        if (enemy.GetACController() != null) enemy.GetACController().EnableMovingAnim();

        FindCooldownPos(enemy);
    }
    public void UpdateState(EnemyStateManager enemy)
    {
        if (Time.time >= _lastAttackTime + _actuallCooldown)
        {
            enemy.ChangeState(new ChaseState());
            return;
        }
        Vector3 _offset = enemy.Player.position - enemy.transform.position;
        float conditionRange = _offset.sqrMagnitude - (_safeDistance * _safeDistance);
        if (conditionRange >= 0 && conditionRange <= _rangeToChaseAgain)
        {
            if (!enemy.Agent.pathPending && enemy.Agent.remainingDistance <= _safeNumberRange)
            {
                enemy.Agent.updateRotation = false;

                Vector3 offsetToPlayer = enemy.Player.position - enemy.transform.position;
                Vector3 faceDir = offsetToPlayer.normalized;
                faceDir.y = 0;
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, Quaternion.LookRotation(faceDir), Time.deltaTime * enemy.GetBrainConfig().TurnSpeed);
            }
        }
        else
        {
            if (_pathUpdateTimer.Tick())
            {
                FindCooldownPos(enemy);
            }
        }
    }
    public void ExitState(EnemyStateManager enemy)
    {
        enemy.Agent.updateRotation = true;
        if (enemy.GetACController() != null) enemy.GetACController().DisableMovingAnim();
    }

    private void FindCooldownPos(EnemyStateManager enemy)
    {
        Vector3 dirToEnemy = (enemy.transform.position - enemy.Player.position).normalized;
        dirToEnemy.y = 0;

        float randomAngle = UnityEngine.Random.Range(-30f, 30f);
        Vector3 rotatedDir = Quaternion.Euler(0, randomAngle, 0) * dirToEnemy;
        Vector3 targetPos = enemy.Player.position + (rotatedDir * _safeDistance);

        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            enemy.Agent.SetDestination(hit.position);
        }
        else
        {
            enemy.Agent.SetDestination(enemy.transform.position);
        }
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
        enemy.DoForceSeePlayer();
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
            Debug.Log("!_isHurtOnce");
            int randomHurtIndex = UnityEngine.Random.Range(1, (int)enemy.GetStats().QuantityOfHurt.GetValue() + 1);
            string attackStateName = "Hurt_" + randomHurtIndex;
            enemy.GetACController().DoTargetAnim(attackStateName);
            _isHurtOnce = true;
        }
        else
        {
            Debug.Log("_isHurtOnce");
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
    private bool _isTriggerDespawnOnce;
    private AnimatorStateInfo _stateInfo;
    public void EnterState(EnemyStateManager enemy)
    {
        _isDeath = false;
        _isTriggerDespawnOnce = false;
        enemy.GetDropManager().ExecuteDrop();
        enemy.InvokeDespawnPing(true);
        enemy.InvokePingDeath();
    }
    public void UpdateState(EnemyStateManager enemy)
    {
        if (_isDeath)
        {
            if (_isTriggerDespawnOnce) return;
            _stateInfo = enemy.GetACController().GetStateInfo();
            if (_stateInfo.normalizedTime >= 1.0f)
            {
                _isTriggerDespawnOnce = true;
                enemy.InvokeDoTryDespawn();
            }
            return;
        }
        enemy.GetACController().DoTargetAnim("Death");
        _isDeath = true;
    } 
    public void ExitState(EnemyStateManager enemy)
    {

    }
}


