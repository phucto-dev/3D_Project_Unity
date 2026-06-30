using System.Collections;
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
    void SetSpeedType(BossStateManager boss, BossSpeedType speedType);
    void Stop(BossStateManager boss);
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

    private string TurnLeftAnimName = "Turn90L";
    private string TurnRightAnimName = "Turn90R";
    private string IdleBreatheAnimName = "IdleBreathe";

    private EnemyVision _vision;
    private ILocomotionStrategy _currentLocomotion;
    private IBossState _currentState;
    private Coroutine _currentAttackCoroutine;
    private HealthSystem _healthSystem;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private BossStatsManager _stats;
    private bool _isRotating;
    private string _currentAnimName;
    private float _turnStartThreshold = 60f;
    private float _turnStopThreshold = 5f;
    public NavMeshAgent GetNavMeshAgent() => _agent;
    public Rigidbody GetRigidbody() => _rb;
    public BossStatsManager GetStats() => _stats;
    public bool IsRotating { get => _isRotating; set => _isRotating = value; }
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
            //Quaternion targetRot = Quaternion.LookRotation(dirToPlayer);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);

            //int side = transform.GetDirectionToTarget(Player.position);
            //string targetAnim = (side == -1) ? TurnLeftAnimName : TurnRightAnimName;
            //if (_currentAnimName != targetAnim)
            //{
            //    _currentAnimName = targetAnim;
            //    Anim.CrossFade(targetAnim, 0.1f);
            //}

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
}