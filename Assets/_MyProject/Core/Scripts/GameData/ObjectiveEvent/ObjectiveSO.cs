using UnityEngine;

public enum ObjectiveEventType
{
    KillEnemy,
    CollectItem,
    TalkToNPC,
    ReachLocation
}

[CreateAssetMenu(fileName = "ObjectiveSO", menuName = "Scriptable Objects/ObjectiveSO")]
public class ObjectiveSO : ScriptableObject
{
    public string ObjectiveID;
    public string Description;

    public ObjectiveEventType EventType;
    public string TargetID;
    public int RequiredAmount;
}
