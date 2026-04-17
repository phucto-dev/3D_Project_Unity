using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineCamera _lockOnCamera;
    [SerializeField] private float _lockOnRadius = 15f;

    [Header("Settings")]
    [SerializeField] private LayerMask _enemyLayer;
    public bool IsLockedOn { get; private set; }

    private string _enemyTag = TagConstant.Enemy_Tag;
    private Transform _currentTarget;

    private void Update()
    {
        
    }

    private void FindLockOnTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, _lockOnRadius, _enemyLayer);

        if (targets.Length == 0) return;

        float nearestTargetDistance = Mathf.Infinity;
        Transform nearestTarget = null;

        foreach (Collider target in targets)
        {
            if (target.CompareTag(_enemyTag))
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _lockOnRadius);
    }

}
