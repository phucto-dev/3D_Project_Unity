using System;
using System.Collections;
using UnityEngine;

public class VFXScaler : MonoBehaviour
{
    [Header("--- SCALE SETTINGS")]
    [SerializeField] private Vector3 _maxSize = new Vector3(5f, 5f, 5f);
    [SerializeField] private float _duration = 2.5f;
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    [SerializeField] private bool _autoPlayOnEnable = true;
    [SerializeField] private bool _playScaleInstant = true;

    public event Action StartScale;
    public event Action EndScale;

    private Coroutine _scaleCoroutine;
    private void OnEnable()
    {
        if (_autoPlayOnEnable)
        {
            PlayScaleEffect();
        }
    }
    private void OnDisable()
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
            EndScale?.Invoke();
        }
        ResetScale();
    }
    public void PlayScaleEffect()
    {
        if (_playScaleInstant)
        {
            transform.localScale = _maxSize;
        }
        else
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(ScaleRoutine());
        }
    }
    private IEnumerator ScaleRoutine()
    {
        Debug.Log("Start ne");
        StartScale?.Invoke();
        float timer = 0f;
        Vector3 startScale = Vector3.one;

        while (timer < _duration)
        {
            timer += Time.deltaTime;

            float percent = timer / _duration;
            float curveEvalute = _scaleCurve.Evaluate(percent);

            transform.localScale = Vector3.LerpUnclamped(startScale, _maxSize, curveEvalute);

            yield return null;
        }

        transform.localScale = _maxSize;
        _scaleCoroutine = null;
        Debug.Log("End ne");
        EndScale?.Invoke();
    }
    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}
