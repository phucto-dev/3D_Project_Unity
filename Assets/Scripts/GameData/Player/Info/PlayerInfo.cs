using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInfo", menuName = "GameData/Player/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    [Header("--- TRANSFORM ---")]
    public Transform PlayerTransform;
}
