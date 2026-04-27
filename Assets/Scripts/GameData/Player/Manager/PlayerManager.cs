using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerInfo Player;

    private void Awake()
    {
        Player.PlayerTransform = this.transform;
    }
}
