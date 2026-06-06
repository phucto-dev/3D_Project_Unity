using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class PoolEntityConfig
{
    public string poolID;
    public GameObject prefab;
    public int defaultCapacity = 10;
    public int maxSize = 50;
}
[CreateAssetMenu(fileName = "NewPoolConfig", menuName = "GameData/System/Pool Config")]
public class PoolConfigSO : ScriptableObject
{
    public List<PoolEntityConfig> poolItems;
}
