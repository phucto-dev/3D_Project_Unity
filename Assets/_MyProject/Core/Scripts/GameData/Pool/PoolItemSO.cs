using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolItem", menuName = "GameData/System/Pool Item")]
public class PoolItemSO : ScriptableObject
{
    public string poolID;
    public GameObject prefab;
    public bool IsUsingNav = false;
    public int defaultCapacity = 10;
    public int maxSize = 50;
}
