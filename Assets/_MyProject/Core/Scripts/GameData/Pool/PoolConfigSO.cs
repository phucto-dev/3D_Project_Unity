using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolConfig", menuName = "GameData/System/Pool Config")]
public class PoolConfigSO : ScriptableObject
{
    public List<PoolItemSO> poolItems;
}
