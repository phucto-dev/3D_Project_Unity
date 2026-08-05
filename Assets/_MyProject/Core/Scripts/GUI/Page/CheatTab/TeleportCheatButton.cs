using System;
using UnityEngine;

public class TeleportCheatButton : MonoBehaviour
{
    public SpawnPointID SpawnID;

    public event Action<SpawnPointID> OnTeleportButtonClick;
    private void OnEnable()
    {
        OnTeleportButtonClick += GameManager.Instance.HandleTeleportCheatButton;
    }
    private void OnDisable()
    {
        OnTeleportButtonClick -= GameManager.Instance.HandleTeleportCheatButton;
    }
    public void HandleOnTeleportClick()
    {
        OnTeleportButtonClick?.Invoke(SpawnID);
    }
}
