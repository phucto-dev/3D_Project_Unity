using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : InventorySlot
{
    public EquipmentSlot SlotType;
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject != null && draggedObject.TryGetComponent(out InventorySlot sourceSlotUI))
        {
            _inventoryController.ProcessEquipmentChange(SlotType, true, sourceSlotUI.GetIndex(), _thisIndex);
        }
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (_inventoryController == null) return;

        EquipmentInstance invItem = _inventoryController.PlayerEquipment.EquippedItems[SlotType];

        if (invItem == null || invItem.ItemDefinition == null) return;

        TooltipManager.Instance.ShowTooltip(invItem);
    }
}
