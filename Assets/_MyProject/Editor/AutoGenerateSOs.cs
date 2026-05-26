using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoGenerateSOs : MonoBehaviour
{
    // Cần tạo sẵn thư mục "Assets/GameData/Items/Armor" trước khi chạy tool
    private const string savePath = "Assets/GameData/Items/Armor";

    [MenuItem("PRO Tools/Generate 90 Armor SOs")]
    public static void GenerateArmorSOs()
    {
        // Trỏ đường dẫn tới file txt của bạn
        string txtPath = EditorUtility.OpenFilePanel("Chọn file ArmorList.txt", Application.dataPath, "txt");
        if (string.IsNullOrEmpty(txtPath)) return;

        string[] lines = File.ReadAllLines(txtPath);
        int count = 0;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

            string[] data = line.Split(',');
            if (data.Length >= 2)
            {
                string id = data[0].Trim();
                string displayName = data[1].Trim();

                // KHỞI TẠO FILE SO MỚI (đảm bảo class ArmorDataSO của bạn đang tồn tại)
                ArmorDataSO newArmor = ScriptableObject.CreateInstance<ArmorDataSO>();

                // Gán sẵn ID và Tên
                newArmor.ItemID = id;
                newArmor.ItemName = displayName;

                // Tự động phân loại SlotType dựa trên chuỗi chữ
                if (id.Contains("_Head_")) newArmor.SlotType = EquipmentSlot.Head;
                else if (id.Contains("_Chest_")) newArmor.SlotType = EquipmentSlot.Chest;
                else if (id.Contains("_Arms_")) newArmor.SlotType = EquipmentSlot.Arms;
                else if (id.Contains("_Belt_")) newArmor.SlotType = EquipmentSlot.Belt;
                else if (id.Contains("_Legs_")) newArmor.SlotType = EquipmentSlot.Legs;
                else if (id.Contains("_Feet_")) newArmor.SlotType = EquipmentSlot.Feet;

                // LƯU THÀNH FILE .asset XUỐNG Ổ CỨNG
                string assetPath = $"{savePath}/{id}.asset";
                AssetDatabase.CreateAsset(newArmor, assetPath);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[PRO Tool] Đã tạo thành công {count} file ScriptableObject tại: {savePath}");
    }
}