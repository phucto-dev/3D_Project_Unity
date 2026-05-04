using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LootMagnetSystem : MonoBehaviour
{
    [Header("--- REF ----")]
    [SerializeField] private LootAnimConfigSO _config;

    [Header("--- SETTINGS ---")]
    [SerializeField] private float _limitOffset = 0.01f;

    private bool _isBeingSuck;
    private float _currentSpeed;
    private Transform _target;
    private Transform _parent;
    private LootDropSystem _dropSystem;

    private void Awake()
    {

        _dropSystem = GetComponentInParent<LootDropSystem>();
        _parent = this.transform.parent.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_dropSystem == null || _config == null) return;
        if (_isBeingSuck) return;

        if (other.CompareTag(TagConstant.TagPlayer))
        {
            _target = other.transform;
            _isBeingSuck = true;
            _currentSpeed = _config.Speed;

            _dropSystem.enabled = false;
        }
    }

    private void Update()
    {
        if (!_isBeingSuck || _target == null) return;

        _currentSpeed += _config.Acceleration * Time.deltaTime;
        _parent.position = Vector3.MoveTowards(_parent.position, _target.position, _currentSpeed * Time.deltaTime);
        float distance = (_target.position - _parent.position).sqrMagnitude;
        if (distance < _limitOffset * _limitOffset)
        {
            AbsortLoot();
        }
    }

    private void AbsortLoot()
    {
        Debug.Log("Get Item");

        this.transform.parent.gameObject.SetActive(false);
    }
}
