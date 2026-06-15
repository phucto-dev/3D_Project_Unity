using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private CinematicSkillEffect _effectController;
    private CinemachineImpulseSource _impulseSource;
    private CinemachineBrain _brain;
    private CinemachineBrain.UpdateMethods _originalUpdateMethod;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _effectController = GetComponent<CinematicSkillEffect>();
        _brain = Camera.main.GetComponent<CinemachineBrain>();
    }
    public void PlaySkillCinematic(CinematicParams config)
    {
        if (_brain == null) return;

        _originalUpdateMethod = _brain.UpdateMethod;

        _brain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;

        var activeCam = _brain.ActiveVirtualCamera as CinemachineCamera;

        if (activeCam != null)
        {
            _impulseSource = activeCam.gameObject.GetComponent<CinemachineImpulseSource>();
            _effectController.ExecuteCinematic(config, activeCam, _brain, _impulseSource);
        }
    }
    public void ResetBrainUpdateMethod()
    {
        if (_brain != null)
        {
            _brain.UpdateMethod = _originalUpdateMethod;
        }
    }
}
