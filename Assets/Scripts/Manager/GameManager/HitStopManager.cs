using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private Dictionary<Animator, Coroutine> _activeHitStops = new Dictionary<Animator, Coroutine>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerHitStop(float duration, params Animator[] animators)
    {
        foreach (Animator anim in animators)
        {
            if (anim == null) continue;

            if (_activeHitStops.ContainsKey(anim))
            {
                StopCoroutine(_activeHitStops[anim]);
                _activeHitStops.Remove(anim);
            }

            Coroutine hitStopCoroutine = StartCoroutine(DoHitStop(anim, duration));
            _activeHitStops.Add(anim, hitStopCoroutine);
        }
    }

    private IEnumerator DoHitStop(Animator anim, float duration)
    {
        float originalSpeed = anim.speed > 0 ? anim.speed : 1f;
        anim.speed = 0f;

        yield return new WaitForSecondsRealtime(duration);

        anim.speed = originalSpeed;
        _activeHitStops.Remove(anim);
    }
}
