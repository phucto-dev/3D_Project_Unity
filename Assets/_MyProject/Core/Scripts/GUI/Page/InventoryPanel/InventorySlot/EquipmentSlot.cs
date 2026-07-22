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
}
