using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject ItemContainer;
    public Image _image;
    public TMP_Text _text;
    public CanvasGroup IconCanvasGroup;

    public event Action<int> SelectedItem;

    protected Transform _dragLayer;
    protected InventoryController _inventoryController;
    protected Transform _originalParent;

    protected int _thisIndex;

    protected void Awake()
    {
        _dragLayer = GameObject.FindGameObjectWithTag(TagConstant.TagDragLayer).transform;
        _inventoryController = GetComponentInParent<InventoryController>();
    }
    protected void OnEnable()
    {
        if (_inventoryController != null) _inventoryController.OnSlotDataChanged += UpdateSlot;
    }
    protected void OnDisable()
    {
        if (_inventoryController != null) _inventoryController.OnSlotDataChanged -= UpdateSlot;
        if (TooltipManager.Instance != null && gameObject.activeInHierarchy)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
    protected void Start()
    {
        _thisIndex = transform.GetSiblingIndex();
    }
    
    public void UpdateSlot(ItemInstance itemInstance)
    {
        if (!ItemContainer) return;
        if (itemInstance == null || itemInstance.ItemDefinition == null || itemInstance.Amount <= 0)
        {
            ClearSlot();
            return;
        }
        ItemContainer.SetActive(true);
        //Debug.Log("Icon " + itemInstance.ItemDefinition.ItemIcon.name);
        //Debug.Log("Amount " + itemInstance.Amount);
        _image.sprite = itemInstance.ItemDefinition.ItemIcon;
        _text.text = itemInstance.Amount > 1 ? itemInstance.Amount.ToString() : "";
    }
    public void ClearSlot()
    {
        if (!_image) return;
        if (!_text) return;
        if (!ItemContainer) return;

        _image.sprite = null;
        ItemContainer.SetActive(false);
        _text.text = "";
    }
    public void OnSelected()
    {
        Debug.Log("Selected");
        SelectedItem?.Invoke(_thisIndex);
    }
    public int GetIndex() => _thisIndex;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!ItemContainer.activeSelf) return;
        if (_image.sprite == null) return;
        _originalParent = ItemContainer.transform.parent;
        ItemContainer.transform.SetParent(_dragLayer);

        IconCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!ItemContainer.activeSelf) return;
        if (_image.sprite == null) return;
        ItemContainer.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IconCanvasGroup.blocksRaycasts = true;
        ItemContainer.transform.SetParent(_originalParent);
        ItemContainer.transform.localPosition = Vector3.zero;
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject != null && draggedObject.TryGetComponent(out InventorySlot sourceSlot))
        {
            if (sourceSlot == this) return;

            if (sourceSlot is EquipmentSlotUI sourceEquipmentSlot)
            {
                Debug.Log("Equipment ne");
                _inventoryController.ProcessEquipmentChange(sourceEquipmentSlot.SlotType, false, _thisIndex, sourceEquipmentSlot.GetIndex());
            }
            else
            {
                Debug.Log("Inventory thuong ne");
                _inventoryController.ProcessDragAndDrop(sourceSlot.GetIndex(), _thisIndex);
            }
        }
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (_inventoryController == null) return;

        ItemInstance invItem = _inventoryController.PlayerInventory.BindingItem(
            _inventoryController.PlayerInventory.PlayerInventory[_thisIndex]
        );

        if (invItem == null || invItem.ItemDefinition == null) return;

        TooltipManager.Instance.ShowTooltip(invItem);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.HideTooltip();
    }
}
