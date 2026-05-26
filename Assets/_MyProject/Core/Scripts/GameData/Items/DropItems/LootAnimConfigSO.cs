using UnityEngine;

[CreateAssetMenu(fileName = "LootAnimConfigSO", menuName = "GameData/Items/Drop/LootAnimConfig")]
public class LootAnimConfigSO : ScriptableObject
{
    [Header("--- DROP DYNAMICS ---")]
    [Tooltip("Falling speed")]
    public float DropSpeed = 0.6f;
    [Tooltip("Radius ground corpse")]
    public float DropRadius = 3.0f;

    [Header("--- MAGNET SETTINGS ---")]
    public float Speed = 2f;
    public float Acceleration = 20f;

    [Header("--- IDLE ANIMATION ---")]
    [Tooltip("Spin speed")]
    public float RotationSpeed = 2f;
    [Tooltip("Floating height")]
    public float BobbingHeight = 0.07f;
    [Tooltip("Floating speed (Sin)")]
    public float BobbingSpeed = 2f;
}
