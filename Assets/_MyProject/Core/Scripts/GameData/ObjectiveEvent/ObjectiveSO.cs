using UnityEngine;

public enum ObjectiveEventType
{
    KillEnemy,
    CollectItem,
    TalkToNPC,
    ReachLocation
}

[CreateAssetMenu(fileName = "NewObjective", menuName = "GameData/ObjectiveEvent")]
public class ObjectiveSO : ScriptableObject
{
    public string ObjectiveID;
    public string Description;
    public string Title;

    public ObjectiveEventType EventType;
    public bool TargetCheck;
    public string TargetID;
    public int RequiredAmount;
}
