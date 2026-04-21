using UnityEngine;

public class CooldownTimer
{
    private float _duration;
    private float _timer;

    public CooldownTimer(float duration)
    {
        _duration = duration;
        _timer = duration;
    }
    public bool Tick()
    {
        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            _timer -= _duration;
            return true;
        }
        return false;
    }
}
