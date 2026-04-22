using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TelekinesisState
{
    Pull,
    Push
}
public class AddForce : MonoBehaviour
{
    public Rigidbody Rb;
    public Transform MainCam;
    public Transform Player;
    public float DistanceOffset;
    public float TimeDelay;
    public float ShootingForce;
    public float PullingForce;

    private float _timer;
    private float _timer2;
    private TelekinesisState _currentState;
    private float _distanceOffset;

    private void Start()
    {
        _currentState = TelekinesisState.Pull;
        _distanceOffset = DistanceOffset * DistanceOffset;
    }

    private void Update()
    {
        ControlTelekinesis();

        switch (_currentState)
        {
            case TelekinesisState.Pull:
                PullState();
                break;
            case TelekinesisState.Push:
                break;
            default:
                break;

        }
    }
    private void PullState()
    {
        if (Vector3.SqrMagnitude(transform.position - Player.position) <= _distanceOffset)
        {
            Rb.linearVelocity = Vector3.zero;
        }
    }
    private void ControlTelekinesis()
    {
        if (Keyboard.current.zKey.isPressed)
        {
            if (Time.time - _timer < TimeDelay) return;
            _currentState = TelekinesisState.Push;
            _timer = Time.time;
            Rb.AddForce((Vector3.up + MainCam.forward) * ShootingForce, ForceMode.Impulse);
        }
        if (Keyboard.current.xKey.isPressed)
        {
            if (Time.time - _timer2 < TimeDelay) return;
            _currentState = TelekinesisState.Pull;
            _timer2 = Time.time;
            Vector3 dir = (Player.position - transform.position).normalized;
            Rb.AddForce(dir * PullingForce, ForceMode.Impulse);
        }
    }
}
