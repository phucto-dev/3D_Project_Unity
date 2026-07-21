using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// ==========================================
// 1. CORE CAPTURE UTILITY (Tách lõi chụp ảnh để dùng chung)
// ==========================================
public static class CameraCaptureUtility
{
    public static void TakeSnapshotAndSave(Camera cam, int resolution, GameObject prefabReference)
    {
        // 1. Setup Camera chụp nền trong suốt
        Color originalColor = cam.backgroundColor;
        CameraClearFlags originalFlags = cam.clearFlags;
        RenderTexture originalRT = cam.targetTexture;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);

        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resolution, resolution, TextureFormat.ARGB32, false);

        // 2. Render & Đọc Pixel
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        screenShot.Apply();

        // 3. Khôi phục trạng thái Camera
        cam.targetTexture = originalRT;
        RenderTexture.active = null;
        cam.clearFlags = originalFlags;
        cam.backgroundColor = originalColor;
        Object.Destroy(rt);

        // 4. Lưu file PNG
#if UNITY_EDITOR
        if (prefabReference != null)
        {
            byte[] bytes = screenShot.EncodeToPNG();
            string prefabPath = AssetDatabase.GetAssetPath(prefabReference);
            if (!string.IsNullOrEmpty(prefabPath))
            {
                string folderPath = Path.GetDirectoryName(prefabPath);
                string savePath = Path.Combine(folderPath, prefabReference.name + "_Icon.png");
                File.WriteAllBytes(savePath, bytes);
                Debug.Log($"<color=cyan>[VFX Capture]</color> Đã lưu icon (Chụp lập tức): {savePath}");
            }
        }
#endif
    }
}

// ==========================================
// 2. COMMANDS
// ==========================================
public interface ICaptureCommand
{
    IEnumerator Execute();
}

// Lệnh chụp Delay (Dùng cho Capture All)
public class VFXCaptureCommand : ICaptureCommand
{
    private GameObject prefab;
    private Camera cam;
    private float delay;
    private int resolution;

    private Vector3 spawnPoint;
    private Vector3 targetOffset;
    private float cameraDistance;
    private float cameraPitch;
    private float cameraYaw;
    private float cameraFOV;

    public VFXCaptureCommand(GameObject prefab, Camera cam, float delay, int resolution,
                             Vector3 spawnPoint, Vector3 targetOffset, float distance, float pitch, float yaw, float fov)
    {
        this.prefab = prefab;
        this.cam = cam;
        this.delay = delay;
        this.resolution = resolution;

        this.spawnPoint = spawnPoint;
        this.targetOffset = targetOffset;
        this.cameraDistance = distance;
        this.cameraPitch = pitch;
        this.cameraYaw = yaw;
        this.cameraFOV = fov;
    }

    public IEnumerator Execute()
    {
        if (prefab == null) yield break;

        GameObject instance = Object.Instantiate(prefab, spawnPoint, Quaternion.identity);
        Vector3 lookTarget = spawnPoint + targetOffset;

        Vector3 oldCamPos = cam.transform.position;
        Quaternion oldCamRot = cam.transform.rotation;
        float oldFOV = cam.fieldOfView;

        Quaternion camRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        cam.transform.position = lookTarget + camRotation * new Vector3(0f, 0f, -cameraDistance);
        cam.transform.LookAt(lookTarget);
        cam.fieldOfView = cameraFOV;

        yield return new WaitForSeconds(delay);

        CameraCaptureUtility.TakeSnapshotAndSave(cam, resolution, prefab);

        cam.transform.position = oldCamPos;
        cam.transform.rotation = oldCamRot;
        cam.fieldOfView = oldFOV;

        Object.Destroy(instance);
    }
}

// Lệnh chụp Lập Tức (Chụp ngay khung hình hiện tại)
public class InstantSnapshotCommand : ICaptureCommand
{
    private Camera cam;
    private int resolution;
    private GameObject prefabReference;

    public InstantSnapshotCommand(Camera cam, int resolution, GameObject prefabReference)
    {
        this.cam = cam;
        this.resolution = resolution;
        this.prefabReference = prefabReference;
    }

    public IEnumerator Execute()
    {
        // Đợi đến cuối frame để đảm bảo Unity đã render xong mọi Particle
        yield return new WaitForEndOfFrame();
        CameraCaptureUtility.TakeSnapshotAndSave(cam, resolution, prefabReference);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }
}

// ==========================================
// 3. MAIN TOOL MANAGER
// ==========================================
public class VFXIconCaptureTool : MonoBehaviour
{
    [Header("Core Settings")]
    public Camera captureCamera;
    public float captureDelay = 1.0f;
    public int iconResolution = 512;

    [Header("Time Control")]
    [Range(0f, 3f)]
    public float timeScaleMultiplier = 1.0f;

    [Header("Framing & Angle Settings")]
    public Vector3 targetOffset = new Vector3(0f, 2.0f, 0f);
    public float cameraDistance = 15f;
    [Range(0f, 85f)] public float cameraPitch = 40f;
    [Range(-180f, 180f)] public float cameraYaw = 0f;
    [Range(5f, 120f)] public float cameraFOV = 30f;

