using UnityEngine;

public class BossTurnState : IBossState
{
    private string TurnLeftAnimName = "Turn90L";
    private string TurnRightAnimName = "Turn90R";

    private float _turnTimer;
    private float _currentTurnAnimLength;
    private float _dynamicRotSpeed = 0f;
    private string _targetAnim;
    private bool _hasCalculatedSpeed;

    public void Enter(BossStateManager boss)
    {
        boss.SetLocomotion(new GroundLocomotion());
        _turnTimer = 0f;
        _currentTurnAnimLength = boss.Anim.GetCurrentAnimatorStateInfo(0).length;
        _hasCalculatedSpeed = false;

        // Chỉ dùng để xét xem nên bật Anim xoay trái hay xoay phải
        int side = boss.transform.GetDirectionToTarget(boss.Player.position);
        _targetAnim = (side == -1) ? TurnLeftAnimName : TurnRightAnimName;
        boss.Anim.CrossFade(_targetAnim, 0.1f);
    }

    public void UpdateState(BossStateManager boss)
    {
        if (boss.Player == null) return;
        _turnTimer += Time.deltaTime;

        if (!_hasCalculatedSpeed)
        {
            AnimatorStateInfo currentInfo = boss.Anim.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = boss.Anim.GetNextAnimatorStateInfo(0);

            if (nextInfo.IsName(_targetAnim))
            {
                _currentTurnAnimLength = nextInfo.length;
                _hasCalculatedSpeed = true;
            }
            else if (currentInfo.IsName(_targetAnim))
            {
                _currentTurnAnimLength = currentInfo.length;
                _hasCalculatedSpeed = true;
            }
            if (!_hasCalculatedSpeed) return;
        }

        // ========================================================
        // LIVE TRACKING KẾT HỢP DYNAMIC SPEED
        // ========================================================
        Vector3 offset = boss.Player.position - boss.transform.position;
        offset.y = 0;

        if (offset != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(offset.normalized);

            // 1. Tính toán lại thời gian còn lại của Animation
            float timeRemaining = _currentTurnAnimLength - _turnTimer;
            if (timeRemaining <= 0f) timeRemaining = 0.1f;

            // 2. Cập nhật góc lệch MỚI NHẤT giữa Boss và Player
            float remainingAngle = Quaternion.Angle(boss.transform.rotation, targetRot);

            // 3. Vận tốc xoay sẽ tự "rướn" lên nếu Player chạy nhanh, tạo cảm giác Dứt khoát
            // Nhân hệ số 1.15f để vận tốc luôn lớn hơn mức tối thiểu, ép Boss xoay bắt kịp mục tiêu
            _dynamicRotSpeed = (remainingAngle / timeRemaining) * 1.15f;

            // 4. Áp dụng xoay
            boss.transform.rotation = Quaternion.RotateTowards(
                boss.transform.rotation,
                targetRot,
                _dynamicRotSpeed * Time.deltaTime
            );
        }

        // CHỐT CHẶN THOÁT STATE AN TOÀN
        if (_turnTimer >= _currentTurnAnimLength)
        {
            // Tự động chuyển thẳng sang DecisionState
            boss.ChangeState(new BossDecisionState());
        }
    }

    public void OnAnimationEnded(BossStateManager boss) { }
    public void OnActionTriggered(BossStateManager boss) { }
    public void Exit(BossStateManager boss) { }
}