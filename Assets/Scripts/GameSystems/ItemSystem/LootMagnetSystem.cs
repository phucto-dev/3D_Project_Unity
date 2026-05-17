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
    private PlayerInventoryManager _playerInventory;

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
            _playerInventory = _target.GetComponent<PlayerInventoryManager>();
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
        int leftOverItems;
        DropInfo dropInfo = GetComponentInParent<DropInfo>();
        if (_playerInventory == null) return;
        if (dropInfo == null) return;

        leftOverItems = _playerInventory.AddItem(dropInfo.ItemData);

        if (leftOverItems != 0)
        {
            dropInfo.SetAmount(leftOverItems);
            _isBeingSuck = false;
            _currentSpeed = 0;
            _dropSystem.enabled = true;
            Debug.Log("Got it");
        }
        else
        {
            this.transform.parent.gameObject.SetActive(false);
        }
    }
}