    [Header("Input Data")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Header("Preview Setup")]
    public int previewIndex = 0;
    private GameObject currentPreview;
    private bool isProcessingQueue = false;

    // --- Time Control Methods ---
    public void SetWorldTimeScale()
    {
        Time.timeScale = timeScaleMultiplier;
        Debug.Log($"<color=orange>[Time Control]</color> World Time Scale được set thành: {Time.timeScale}");
    }

    public void ResetWorldTime()
    {
        timeScaleMultiplier = 1.0f;
        Time.timeScale = 1.0f;
        Debug.Log($"<color=orange>[Time Control]</color> Đã Reset Time Scale về 1.0");
    }

    // --- Action Methods ---
    public void PreviewSkill()
    {
        if (currentPreview != null) Destroy(currentPreview);
        if (prefabs == null || prefabs.Count == 0) return;

        previewIndex = Mathf.Clamp(previewIndex, 0, prefabs.Count - 1);
        if (prefabs[previewIndex] != null)
        {
            Vector3 spawnPoint = this.transform.position;
            Vector3 lookTarget = spawnPoint + targetOffset;

            currentPreview = Instantiate(prefabs[previewIndex], spawnPoint, Quaternion.identity);

            Quaternion camRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            captureCamera.transform.position = lookTarget + camRotation * new Vector3(0f, 0f, -cameraDistance);
            captureCamera.transform.LookAt(lookTarget);
            captureCamera.fieldOfView = cameraFOV;
        }
    }

    // Chụp Instant (Khoảnh khắc hiện tại)
    public void InstantCapture()
    {
        if (prefabs == null || prefabs.Count == 0 || previewIndex >= prefabs.Count) return;
        if (prefabs[previewIndex] == null) return;

        StartCoroutine(new InstantSnapshotCommand(captureCamera, iconResolution, prefabs[previewIndex]).Execute());
    }

    // Chụp hàng loạt
    public void CaptureAll()
    {
        if (isProcessingQueue || prefabs.Count == 0) return;

        // Reset thời gian về bình thường để tool tự chạy
        ResetWorldTime();

        Queue<ICaptureCommand> queue = new Queue<ICaptureCommand>();
        foreach (var p in prefabs)
        {
            if (p != null)
                queue.Enqueue(new VFXCaptureCommand(
                    p, captureCamera, captureDelay, iconResolution,
                    this.transform.position, targetOffset, cameraDistance, cameraPitch, cameraYaw, cameraFOV
                ));
        }

        StartCoroutine(ProcessQueue(queue));
    }

    private IEnumerator ProcessQueue(Queue<ICaptureCommand> queue)
    {
        isProcessingQueue = true;
        Debug.Log($"<color=yellow>[VFX Capture]</color> Bắt đầu chụp {queue.Count} kỹ năng...");

        while (queue.Count > 0)
        {
            var cmd = queue.Dequeue();
            yield return StartCoroutine(cmd.Execute());
        }

        isProcessingQueue = false;
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        Debug.Log("<color=green>[VFX Capture]</color> Hoàn tất toàn bộ quy trình Capture All!");
    }
}

// ==========================================
// 4. EDITOR UI
// ==========================================
#if UNITY_EDITOR
[CustomEditor(typeof(VFXIconCaptureTool))]
public class VFXIconCaptureToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VFXIconCaptureTool tool = (VFXIconCaptureTool)target;

        GUILayout.Space(15);

        // --- TIME CONTROL UI ---
        EditorGUILayout.LabelField("Time & Slow Motion Control", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply Time Scale", GUILayout.Height(25)))
        {
            if (Application.isPlaying) tool.SetWorldTimeScale();
            else Debug.LogWarning("Chỉ hoạt động trong Play Mode!");
        }
        if (GUILayout.Button("Reset Time (1.0)", GUILayout.Height(25)))
        {
            if (Application.isPlaying) tool.ResetWorldTime();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // --- PREVIEW & INSTANT CAPTURE UI ---
        EditorGUILayout.LabelField("Preview & Manual Capture", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("1. Preview (Chạy Skill)", GUILayout.Height(35)))
        {
            if (Application.isPlaying) tool.PreviewSkill();
            else Debug.LogWarning("Vui lòng ấn nút PLAY game!");
        }

        GUI.backgroundColor = new Color(1f, 0.6f, 0f); // Màu cam nổi bật
        if (GUILayout.Button("2. INSTANT CAPTURE (Chụp Ngay)", GUILayout.Height(35)))
        {
            if (Application.isPlaying) tool.InstantCapture();
            else Debug.LogWarning("Vui lòng ấn nút PLAY game!");
        }
        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(15);

        // --- CAPTURE ALL UI ---
        EditorGUILayout.LabelField("Batch Process", EditorStyles.boldLabel);
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("CAPTURE ALL (Tự động chụp theo Delay)", GUILayout.Height(40)))
        {
            if (Application.isPlaying) tool.CaptureAll();
            else EditorUtility.DisplayDialog("Yêu cầu", "Vui lòng chạy Play Mode.", "Đã hiểu");
        }
        GUI.backgroundColor = Color.white;
    }
}
#endif