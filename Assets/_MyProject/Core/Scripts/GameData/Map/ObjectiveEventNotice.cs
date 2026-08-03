using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveEventNotice", menuName = "GameData/Event/ObjectiveEventNotice")]
public class ObjectiveEventNotice : ScriptableObject
{
    public event Action<string> OnObjectiveEventComplete;

    public void TriggerOnObjectiveEventComplete(string objID)
    {
        OnObjectiveEventComplete?.Invoke(objID);
    }
}
