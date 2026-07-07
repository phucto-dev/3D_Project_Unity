using System;
using UnityEngine;

public class BossEntryGate : MonoBehaviour
{
    private BossAreaManager _areaManager;
    public event Action PlayerEnterTrigger;

    private void Awake()
    {
        _areaManager = GetComponentInParent<BossAreaManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            PlayerEnterTrigger?.Invoke();
        }
    }
}
