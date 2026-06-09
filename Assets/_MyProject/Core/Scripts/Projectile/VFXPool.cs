using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VFXPool : MonoBehaviour
{
    private string _myPoolID;

    public void Setup(string poolID)
    {
        _myPoolID = poolID;
    }

    private void OnParticleSystemStopped()
    {
        if (!string.IsNullOrEmpty(_myPoolID))
        {
            PoolManager.Instance.Release(_myPoolID, this.gameObject);
        }
    }
}
