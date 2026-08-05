using System;
using TMPro;
using UnityEngine;

public enum StatsCheatType
{
    Attack,
    Health,
    Mana
}
public class StatsCheatButton : MonoBehaviour
{
    public StatsCheatType CheatType;
    public TMP_InputField InputText;

    public event Action<StatsCheatType, float> OnHandleStatCheatClick;
    private void OnEnable()
    {
        OnHandleStatCheatClick += GameManager.Instance.HandleCheatButton;
    }
    private void OnDisable()
    {
        OnHandleStatCheatClick -= GameManager.Instance.HandleCheatButton;
    }

    public void OnHandleClick()
    {
        Debug.Log("Log1");
        if (InputText == null) return;
        Debug.Log("Log2");

        // Ép TextMeshPro cập nhật lại dữ liệu đang nhập từ bàn phím ảo/thực tế ngay lập tức
        InputText.ForceLabelUpdate();

        // Lấy chuỗi từ input, nếu rỗng thì có thể quét qua textComponent con đề phòng buffer chưa ăn
        string rawText = InputText.text;
        if (string.IsNullOrEmpty(rawText) && InputText.textComponent != null)
        {
            rawText = InputText.textComponent.text;
        }

        float result = 0f;
        if (!string.IsNullOrEmpty(rawText) && float.TryParse(rawText.Trim(), out float parsedValue))
        {
            result = parsedValue;
            Debug.Log("Log3 - Parse thành công: " + result);
        }
        else
        {
            Debug.LogWarning("Log3 - Parse thất bại, dùng giá trị mặc định: 0");
        }

        OnHandleStatCheatClick?.Invoke(CheatType, result);
    }
}
