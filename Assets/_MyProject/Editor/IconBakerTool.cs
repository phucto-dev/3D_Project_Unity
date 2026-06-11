using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class IconBakerTool : EditorWindow
{
    public enum BakeMode { Armor, Weapon }

    [Header("Cài đặt chung")]
    public Camera renderCamera;
    public int iconSize = 256;
    public BakeMode currentMode = BakeMode.Armor;

    [Header("Dữ liệu Armor")]
    public ArmorDataSO singleArmorData;
    public List<ArmorDataSO> armorDataList = new List<ArmorDataSO>();

    [Header("Dữ liệu Weapon")]
    public WeaponDataSO singleWeaponData;
    public List<WeaponDataSO> weaponDataList = new List<WeaponDataSO>();

    private SerializedObject serializedObj;
    private SerializedProperty armorDataListProp;
    private SerializedProperty weaponDataListProp;

    [MenuItem("PRO Tools/Icon Baker Studio")]
    public static void ShowWindow()
    {
        GetWindow<IconBakerTool>("Icon Baker");
    }

    private void OnEnable()
    {
        serializedObj = new SerializedObject(this);
        armorDataListProp = serializedObj.FindProperty("armorDataList");
        weaponDataListProp = serializedObj.FindProperty("weaponDataList");
    }

    private void OnGUI()
    {
        GUILayout.Label("CÀI ĐẶT CHUNG", EditorStyles.boldLabel);
        renderCamera = (Camera)EditorGUILayout.ObjectField("Camera Chụp", renderCamera, typeof(Camera), true);
        iconSize = EditorGUILayout.IntField("Kích thước (px)", iconSize);

        GUILayout.Space(5);
        currentMode = (BakeMode)EditorGUILayout.EnumPopup("Loại Item (Mode)", currentMode);

        DrawLine();
        serializedObj.Update();

        if (currentMode == BakeMode.Armor) DrawArmorGUI();
        else DrawWeaponGUI();

        serializedObj.ApplyModifiedProperties();
    }

    private void DrawLine()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);
    }

    // ====================================================================
    // GIAO DIỆN & ĐIỀU HƯỚNG
    // ====================================================================
    private void DrawArmorGUI()
    {
        GUILayout.Label("CHẾ ĐỘ ĐƠN - ARMOR", EditorStyles.boldLabel);
        singleArmorData = (ArmorDataSO)EditorGUILayout.ObjectField("File SO (Mẫu)", singleArmorData, typeof(ArmorDataSO), false);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Chụp V1 (Chỉnh tay)", GUILayout.Height(30)) && CheckReady(singleArmorData))
            ProcessBake(singleArmorData, false, true);

        if (GUILayout.Button("Chụp V2 (Auto-Focus)", GUILayout.Height(30)) && CheckReady(singleArmorData))
            ProcessBake(singleArmorData, true, true);
        GUILayout.EndHorizontal();

        DrawLine();

        GUILayout.Label("CHẾ ĐỘ HÀNG LOẠT - ARMOR", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(armorDataListProp, new GUIContent("Danh sách SO"), true);

        if (GUILayout.Button("Chụp Tất Cả (Auto-Focus)", GUILayout.Height(40)))
            BakeBatch(armorDataList);
    }

    private void DrawWeaponGUI()
    {
        GUILayout.Label("CHẾ ĐỘ ĐƠN - WEAPON", EditorStyles.boldLabel);
        singleWeaponData = (WeaponDataSO)EditorGUILayout.ObjectField("File SO (Vũ khí)", singleWeaponData, typeof(WeaponDataSO), false);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Chụp V1 (Chỉnh tay)", GUILayout.Height(30)) && CheckReady(singleWeaponData))
            ProcessBake(singleWeaponData, false, false);

        if (GUILayout.Button("Chụp V2 (Auto-Focus)", GUILayout.Height(30)) && CheckReady(singleWeaponData))
            ProcessBake(singleWeaponData, true, false);
        GUILayout.EndHorizontal();

        DrawLine();

        GUILayout.Label("CHẾ ĐỘ HÀNG LOẠT - WEAPON", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(weaponDataListProp, new GUIContent("Danh sách SO"), true);

        if (GUILayout.Button("Chụp Tất Cả (Auto-Focus)", GUILayout.Height(40)))
            BakeBatch(weaponDataList);
    }

    private bool CheckReady(ScriptableObject data)
    {
        if (renderCamera == null || data == null)
        {
            Debug.LogError("[LỖI] Thiếu Camera hoặc chưa gắn File SO!");
            return false;
        }
        return true;
    }

    // ====================================================================
    // CORE PIPELINE (XỬ LÝ DỮ LIỆU & RENDER)
    // ====================================================================
    private void BakeBatch<T>(List<T> list) where T : ScriptableObject
    {
        if (renderCamera == null || list.Count == 0) return;

        bool isArmor = typeof(T) == typeof(ArmorDataSO);
        int total = list.Count;

        for (int i = 0; i < total; i++)
        {
            if (list[i] == null) continue;
            EditorUtility.DisplayProgressBar("Đang nướng Icon...", $"Đang xử lý: {list[i].name} ({i + 1}/{total})", (float)i / total);
            ProcessBake(list[i], true, isArmor);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        Debug.Log($"[PRO Tool] Đã hoàn thành nướng hàng loạt {total} Icon!");
    }

    private void ProcessBake(ScriptableObject data, bool isAutoFocus, bool isArmor)
    {
        GameObject dummy = null;

        // BƯỚC 1: Khởi tạo Dummy Model
        if (isArmor)
        {
            ArmorDataSO armorData = data as ArmorDataSO;
            if (armorData.EquipmentMesh == null) return;

            dummy = new GameObject("DummyProp");
            dummy.AddComponent<MeshFilter>().sharedMesh = armorData.EquipmentMesh;
            dummy.AddComponent<MeshRenderer>().sharedMaterial = armorData.EquipmentMaterial;
        }
        else
        {
            WeaponDataSO weaponData = data as WeaponDataSO;
            if (weaponData.EquippedPrefab == null) return;

            dummy = Instantiate(weaponData.EquippedPrefab);
            dummy.name = "DummyProp";

            // Tắt các hiệu ứng hạt (VFX) không mong muốn dính vào Icon
            ParticleSystem[] particles = dummy.GetComponentsInChildren<ParticleSystem>();
            foreach (var ps in particles) ps.gameObject.SetActive(false);
        }

        dummy.transform.position = Vector3.zero;

        // BƯỚC 2: Căn chỉnh Camera
        if (isAutoFocus)
        {
            renderCamera.orthographic = true;
            Bounds bounds = CalculateBounds(dummy);

            dummy.transform.position = -bounds.center;
            float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            renderCamera.orthographicSize = maxExtent * 1.2f;

            renderCamera.transform.position = new Vector3(0, 0, -10f);
            renderCamera.transform.LookAt(Vector3.zero);
        }

        // BƯỚC 3: Chụp ảnh & Lưu dữ liệu
        CaptureAndSave(data, dummy);
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        // Quét toàn bộ Renderer (Kể cả MeshRenderer và SkinnedMeshRenderer của Prefab)
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }

    private void CaptureAndSave(ScriptableObject targetData, GameObject dummy)
    {
        RenderTexture rt = new RenderTexture(iconSize, iconSize, 24);
        renderCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(iconSize, iconSize, TextureFormat.RGBA32, false);

        renderCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, iconSize, iconSize), 0, 0);
        screenShot.Apply();

        renderCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(dummy);

        // Lưu file PNG
        byte[] bytes = screenShot.EncodeToPNG();
        string soPath = AssetDatabase.GetAssetPath(targetData);
        string folderPath = Path.GetDirectoryName(soPath);
        string pngPath = $"{folderPath}/{targetData.name}_Icon.png";

        File.WriteAllBytes(pngPath, bytes);
        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        // Cấu hình Texture Importer
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        Sprite generatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        if (generatedSprite != null)
        {
            // Sử dụng SerializedObject để gán biến động, không cần cast cụ thể kiểu class
            SerializedObject so = new SerializedObject(targetData);
            SerializedProperty iconProp = so.FindProperty("ItemIcon");
            if (iconProp != null)
            {
                iconProp.objectReferenceValue = generatedSprite;
                so.ApplyModifiedProperties();
            }
        }
    }
}