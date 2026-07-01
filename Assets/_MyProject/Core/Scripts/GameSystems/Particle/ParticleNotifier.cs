using System;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleNotifier : MonoBehaviour
{
    public event Action OnStopped;

    private void OnParticleSystemStopped()
    {
        OnStopped?.Invoke();
    }
}
