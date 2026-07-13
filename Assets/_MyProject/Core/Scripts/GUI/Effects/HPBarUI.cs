using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed = 5f;
    private Image _healthFillImage;
    private float _targetFillAmount = 1f;
    private void Awake()
    {
        _healthFillImage = GetComponent<Image>();
    }
    private void Start()
    {
        if (_healthFillImage == null) return;
        _healthFillImage.fillAmount = 1f;
        _targetFillAmount = 1f;
    }
    private void Update()
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
