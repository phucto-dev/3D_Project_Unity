using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HurtUI : MonoBehaviour
{
    [Header("--- SETTINGS ---")]
    [Range(0f, 1f)]
    public float MaxAlpha = 0.8f;
    public float AppearSpeed = 1f;
    public float FadeOutTime = 5f;
    public float FadeOutSpeed = 5f;

    [Header("--- ONLY FOR TEST ---")]
    public PlayerManager Player;
    public HealthSystem PlayerHealth;

    private Image _hurtImage;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _hurtImage = GetComponent<Image>();

        Color startColor = _hurtImage.color;
        startColor.a = 0f;
        _hurtImage.color = startColor;
    }
    private void OnEnable()
    {
        if (Player == null) return;
        Player.OnGetHit += TriggerHurtEffect;
    }
    private void OnDisable()
    {
        if (Player == null) return;
        Player.OnGetHit -= TriggerHurtEffect;
    }

    private void TriggerHurtEffect()
    {
        DoCoroutine();
    }
    private void DoCoroutine()
    {
        if (PlayerHealth == null) return;
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }
    private IEnumerator FadeOutRoutine()
    {
        if (PlayerHealth == null || PlayerHealth.MaxHealth <= 0) yield break;
        float currentPercentHealth = PlayerHealth.CurrentHealth / PlayerHealth.MaxHealth;
        float targetAlpha = Mathf.Min(1f - currentPercentHealth, MaxAlpha);
        Color color = _hurtImage.color;

        while (Mathf.Abs(color.a - targetAlpha) > 0.01f)
        {
            color.a = Mathf.Lerp(color.a, targetAlpha, AppearSpeed * Time.deltaTime);
            _hurtImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        _hurtImage.color = color;

        yield return new WaitForSeconds(FadeOutTime);

        while (_hurtImage.color.a > 0f)
        {
            color.a -= FadeOutSpeed * Time.deltaTime;
            _hurtImage.color = color;

            yield return null;
        }

        color.a = 0f;
        _hurtImage.color = color;
    }
}
