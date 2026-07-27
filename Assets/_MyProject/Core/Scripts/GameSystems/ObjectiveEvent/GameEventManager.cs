using System;
using UnityEngine;

public static class GameEventManager
{
    public static event Action<ObjectiveEventType, string, int> OnObjectiveAction;

    public static void TriggerObjectiveAction(ObjectiveEventType type, string targetId, int amount = 1)
    {
        OnObjectiveAction?.Invoke(type, targetId, amount);
    }
}
