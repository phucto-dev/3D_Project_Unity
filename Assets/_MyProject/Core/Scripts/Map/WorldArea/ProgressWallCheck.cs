using UnityEngine;

public class ProgressWallCheck : MonoBehaviour
{
    public ObjectiveEventNotice EventCall;

    [SerializeField] private string ObjectiveID;

    private Collider wall;
    private void Awake()
    {
        wall = GetComponentInChildren<Collider>();
    }
    private void OnEnable()
    {
        EventCall.OnObjectiveEventComplete += TurnOffWall;
    }
    private void OnDisable()
    {
        EventCall.OnObjectiveEventComplete += TurnOffWall;
    }

    private void TurnOffWall(string id)
    {
        if (wall == null) return;
        if (id == ObjectiveID) wall.enabled = false;
    }
}
