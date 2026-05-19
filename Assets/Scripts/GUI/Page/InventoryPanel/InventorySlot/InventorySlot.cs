using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public GameObject ItemContainer;
    public Image _image;
    public TMP_Text _text;
    public CanvasGroup IconCanvasGroup;

    private Transform _dragLayer;
    private InventoryController _inventoryController;
    private Transform _originalParent;

    private int _thisIndex;

    private void Awake()
    {
        _dragLayer = GameObject.FindGameObjectWithTag(TagConstant.TagDragLayer).transform;
        _inventoryController = GetComponentInParent<InventoryController>();
    }
    private void OnEnable()
    {
        if (_inventoryController != null) _inventoryController.OnSlotDataChanged += UpdateSlot;
    }
    private void OnDisable()
    {
        if (_inventoryController != null) _inventoryController.OnSlotDataChanged -= UpdateSlot;
    }
    private void Start()
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
        Debug.Log("Icon " + itemInstance.ItemDefinition.ItemIcon.name);
        Debug.Log("Amount " + itemInstance.Amount);
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

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject != null && draggedObject.TryGetComponent(out InventorySlot sourceSlotUI))
        {
            if (sourceSlotUI == this) return;
            _inventoryController.ProcessDragAndDrop(sourceSlotUI.GetIndex(), _thisIndex);
        }
    }
}
