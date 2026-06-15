using UnityEngine;

public class SkillCinematic : MonoBehaviour
{
    public CinematicParams Config;

    private void OnEnable()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.PlaySkillCinematic(Config);
        }
    }
}
