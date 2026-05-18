using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image _image;
    public TMP_Text _text;

    public void UpdateSlot(ItemInstance itemInstance)
    {
        if (itemInstance == null || itemInstance.ItemDefinition == null || itemInstance.Amount <= 0)
        {
            ClearSlot();
            return;
        }
        _image.transform.gameObject.SetActive(true);
        Debug.Log("Icon " + itemInstance.ItemDefinition.ItemIcon.name);
        Debug.Log("Amount " + itemInstance.Amount);
        _image.sprite = itemInstance.ItemDefinition.ItemIcon;
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
