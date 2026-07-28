using System;
using UnityEngine;

public static class GameEventManager
{
    public static event Action<ObjectiveEventType, bool, string, int> OnObjectiveAction;

    public static void TriggerObjectiveAction(ObjectiveEventType type, bool targetCheck, string targetId, int amount = 1)
    {
        OnObjectiveAction?.Invoke(type, targetCheck, targetId, amount);
    }
}
