using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectiveChain", menuName = "GameData/Objective Chain")]
public class ObjectiveChainSO : ScriptableObject
{
    public string ChainName;

    [Header("Sequence of Objectives")]
    public List<ObjectiveSO> Objectives = new List<ObjectiveSO>();
}