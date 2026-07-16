using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    [field: SerializeField] public PoolItemSO SkyFallData { get; private set; }

    public event Action<BossCombatInfo , BossStatsManager> OnSummonStatues;

    private string BaseAnimLayer = "Base Layer";
    private string FlyStationAnimName = "FlyStationary";
    private string FlyStationToLandAnimName = "FlyStationaryToLanding";
    private string TakeOffAnimName = "TakeOff";

    private ILocomotionStrategy _currentLocomotion;
    private IBossState _currentState;
    private Coroutine _currentAttackCoroutine;
    private HealthSystem _healthSystem;
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    private BossStatsManager _stats;
    private BossBiteHitboxControl _bossBiteHitboxControl;
    private bool _isRotating;
    private bool _isSummonable;
    private BossPhase _currentPhase = BossPhase.FirstPhase;
    private readonly float _landingThreshold = 0.05f;
    private readonly float _flyMaxHeight = 15f;
    private bool _isBossDeath;

    public NavMeshAgent GetNavMeshAgent() => _agent;
    public Rigidbody GetRigidbody() => _rb;
    public BossStatsManager GetStats() => _stats;
    public BossPhase GetCurrentPhase() => _currentPhase;
    public bool IsRotating { get => _isRotating; set => _isRotating = value; }
    public bool AlreadyTurn { get ; private set; }
    public bool IsSummonAble { get => _isSummonable; set => _isSummonable = value; }
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        Anim = GetComponentInChildren<Animator>();
        _stats = GetComponent<BossStatsManager>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _bossBiteHitboxControl = GetComponentInChildren<BossBiteHitboxControl>();
    }
    private void OnEnable()
    {
        if (_healthSystem != null) _healthSystem.OnDeath += OnDeath;
        if (_healthSystem != null) _healthSystem.OnTakeDmg += OnTakeDmg;
        if (_healthSystem != null) _healthSystem.OnHealthChanged += OnChangePhase;
    }
    private void OnDisable()
    {
        if (_healthSystem != null) _healthSystem.OnDeath -= OnDeath;
        if (_healthSystem != null) _healthSystem.OnTakeDmg -= OnTakeDmg;
        if (_healthSystem != null) _healthSystem.OnHealthChanged -= OnChangePhase;
    }
    private void Start()
    {
        _isBossDeath = false;
        if (PlayerInformation != null) Player = PlayerInformation.PlayerTransform;
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
        if (_isBossDeath) return;
        if (newState is BossDieState) _isBossDeath = true;
        if (_currentState is BossTurnState) AlreadyTurn = true; 
        if (!(newState is BossDecisionState)) AlreadyTurn = false; 
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState.Enter(this);
    }
    public void MoveToTarget(Vector3 target) => _currentLocomotion?.MoveTo(this, target);
    public void MoveToDir(Vector3 dir) => _currentLocomotion?.MoveBack(this, dir);
    public void ChasePlayer() => _currentLocomotion?.MoveTo(this, Player.position);
    public void SetCurentSpeedType(BossSpeedType speed) => _currentLocomotion?.SetSpeedType(this,speed);
    public void CloseHurtBox() 
    {
        if (_healthSystem == null) return;
        _healthSystem.enabled = false;
    }
    public void OpenHurtBox() 
    {
        if (_healthSystem == null) return;
        _healthSystem.enabled = true;
    }
    public void ExecuteAttack(IBossAttackStrategy attack)
    {
        Debug.Log("Execute Attack: " + attack);
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

    public IEnumerator LandCoroutine(System.Action onComplete = null)
    {
        Vector3 landingTarget;
        Anim.CrossFade(FlyStationAnimName, 0.1f);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
            {
                landingTarget = navHit.position;
            }
            else landingTarget = hit.point;
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
        Vector3 finalPos = transform.position;
        finalPos.y = landingTarget.y;
        transform.position = finalPos;

        Anim.CrossFade(FlyStationToLandAnimName, 0.1f);
        yield return new WaitForSeconds(0.1f);
        float animLength = Anim.GetCurrentAnimatorStateInfo(Anim.GetLayerIndex(BaseAnimLayer)).length;
        yield return new WaitForSeconds(animLength - 0.1f);
        onComplete?.Invoke();
    }
    public IEnumerator TakeOffCoroutine(System.Action onComplete = null, float flyHeight = -1)
    {
        float riseSpeed = 5f;
        float groundPosY = transform.position.y;
        float flyMaxHeight = flyHeight <= 0 ? _flyMaxHeight : flyHeight;
        float maxHeight = groundPosY + flyMaxHeight;
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
    public void SummonStatues(BossCombatInfo info)
    {
        OnSummonStatues?.Invoke(info, _stats);
    }
    public void SummonSkyFall(BossCombatInfo info)
    {
        string id = SkyFallData.poolID;
        Vector3 summonPos = Player.position;
        summonPos.y = transform.position.y + 20f;
        GameObject skyFall = PoolManager.Instance.Get(id);
        if (skyFall != null)
        {
            skyFall.transform.position = summonPos;
            PooledObjectMove skillAttack = skyFall.GetComponent<PooledObjectMove>();
            if (skillAttack != null) skillAttack.SetUpHitObject(info, _stats);
        }
    }
    public void SetSummonAble(bool value)
    {
        _isSummonable = value;
    }
    public float GetGroundYPos()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
        {
            if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 5f, NavMesh.AllAreas))
            {
                return navHit.position.y;
            }
            else return hit.point.y;
        }
        return -1;
    }
    private void OnDeath()
    {
        ChangeState(new BossDieState());
    }
    private void OnTakeDmg(DmgInfo info)
    {
        if (!(_currentState is BossGroundedIdleState)) return;
        ChangeState(new BossHurtState());
    }
    private void OnChangePhase(float currentHealth, float maxHealth)
    {
        if (_currentPhase == BossPhase.SecondPhase) return;
        float percentleft = currentHealth / maxHealth * 100f;
        if (percentleft <= 50)
        {
            _currentPhase = BossPhase.SecondPhase;
            ChangeState(new BossEnterSecondPhase());
        }
    }
    public void BossOpenBiteHitBox(BossCombatInfo info)
    {
        if (_bossBiteHitboxControl == null) return;
        _bossBiteHitboxControl.OpenHitbox(info);
    }
    public void BossCloseBiteHitBox()
    {
        if (_bossBiteHitboxControl == null) return;
        _bossBiteHitboxControl.CloseHitbox();
    }
}