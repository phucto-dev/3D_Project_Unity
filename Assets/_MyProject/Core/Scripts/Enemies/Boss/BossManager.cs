using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("--- GROUND SETUP ---")]
    public Transform GroundCheckPos;
    public float GroundCheckRadius;
    public LayerMask GroundLayer;

    private bool _isGrounded;

    public bool CheckGround()
    {
        _isGrounded = false;
        if (GroundCheckPos != null)
            _isGrounded = Physics.CheckSphere(GroundCheckPos.position, GroundCheckRadius, GroundLayer);
        return _isGrounded;
    }
    private void OnDrawGizmosSelected()
    {
        if (GroundCheckPos != null)
        {
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(GroundCheckPos.position, GroundCheckRadius);
        }
    }
}
