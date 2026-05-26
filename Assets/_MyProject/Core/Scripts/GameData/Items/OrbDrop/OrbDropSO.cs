using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RarityOrbMapping
{
    public EquipmentRarity Rarity;
    public GameObject OrbPrefab;
}

[CreateAssetMenu(fileName = "OrbDropSO", menuName = "GameData/Items/Drop/OrbDropSO")]
public class OrbDropSO : ScriptableObject
{
    public List<RarityOrbMapping> OrbDrop;
}
