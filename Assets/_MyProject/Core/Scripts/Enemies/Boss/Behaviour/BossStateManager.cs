using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossSpeedType
{
    Normal,
    Fast
}
public interface ILocomotionStrategy
{
    void Enter(BossStateManager boss);
    void MoveTo(BossStateManager boss, Vector3 targetPosition);
    void MoveBack(BossStateManager boss, Vector3 dir);
    void SetSpeedType(BossStateManager boss, BossSpeedType speedType);
    void Stop(BossStateManager boss);
    void Exit(BossStateManager boss);
}
public interface IBossAttackStrategy
{
    IEnumerator ExecuteRoutine(BossStateManager boss);
    void SetCombatInfo(BossCombatInfo info);
    void AttackTrigger(BossStateManager boss);
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

    [field: SerializeField] public GameObject MouthPoint { get; private set; }

    [Header("--- REF ---")]
    [field: SerializeField] public PlayerInfo PlayerInformation { get; private set; }
    [field: SerializeField] public BossCombatListSO BossCombatDataList { get; private set; }

    public event Action OnSummonStatues;

    private string BaseAnimLayer = "Base Layer";
    private string TurnLeftAnimName = "Turn90L";
    private string TurnRightAnimName = "Turn90R";
    private string IdleBreatheAnimName = "IdleBreathe";
    private string FlyStationAnimName = "FlyStationary";
    private string FlyStationToLandAnimName = "FlyStationaryToLanding";
    private string TakeOffAnimName = "TakeOff";

    private EnemyVision _vision;
    private ILocomotionStrategy _currentLocomotion;
    private IBossState _currentState;
    private Coroutine _currentAttackCoroutine;
    private HealthSystem _healthSystem;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private BossStatsManager _stats;
    private bool _isRotating;
    private bool _isSummonable;
    private string _currentAnimName;
    private float _turnStartThreshold = 60f;
    private float _turnStopThreshold = 5f;
    private BossPhase _currentPhase = BossPhase.SecondPhase;
    private readonly float _landingThreshold = 0f;
    private readonly float _flyMaxHeight = 12f;
    
    public NavMeshAgent GetNavMeshAgent() => _agent;
    public Rigidbody GetRigidbody() => _rb;
    public BossStatsManager GetStats() => _stats;
    public BossPhase GetCurrentPhase() => _currentPhase;
    public bool IsRotating { get => _isRotating; set => _isRotating = value; }
    public bool IsSummonAble { get => _isSummonable; set => _isSummonable = value; }
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _vision = GetComponent<EnemyVision>();
        Anim = GetComponentInChildren<Animator>();
        _stats = GetComponent<BossStatsManager>();
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
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
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
    public void MoveToDir(Vector3 dir) => _currentLocomotion?.MoveBack(this, dir);
    public void ChasePlayer() => _currentLocomotion?.MoveTo(this, Player.position);
    public void SetCurentSpeedType(BossSpeedType speed) => _currentLocomotion?.SetSpeedType(this,speed);

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
    public void RotateFaceToPlayer()
    {
        if (Player == null) return;
        Vector3 offset;
        offset = Player.position - transform.position;

        Vector3 dirToPlayer = offset.normalized;
        dirToPlayer.y = 0;

        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        if (!_isRotating)
        {
            if (angleToPlayer >= _turnStartThreshold)
            {
                _isRotating = true;

                int side = transform.GetDirectionToTarget(Player.position);
                string targetAnim = (side == -1) ? TurnLeftAnimName : TurnRightAnimName;

                if (_currentAnimName != targetAnim)
                {
                    _currentAnimName = targetAnim;
                    Anim.CrossFade(targetAnim, 0.1f);
                }
            }
            else
            {
                if (_currentAnimName != IdleBreatheAnimName)
                {
                    _currentAnimName = IdleBreatheAnimName;
                    Anim.CrossFade(IdleBreatheAnimName, 0.1f);
                }
            }
        }
        else
        {
            if (angleToPlayer > 1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
            }
            if (angleToPlayer <= _turnStopThreshold)
            {
                _isRotating = false;
            }
        }
    }
    public IEnumerator LandCoroutine(System.Action onComplete = null)
    {
        Vector3 landingTarget;
        Anim.CrossFade(FlyStationAnimName, 0.1f);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            landingTarget = hit.point;
        }
        else
        {
            landingTarget = transform.position - new Vector3(0, 10f, 0);
        }

        float distance = transform.position.y - landingTarget.y;
        float descendSpeed = 5f;
        while (distance > _landingThreshold)
        {
            Vector3 currentPos = transform.position;
            currentPos.y = Mathf.MoveTowards(currentPos.y, landingTarget.y, descendSpeed * Time.deltaTime);
            LookForward();
            transform.position = currentPos;
            distance = transform.position.y - landingTarget.y;
            yield return null;
        }
        Anim.CrossFade(FlyStationToLandAnimName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        float animLength = Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(BaseAnimLayer)).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        onComplete?.Invoke();
    }
    public IEnumerator TakeOffCoroutine(System.Action onComplete = null)
    {
        float riseSpeed = 5f;
        float groundPosY = transform.position.y;
        float maxHeight = groundPosY + _flyMaxHeight;
        float distance = maxHeight - groundPosY;
        Anim.CrossFade(TakeOffAnimName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        float animLength = Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(BaseAnimLayer)).length;
        yield return new WaitForSeconds(animLength * 0.5f);
        Anim.CrossFade(FlyStationAnimName, 0.1f);
        while (distance > 0.05f)
        {
            Vector3 currentPos = transform.position;
            currentPos.y = Mathf.MoveTowards(currentPos.y, maxHeight, riseSpeed * Time.deltaTime);
            LookForward();
            transform.position = currentPos;
            distance = maxHeight - transform.position.y;
            yield return null;
        }
        Anim.CrossFade(FlyStationAnimName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        onComplete?.Invoke();
    }
    public void SummonStatues()
    {
        OnSummonStatues?.Invoke();
    }
    public void SetSummonAble(bool value)
    {
        _isSummonable = value;
    }
}