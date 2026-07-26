using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] protected float _lerpSpeed = 5f;
    [SerializeField] protected TMP_Text _currentValueText;
    [SerializeField] protected TMP_Text _currentMaxValueText;
    protected Image _fillImage;
    protected float _targetFillAmount = 1f;
    protected void Awake()
    {
        _fillImage = GetComponent<Image>();
    }
    protected void Start()
    {
        if (_fillImage == null) return;
        _fillImage.fillAmount = 1f;
        _targetFillAmount = 1f;
    }
    protected void Update()
    {
        if (_fillImage == null) return;
        if (!Mathf.Approximately(_fillImage.fillAmount, _targetFillAmount))
        {
            _fillImage.fillAmount = Mathf.Lerp(
                _fillImage.fillAmount,
                _targetFillAmount,
                _lerpSpeed * Time.deltaTime
            );
        }
    }
    public virtual void SetTarget(float current, float max)
    {
        _targetFillAmount = current / max;
        if (_currentValueText != null)
        {
            _currentValueText.SetText(current + "");
        }

        if (_currentMaxValueText != null) _currentMaxValueText.SetText(max.ToString());
    }
}
