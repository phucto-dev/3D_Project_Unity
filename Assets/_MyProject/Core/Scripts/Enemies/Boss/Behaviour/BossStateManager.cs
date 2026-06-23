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
public class GroundLocomotion: ILocomotionStrategy
{
    public void Enter(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = true;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        if (boss.GetNavMeshAgent().isOnNavMesh) boss.GetNavMeshAgent().SetDestination(targetPosition);
    }
    public void Exit(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = true;
    }
}
public class AirLocomotion: ILocomotionStrategy
{
    public void Enter(BossStateManager boss)
    {
        boss.GetNavMeshAgent().enabled = false;
        boss.GetRigidbody().isKinematic = true;
    }
    public void MoveTo(BossStateManager boss, Vector3 targetPosition)
    {
        boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetPosition, Time.deltaTime * boss.FlySpeed);
        Vector3 dir = (targetPosition - boss.transform.position).normalized;
        if (dir != Vector3.zero)
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }
    public void Exit(BossStateManager boss)
    {

    }
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
        SetLocomotion(new GroundLocomotion());
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
        SeePlayer = _vision.CanSeePlayer();
    }
    private void Update()
    {
        
    }
    public void SetLocomotion(ILocomotionStrategy newLocomotion)
    {
        _currentLocomotion?.Exit(this);
        _currentLocomotion = newLocomotion;
        _currentLocomotion.Enter(this);
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
}