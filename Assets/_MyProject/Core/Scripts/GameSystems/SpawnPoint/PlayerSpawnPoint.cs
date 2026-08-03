using UnityEngine;

public enum SpawnPointID
{
    Default_NewGame,
    Bridge,
    Boss_Room_Entrance
}
public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("--- SPAWN ID ---")]
    public SpawnPointID PointID;

    [Header("--- OBJECTIVE EVENT ID ---")]
    public string ObjectiveID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(TagConstant.TagPlayer))
        {
            Debug.Log("Vo r neeeee");
            GameManager.Instance.HandleResetStatsPlayer();
            GameManager.Instance.SetCheckPoint(PointID);
            GameEventManager.TriggerObjectiveAction(ObjectiveEventType.ReachLocation, true, ObjectiveID, 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
