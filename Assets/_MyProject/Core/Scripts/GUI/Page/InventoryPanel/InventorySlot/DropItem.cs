using UnityEngine;
using System;

public class DropItem : MonoBehaviour
{
    private InventoryController _inventoryController;

    private void Awake()
    {
        _inventoryController = GetComponentInParent<InventoryController>();
    }
    public void OnDrop()
    {
        if (_inventoryController.Player == null) return;
        if (_inventoryController.CurrentSelectedIndex == -1) return;
        ItemInstance item = _inventoryController.CurrentSelectedItem;
        if (item == null || item.DropPrefab == null)
        {
            Debug.LogWarning("item || DropPrefab == null");
            return;
        }
        Debug.Log(item.ItemDefinition.ItemName);
        Debug.Log(item.ItemDefinition.ItemID);
        Debug.Log(item.Amount);
        Debug.Log(item.ItemDefinition);
        GameObject orbDrop = Instantiate(item.DropPrefab, _inventoryController.Player.PlayerTransform.position, Quaternion.identity);
        if (orbDrop != null)
        {
            DropInfo dropInfo = orbDrop.GetComponent<DropInfo>();
            if (dropInfo == null) return;
            dropInfo.Initialize(item);
        }
        _inventoryController.PlayerInventory.DropItem(item);
        _inventoryController.UpdateSlot(_inventoryController.CurrentSelectedIndex, null);
        _inventoryController.ClearSelectedItem();
    }
}
