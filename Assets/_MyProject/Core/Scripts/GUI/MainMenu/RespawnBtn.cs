using UnityEngine;

public class RespawnBtn : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Instance.HandleRespawn();
    }
}
