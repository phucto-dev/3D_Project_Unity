using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;

    private void Start()
    {
        _image = GetComponentInChildren<Image>();
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateSlot(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.ItemDefinition == null || itemInstance.Amount <= 0)
        {
            ClearSlot();
            return;
        }

        _image.sprite = itemInstance.ItemDefinition.ItemIcon;
        _image.transform.gameObject.SetActive(true);
        _text.text = itemInstance.Amount > 1 ? itemInstance.Amount.ToString() : "";
        
    }
    public void ClearSlot()
    {
        if (!_image) return;
        if (!_text) return;

        _image.sprite = null;
        _image.transform.gameObject.SetActive(false);
        _text.text = "";
    }
}
