using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] protected float _lerpSpeed = 5f;
    protected Image _healthFillImage;
    protected float _targetFillAmount = 1f;
    protected void Awake()
    {
        _healthFillImage = GetComponent<Image>();
    }
    protected void Start()
    {
        if (_healthFillImage == null) return;
        _healthFillImage.fillAmount = 1f;
        _targetFillAmount = 1f;
    }
    protected void Update()
    {
        if (_healthFillImage == null) return;
        if (!Mathf.Approximately(_healthFillImage.fillAmount, _targetFillAmount))
        {
            _healthFillImage.fillAmount = Mathf.Lerp(
                _healthFillImage.fillAmount,
                _targetFillAmount,
                _lerpSpeed * Time.deltaTime
            );
        }
    }
    public void SetTargetHealth(float currentHealth, float maxHealth)
    {
        _targetFillAmount = currentHealth / maxHealth;
    }
}
