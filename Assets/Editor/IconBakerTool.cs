using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class IconBakerTool : EditorWindow
{
    [Header("Cài đặt chung")]
    public Camera renderCamera;
    public int iconSize = 256;

    [Header("Chế độ Đơn")]
    public ArmorDataSO singleArmorData;

    [Header("Chế độ Hàng loạt")]
    public List<ArmorDataSO> armorDataList = new List<ArmorDataSO>();

    // Cầu nối cho List
    private SerializedObject serializedObj;
    private SerializedProperty armorDataListProp;

    [MenuItem("PRO Tools/Icon Baker Studio")]
    public static void ShowWindow()
    {
        GetWindow<IconBakerTool>("Icon Baker");
    }

    private void OnEnable()
    {
        serializedObj = new SerializedObject(this);
        armorDataListProp = serializedObj.FindProperty("armorDataList");
    }

    private void OnGUI()
    {
        GUILayout.Label("CÀI ĐẶT CHUNG", EditorStyles.boldLabel);
        renderCamera = (Camera)EditorGUILayout.ObjectField("Camera Chụp", renderCamera, typeof(Camera), true);
        iconSize = EditorGUILayout.IntField("Kích thước (px)", iconSize);

        DrawLine();

        // --- KHU VỰC 1: CHẾ ĐỘ ĐƠN ---
        GUILayout.Label("CHẾ ĐỘ ĐƠN (SINGLE BAKE)", EditorStyles.boldLabel);
        singleArmorData = (ArmorDataSO)EditorGUILayout.ObjectField("File SO (Mẫu)", singleArmorData, typeof(ArmorDataSO), false);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Chụp V1 (Chỉnh tay)", GUILayout.Height(30)))
        {
            if (CheckSingleBakeReady()) BakeIcon();
        }
        if (GUILayout.Button("Chụp V2 (Auto-Focus)", GUILayout.Height(30)))
        {
            if (CheckSingleBakeReady()) BakeIcon_v2();
        }
        GUILayout.EndHorizontal();

        DrawLine();

        // --- KHU VỰC 2: CHẾ ĐỘ HÀNG LOẠT ---
        GUILayout.Label("CHẾ ĐỘ HÀNG LOẠT (BATCH BAKE)", EditorStyles.boldLabel);

        serializedObj.Update();
        EditorGUILayout.PropertyField(armorDataListProp, new GUIContent("Danh sách SO"), true);
        serializedObj.ApplyModifiedProperties();

        GUILayout.Space(5);

        if (GUILayout.Button("Chụp Tất Cả Danh Sách (Sử dụng logic V2)", GUILayout.Height(40)))
        {
            if (renderCamera == null)
            {
                Debug.LogError("[LỖI] Thiếu Camera!");
                return;
            }
            if (armorDataList.Count == 0)
            {
                Debug.LogWarning("[CẢNH BÁO] Danh sách trống!");
                return;
            }
            BakeAllIcons();
        }

        if (GUILayout.Button("Clear Danh Sách", GUILayout.Height(20)))
        {
            armorDataList.Clear();
        }
    }

    private void DrawLine()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.Space(10);
    }

    private bool CheckSingleBakeReady()
    {
        if (renderCamera == null || singleArmorData == null)
        {
            Debug.LogError("[LỖI] Thiếu Camera hoặc chưa gắn File SO Đơn!");
            return false;
        }
        return true;
    }

    // ====================================================================
    // HÀM CŨ 1: BAKE ICON (THỦ CÔNG) - Giữ nguyên yêu cầu cá nhân
    // ====================================================================
    private void BakeIcon()
    {
        GameObject dummy = new GameObject("DummyProp");
        dummy.transform.position = Vector3.zero;

        MeshFilter mf = dummy.AddComponent<MeshFilter>();
        MeshRenderer mr = dummy.AddComponent<MeshRenderer>();

        mf.sharedMesh = singleArmorData.ArmorMesh;
        mr.sharedMaterial = singleArmorData.ArmorMaterial;

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

        byte[] bytes = screenShot.EncodeToPNG();
        string soPath = AssetDatabase.GetAssetPath(singleArmorData);
        string folderPath = Path.GetDirectoryName(soPath);
        string pngPath = $"{folderPath}/{singleArmorData.name}_Icon.png";

        File.WriteAllBytes(pngPath, bytes);
        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        Sprite generatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        if (generatedSprite != null)
        {
            singleArmorData.ItemIcon = generatedSprite;
            EditorUtility.SetDirty(singleArmorData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[PRO Tool V1] Đã tạo thành công cho: {singleArmorData.name}");
        }
    }

    // ====================================================================
    // HÀM CŨ 2: BAKE ICON V2 (AUTO-FOCUS) - Giữ nguyên yêu cầu cá nhân
    // ====================================================================
    private void BakeIcon_v2()
    {
        GameObject dummy = new GameObject("DummyProp");
        MeshFilter mf = dummy.AddComponent<MeshFilter>();
        MeshRenderer mr = dummy.AddComponent<MeshRenderer>();

        mf.sharedMesh = singleArmorData.ArmorMesh;
        mr.sharedMaterial = singleArmorData.ArmorMaterial;

        renderCamera.orthographic = true;
        Bounds bounds = mf.sharedMesh.bounds;
        dummy.transform.position = -bounds.center;

        float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        renderCamera.orthographicSize = maxExtent * 1.2f;

        renderCamera.transform.position = new Vector3(0, 0, -10f);
        renderCamera.transform.LookAt(Vector3.zero);

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

        byte[] bytes = screenShot.EncodeToPNG();
        string soPath = AssetDatabase.GetAssetPath(singleArmorData);
        string folderPath = Path.GetDirectoryName(soPath);
        string pngPath = $"{folderPath}/{singleArmorData.name}_Icon.png";

        File.WriteAllBytes(pngPath, bytes);
        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        Sprite generatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        if (generatedSprite != null)
        {
            singleArmorData.ItemIcon = generatedSprite;
            EditorUtility.SetDirty(singleArmorData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[PRO Tool V2] Đã auto-focus và tạo thành công cho: {singleArmorData.name}");
        }
    }

    // ====================================================================
    // HÀM MỚI: BAKE HÀNG LOẠT (DÙNG LOGIC AUTO-FOCUS V2)
    // ====================================================================
    private void BakeAllIcons()
    {
        int total = armorDataList.Count;

        for (int i = 0; i < total; i++)
        {
            ArmorDataSO data = armorDataList[i];
            if (data == null) continue;

            EditorUtility.DisplayProgressBar("Đang nướng Icon...", $"Đang xử lý: {data.name} ({i + 1}/{total})", (float)i / total);
            BakeSingleIconForBatch(data);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        Debug.Log($"[PRO Tool Batch] Đã hoàn thành nướng hàng loạt {total} Icon!");
    }

    private void BakeSingleIconForBatch(ArmorDataSO targetData)
    {
        if (targetData.ArmorMesh == null) return;

        GameObject dummy = new GameObject("DummyProp");
        MeshFilter mf = dummy.AddComponent<MeshFilter>();
        MeshRenderer mr = dummy.AddComponent<MeshRenderer>();

        mf.sharedMesh = targetData.ArmorMesh;
        mr.sharedMaterial = targetData.ArmorMaterial;

        renderCamera.orthographic = true;
        Bounds bounds = mf.sharedMesh.bounds;
        dummy.transform.position = -bounds.center;

        float maxExtent = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z);
        renderCamera.orthographicSize = maxExtent * 1.2f;

        renderCamera.transform.position = new Vector3(0, 0, -10f);
        renderCamera.transform.LookAt(Vector3.zero);

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

        byte[] bytes = screenShot.EncodeToPNG();
        string soPath = AssetDatabase.GetAssetPath(targetData);
        string folderPath = Path.GetDirectoryName(soPath);
        string pngPath = $"{folderPath}/{targetData.name}_Icon.png";

        File.WriteAllBytes(pngPath, bytes);
        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(pngPath);
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        AssetDatabase.ImportAsset(pngPath, ImportAssetOptions.ForceUpdate);

        Sprite generatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngPath);
        if (generatedSprite != null)
        {
            targetData.ItemIcon = generatedSprite;
            EditorUtility.SetDirty(targetData);
        }
    }
}