using UnityEngine;

public class MeleeTracer : MonoBehaviour
{
    [Header("--- TRACER SETTINGS")]
    [SerializeField] private Transform[] _damagePoints;
    [SerializeField] private float _hitboxRadius;
    [SerializeField] private LayerMask _enemyLayer;

    private bool _isAttacking;
    private Vector3[] _previousPos;

    private void Start()
    {
        _previousPos = new Vector3[_damagePoints.Length];
    }

    public void StartSwing()
    {
        _isAttacking = true;
        int i = 0;
        foreach (Transform damagePoint in _damagePoints)
        {
            _previousPos[i] = damagePoint.position;
            i++;
        }
    }

    public void StopSwing()
    {
        _isAttacking = false;
    }

    private void Update()
    {
        if (!_isAttacking) return;

        for (int i = 0; i < _damagePoints.Length; i++)
        {
            Vector3 currentPos = _damagePoints[i].position;
            Vector3 prevPos = _previousPos[i];

            float distance = Vector3.Distance(currentPos, prevPos);
            Vector3 dir = (currentPos - prevPos).normalized;

            if (Physics.SphereCast(prevPos, _hitboxRadius, dir, out RaycastHit hit, distance, _enemyLayer))
            {
                HandleHit(hit.collider);
            }

            _previousPos[i] = currentPos;
        }
    }

    private void HandleHit(Collider enemyCollider)
    {
        Debug.Log("Hit");
    }
}
