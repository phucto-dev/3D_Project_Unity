using System;
using System.Collections;
using UnityEngine;

public class BossEntryGate : MonoBehaviour
{
    private BossAreaManager _areaManager;
    public event Action PlayerEnterTrigger;
    [SerializeField] private Collider _wall;
    [SerializeField] private float _closeTimeOffset = 1f;
    private Coroutine _closeGateRoutine;
    private bool _doneBossPhase;
    private void Awake()
    {
        _areaManager = GetComponentInParent<BossAreaManager>();
        _doneBossPhase = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_doneBossPhase) return;
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            PlayerEnterTrigger?.Invoke();
            if (_wall != null) _wall.enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_doneBossPhase) return;
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            if (_closeGateRoutine != null)
            {
                StopCoroutine(_closeGateRoutine);
                _closeGateRoutine = null;
            }
            _closeGateRoutine = StartCoroutine(CloseGateRoutineF());
        }
    }
    private IEnumerator CloseGateRoutineF()
    {
        yield return new WaitForSeconds(_closeTimeOffset);

        if (_wall != null) _wall.enabled = true;
        _closeGateRoutine = null;
    }
    public void OpenGate()
    {
        if (_wall != null) _wall.enabled = false;
    }
    public void MarkDoneBoss()
    {
        _doneBossPhase = true;
    }
}
