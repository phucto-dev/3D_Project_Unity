using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLootManager : MonoBehaviour
{
    private PlayerInput _inputSystem;
    private InputAction _lootAction;
    private PlayerInventoryManager _playerInventory;
    private List<DropInfo> _listDrop;

    private void Awake()
    {
        _inputSystem = GetComponentInParent<PlayerInput>();
        if (_inputSystem == null) return;
        _lootAction = _inputSystem.actions["Loot"];
    }
    private void OnEnable()
    {
        _lootAction.performed += HandleLootInput;
    }
    private void OnDisable()
    {
        _lootAction.performed -= HandleLootInput;
    }
    private void Start()
    {
        _playerInventory = GetComponentInParent<PlayerInventoryManager>();
        _listDrop = new List<DropInfo>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagDropItem) && other.TryGetComponent<DropInfo>(out DropInfo item))
        {
            _listDrop.Add(item);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(TagConstant.TagDropItem) && other.TryGetComponent<DropInfo>(out DropInfo item))
        {
            _listDrop.Remove(item);
        }
    }
    private void HandleLootInput(InputAction.CallbackContext ctx)
    {
        LootItem();
    }

    private void LootItem()
    {
        if (_listDrop.Count <= 0) return;
        int leftOverItems;
        if (_playerInventory == null) return;

        leftOverItems = _playerInventory.AddItem(_listDrop[0].ItemData);

        if (leftOverItems != 0)
        {
            _listDrop[0].SetAmount(leftOverItems);
            Debug.Log("Inventory full");
            
        }
        else
        {
            _listDrop[0].RemovePrefab();
            _listDrop.RemoveAt(0);
            Debug.Log("Got it");
        }
    }
}
