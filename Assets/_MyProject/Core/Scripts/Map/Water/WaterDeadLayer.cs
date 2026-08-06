using UnityEngine;

public class WaterDeadLayer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out HealthSystem health)) return;

        health.InstantDead();
        Debug.Log("Cut ngay");
    }
}
