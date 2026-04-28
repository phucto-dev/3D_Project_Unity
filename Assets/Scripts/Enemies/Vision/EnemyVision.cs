#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("--- VISION SETTINGS ---")]
    [SerializeField] private EnemyBrainConfigSO _brainConfig;

    [Header("--- TARGET ---")]
    public PlayerInfo Player;
    public LayerMask TargetLayer;
    public LayerMask ObstacleLayer;

    // Transfer Human Angle to Machine Angle
    private float _cosThreshold;
    private bool _seePlayer;
    private float _distanceToTarget;

    private void Start()
    {
        // Just a random recipe AI gen which change Angle of 0 (Human) to 1 (Machine) or 90 to 0
        _cosThreshold = Mathf.Cos(_brainConfig.FieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
    }

    public bool CanSeePlayer()
    {
        if (Player == null) return false;

        _distanceToTarget = Vector3.Distance(transform.position, Player.PlayerTransform.position);
        if (!_seePlayer) _seePlayer = CheckPlayerOnSightRange();
        else _seePlayer = CheckChasingPlayer();
        return _seePlayer;
    }

    private bool CheckPlayerOnSightRange()
    {
        if (_distanceToTarget > _brainConfig.SightRange) return false;

        Vector3 directionToTarget = (Player.PlayerTransform.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, directionToTarget) < _cosThreshold) return false;

        Vector3 origin = transform.position + _brainConfig.SightOffset;
        Vector3 targetPos = Player.PlayerTransform.position + _brainConfig.SightOffset;
        Vector3 dir = (targetPos - origin).normalized;

        if (!Physics.Raycast(origin, dir, _distanceToTarget, ObstacleLayer))
        {
            return true;
        }

        return false;
    }
    private bool CheckChasingPlayer()
    {
        if (_distanceToTarget <= _brainConfig.LimitChaseRange) return true;
        return false;
    }

    private void OnDrawGizmos()
    {
        if (Player == null) return;
        // Chặn lỗi văng NullReference nếu cấu hình chưa được gán
        if (_brainConfig == null) return;

        Vector3 eyePosition = transform.position + _brainConfig.SightOffset;
        // Attack Range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(eyePosition, _brainConfig.AttackRange);
        // Limit Chase Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(eyePosition, _brainConfig.LimitChaseRange);
        // 1. VẼ VÒNG TRÒN BÁN KÍNH TẦM NHÌN (Màu trắng)
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(eyePosition, _brainConfig.SightRange);

        // 2. VẼ 2 ĐƯỜNG CHÉO TẠO THÀNH HÌNH NÓN GÓC NHÌN (Màu vàng)
        // Lấy góc quay hiện tại của quái vật trên trục Y
        float currentYRotation = transform.eulerAngles.y;

        // Tính 2 vector hướng từ tâm rẽ ra 2 bên
        Vector3 rightBoundary = DirFromAngle(currentYRotation, _brainConfig.FieldOfViewAngle / 2f);
        Vector3 leftBoundary = DirFromAngle(currentYRotation, -_brainConfig.FieldOfViewAngle / 2f);

        Gizmos.color = Color.yellow;
        // Vẽ từ mắt -> điểm cuối của đường chéo (nhân với độ dài tầm nhìn)
        Gizmos.DrawLine(eyePosition, eyePosition + rightBoundary * _brainConfig.SightRange);
        Gizmos.DrawLine(eyePosition, eyePosition + leftBoundary * _brainConfig.SightRange);

        // 3. VẼ ĐƯỜNG NỐI ĐẾN PLAYER ĐỂ DEBUG (Xanh = Thấy, Đỏ = Mù)
        if (Player.PlayerTransform != null)
        {
            Vector3 targetPos = Player.PlayerTransform.position + _brainConfig.SightOffset;

            if (CanSeePlayer())
            {
                Gizmos.color = Color.green; // Đổi màu xanh nếu lọt vào góc và không bị cản
                Gizmos.DrawLine(eyePosition, targetPos);
            }
            else
            {
                Gizmos.color = Color.red; // Màu đỏ nếu đang ngoài góc, ngoài tầm, hoặc bị nấp sau tường
                Gizmos.DrawLine(eyePosition, targetPos);
            }
        }
    }

    /// <summary>
    /// Hàm toán học hỗ trợ: Chuyển đổi Góc (Độ) thành Hướng (Vector3)
    /// </summary>
    private Vector3 DirFromAngle(float eulerY, float angleInDegrees)
    {
        // Cộng thêm góc quay hiện tại của nhân vật để hình nón luôn xoay theo mặt quái vật
        angleInDegrees += eulerY;

        // Lưu ý kiến trúc Unity: Trục Z (Forward) tương ứng với Cosine, trục X (Right) tương ứng với Sine
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
