using TMPro;
using UnityEngine;

public class ObjectiveEventManager : MonoBehaviour
{
    public static ObjectiveEventManager Instance { get; private set; }

    [Header("--- UI REF ---")]
    public GameObject ObjectivePanel;
    public TMP_Text ObjectiveTitle;
    public TMP_Text ObjectiveDesc;
    public TMP_Text ObjectiveProgress;

    [Header("--- CURRENT PROGRESS ---")]
    [SerializeField] private ObjectiveChainSO _currentChain;
    [SerializeField] private ObjectiveEventNotice EventCall;

    private int _currentIndex = 0;

    private ObjectiveSO _currentObjective;
    private int _currentAmount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }
    private void OnEnable() => GameEventManager.OnObjectiveAction += HandleObjectiveProgress;
    private void OnDisable() => GameEventManager.OnObjectiveAction -= HandleObjectiveProgress;

    private void Start()
    {
        ObjectivePanel.SetActive(false);
    }

    public void StartObjectiveChain(ObjectiveChainSO chain)
    {
        if (chain == null || chain.Objectives.Count == 0) return;

        _currentChain = chain;
        _currentIndex = 0;

        ObjectivePanel.SetActive(true);
        LoadObjectiveFromChain();
    }
    private void LoadObjectiveFromChain()
    {
        _currentObjective = _currentChain.Objectives[_currentIndex];
        _currentAmount = 0;

        UpdateUI();
    }
    private void HandleObjectiveProgress(ObjectiveEventType type, bool targetCheck, string targetId, int amount)
    {
        //Debug.Log($"Param || Type: {type}, TargetCheck: {targetCheck}, TargetID: {targetId}, Amount: {amount}");
        if (_currentObjective == null) return;
        //Debug.Log($"CurrentObjective || Type: {_currentObjective.EventType}, TargetCheck: {_currentObjective.TargetCheck}, TargetID: {_currentObjective.TargetID}, Amount: {_currentObjective.RequiredAmount}");

        if (_currentObjective.EventType == type)
        {
            if (_currentObjective.TargetCheck)
            {
                if (_currentObjective.TargetID != targetId) return;
            }
            _currentAmount += amount;

            if (_currentAmount >= _currentObjective.RequiredAmount)
            {
                _currentAmount = _currentObjective.RequiredAmount;
                UpdateUI();
                CompleteCurrentObjective();
                return;
            }

            UpdateUI();
        }
    }
    private void CompleteCurrentObjective()
    {
        if (_currentObjective != null)
        {
            if (EventCall != null)
                EventCall.TriggerOnObjectiveEventComplete(_currentObjective.ObjectiveID);
        }

        _currentIndex++;

        if (_currentIndex < _currentChain.Objectives.Count)
        {
            LoadObjectiveFromChain();
        }
        else
        {
            CompleteChain();
        }
    }
    private void CompleteChain()
    {
        _currentObjective = null;
        _currentChain = null;

        ObjectivePanel.SetActive(false);
    }
    private void UpdateUI()
    {
        if (_currentObjective == null) return;
        ObjectiveTitle.SetText($"{_currentObjective.Title}");
        ObjectiveDesc.SetText($"{_currentObjective.Description}");
        ObjectiveProgress.SetText($"{_currentAmount}/{_currentObjective.RequiredAmount}");
    }
}
