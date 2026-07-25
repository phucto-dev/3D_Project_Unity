using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("--- UI REF ---")]
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _mainStat;
    [SerializeField] private TMP_Text _subStat;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _rarity;
    [SerializeField] private TMP_Text _quantity;

    private RectTransform _rectTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }
    public void ShowTooltip(ItemInstance invItem)
    {
        if (invItem == null || invItem.ItemDefinition == null) return;

        if (invItem is EquipmentInstance equipItem)
        {
            EquipmentDataSO equipType = equipItem.GetEquipData();
            if (equipType != null)
            {
                string hexColor = GetRarityHexColor(equipItem.Rarity);

                _nameText.SetText($"<color={hexColor}>{equipType.ItemName}</color>");
                _mainStat.SetText($"{equipType.MainStat.Type}: {equipType.MainStat.Value}");
                _subStat.SetText($"{equipType.SubStat.Type}: {equipType.SubStat.Value}");
                _descriptionText.SetText(string.IsNullOrEmpty(equipType.Description) ? "???" : equipType.Description);
                _rarity.SetText($"Rarity: <color={hexColor}>{equipItem.Rarity}</color>");
                _quantity.SetText("Quantity: 1");
            }
        }
        else
        {
            ItemDefinitionSO itemInfo = invItem.ItemDefinition;

            _nameText.SetText(itemInfo.ItemName);
            _mainStat.SetText("");
            _subStat.SetText("");
            _descriptionText.SetText(string.IsNullOrEmpty(itemInfo.Description) ? "???" : itemInfo.Description);
            _rarity.SetText("");
            _quantity.SetText("Quantity: {0}", invItem.Amount);
        }

        FollowMousePosition();
        gameObject.SetActive(true);
    }
    private string GetRarityHexColor(EquipmentRarity rarity)
    {
        return rarity switch
        {
            EquipmentRarity.Common => "#FFFFFF",
            EquipmentRarity.Rare => "#0070DD",
            EquipmentRarity.Unique => "#A335EE",
            EquipmentRarity.Legendary => "#FF8000",
            EquipmentRarity.Mythic => "#FF0000",
            _ => "#FFFFFF"
        };
    }
    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
    private void FollowMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;

        _rectTransform.pivot = new Vector2(
            pivotX > 0.5f ? 1.05f : -0.05f,
            pivotY > 0.5f ? 1.05f : -0.05f
        );

        _rectTransform.position = mousePos;
    }
}
