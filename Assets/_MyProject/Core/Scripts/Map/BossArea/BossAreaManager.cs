using Newtonsoft.Json;
using System;
using UnityEngine;
public struct StatueInfo
{
    public BossStatueSpawner Statue;
    public bool IsActive;
}
public class BossAreaManager : MonoBehaviour
{
    [Header("--- REF ---")]
    public PoolItemSO BossInfo;
    public Transform BossSpawn;
    public BossStatueSpawner StatueA;
    public BossStatueSpawner StatueB;

    public event Action<bool> OnStatueStatusChanged;

    private StatueInfo[] _statueList;
    private BossEntryGate _bossEntryGate;
    private GameObject _boss;
    private bool _isStarted;
    private void Awake()
    {
        _bossEntryGate = GetComponentInChildren<BossEntryGate>();
        _isStarted = false;
    }
    private void OnEnable()
    {
        if (_bossEntryGate != null) _bossEntryGate.PlayerEnterTrigger += StartBossPhase;
        if (_bossEntryGate != null) _bossEntryGate.PlayerEnterTrigger += MainMenuController.Instance.ShowBossHUD;
    }
    private void OnDisable()
    {
        if (_bossEntryGate != null) _bossEntryGate.PlayerEnterTrigger -= StartBossPhase;
        if (_bossEntryGate != null) _bossEntryGate.PlayerEnterTrigger -= MainMenuController.Instance.ShowBossHUD;
    }
    private void Start()
    {
        _statueList = new StatueInfo[]
        {
            new StatueInfo {Statue = StatueA, IsActive = false},
            new StatueInfo {Statue = StatueB, IsActive = false},
        };
    }
    private void StartBossPhase()
    {
        if (_isStarted) return;
        if (BossInfo == null) return;
        _boss = PoolManager.Instance.Get(BossInfo.poolID);
        if (_boss == null) return;
        BossManager bossManager = _boss.GetComponent<BossManager>();
        HealthSystem bossHealth = _boss.GetComponentInChildren<HealthSystem>();
        if (bossManager != null)
        {
            bossManager.GetBossStateManager().OnSummonStatues += SummonStatues;
            OnStatueStatusChanged += bossManager.GetBossStateManager().SetSummonAble;
        }
        if (bossHealth != null)
        {
            if (MainMenuController.Instance.HPBossBar() != null)
            {
                bossHealth.OnHealthChanged += MainMenuController.Instance.HPBossBar().SetTargetHealth;
            }
        }
        OnStatueStatusChanged?.Invoke(true);

        _boss.transform.position = BossSpawn.position;
        _boss.transform.rotation = BossSpawn.rotation;
        _isStarted = true;
    }
    private void ResetBossPhase()
    {
        if (_boss == null) return;
        BossManager bossManager = _boss.GetComponent<BossManager>();
        HealthSystem bossHealth = _boss.GetComponentInChildren<HealthSystem>();
        if (bossManager != null)
        {
            bossManager.GetBossStateManager().OnSummonStatues -= SummonStatues;
            OnStatueStatusChanged -= bossManager.GetBossStateManager().SetSummonAble;
        }
        if (bossHealth != null)
        {
            if (MainMenuController.Instance.HPBossBar() != null)
            {
                bossHealth.OnHealthChanged -= MainMenuController.Instance.HPBossBar().SetTargetHealth;
            }
        }
        PoolManager.Instance.Release(BossInfo.poolID, _boss);
        _boss = null;
        StartBossPhase();
    }
    private void SummonStatues(BossCombatInfo info, BossStatsManager stats)
    {
        int flag = 0;
        int i = 0;
        foreach (StatueInfo statue in _statueList)
        {
            if (!statue.IsActive)
            {
                statue.Statue.Activate(info, stats);
                _statueList[i].IsActive = true;
                flag++;
            }
            i++;
        }
        if (flag >= _statueList.Length) OnStatueStatusChanged?.Invoke(false);
        else OnStatueStatusChanged?.Invoke(true);
    }
    private void SetStatueStatus(bool value)
    {

    }
    private bool CheckAllowSummon()
    {
        OnStatueStatusChanged?.Invoke(false);
        return false;
    }
}
