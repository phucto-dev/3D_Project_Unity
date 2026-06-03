using UnityEngine;

public enum SpawnPointID
{
    Default_NewGame,
    South_Gate,
    North_Gate,
    Boss_Room_Entrance
}
public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("--- SPAWN ID ---")]
    public SpawnPointID PointID;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
