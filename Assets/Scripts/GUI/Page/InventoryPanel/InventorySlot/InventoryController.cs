using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public PlayerInventorySO PlayerInventory;
    public GameObject SlotContainer;

    private InventorySlot[] _listSlot;
    private void Awake()
    {
        if (SlotContainer == null) return;
        _listSlot = SlotContainer.GetComponentsInChildren<InventorySlot>(true);
    }
    private void OnEnable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated += UpdateSlot;
    }
    private void OnDisable()
    {
        if (PlayerInventory == null) return;
        PlayerInventory.OnSlotUpdated -= UpdateSlot;
    }
    private void Start()
    {
        if (PlayerInventory == null) return;
        RefreshUI(PlayerInventory.PlayerInventory);
    }
    public void UpdateSlot(int i, ItemInstance slot)
    {
        if (_listSlot == null || _listSlot.Length == 0) return;
        _listSlot[i].UpdateSlot(slot);
    }
    public void RefreshUI(ItemInstance[] inventorySlots)
    {
        if (_listSlot == null || _listSlot.Length == 0) return;
        if (_listSlot.Length != inventorySlots.Length) return;

        for (int i = 0; i < _listSlot.Length; i++)
        {
            _listSlot[i].UpdateSlot(inventorySlots[i]);
        }
    }
}
