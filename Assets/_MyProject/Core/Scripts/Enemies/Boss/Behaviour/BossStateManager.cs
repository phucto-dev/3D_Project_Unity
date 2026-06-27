using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ILocomotionStrategy
{
    void Enter(BossStateManager boss);
    void MoveTo(BossStateManager boss, Vector3 targetPosition);
    void Exit(BossStateManager boss);
}
public interface IBossAttackStrategy
{
    IEnumerator ExecuteRoutine(BossStateManager boss);
}
public interface IBossState
{
    void Enter(BossStateManager boss);
    void UpdateState(BossStateManager boss);
    void Exit(BossStateManager boss);
    void OnActionTriggered(BossStateManager boss);
    void OnAnimationEnded(BossStateManager boss);
}

public class BossStateManager : MonoBehaviour
{
    public Animator Anim { get; private set; }
    public Transform Player { get; private set; }
    public bool SeePlayer { get; private set; }

    [Header("--- REF ---")]
    [field: SerializeField] public PlayerInfo PlayerInformation { get; private set; }
    public float FlySpeed = 15f;

    private EnemyStatsManager _stats;
    private EnemyVision _vision;
    private ILocomotionStrategy _currentLocomotion;
    private IBossState _currentState;
    private Coroutine _currentAttackCoroutine;
    private HealthSystem _healthSystem;
    private NavMeshAgent _agent;
    private Rigidbody _rb;

    public NavMeshAgent GetNavMeshAgent() => _agent;
    public Rigidbody GetRigidbody() => _rb;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _vision = GetComponent<EnemyVision>();
        Anim = GetComponentInChildren<Animator>();
        _stats = GetComponent<EnemyStatsManager>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
    }
    private void Start()
    {
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
        //SeePlayer = _vision.CanSeePlayer();
        ChangeState(new BossIntro());
    }
    private void Update()
    {
        _currentState?.UpdateState(this);
    }
    public void SetLocomotion(ILocomotionStrategy newLocomotion)
    {
        _currentLocomotion?.Exit(this);
        _currentLocomotion = newLocomotion;
        _currentLocomotion.Enter(this);
    }
    public void ChangeState(IBossState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }
    public void MoveToTarget(Vector3 target) => _currentLocomotion?.MoveTo(this, target);

    public void ExecuteAttack(IBossAttackStrategy attack)
    {
        StopCurrentAttack();
        _currentAttackCoroutine = StartCoroutine(attack.ExecuteRoutine(this));
    }

    public void StopCurrentAttack()
    {
        if (_currentAttackCoroutine != null)
        {
            StopCoroutine(_currentAttackCoroutine);
            _currentAttackCoroutine = null;
        }
    }
    public void OnActionTriggered()
    {
        _currentState?.OnActionTriggered(this);
    }
    public void OnAnimationEnded()
    {
        _currentState?.OnAnimationEnded(this);
    }
    public void LookForward()
    {
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0f, currentRotation.y, currentRotation.z);
    }
}