using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject _mainHUD;
    [SerializeField] private GameObject _mainBossAreaHUD;

    private HPBarUI hpBar;

    private void Start()
    {
        if (_mainBossAreaHUD == null) return;
        _mainBossAreaHUD.SetActive(false);
        hpBar = _mainBossAreaHUD.GetComponentInChildren<HPBarUI>();
    }
    public void OpenBossHUD()
    {
        if (_mainBossAreaHUD == null) return;
        _mainBossAreaHUD.SetActive(true);
    }
    public void CloseBossHUD()
    {
        if (_mainBossAreaHUD == null) return;
        _mainBossAreaHUD.SetActive(false);
    }
    public HPBarUI GetHPBar()
    {
        return hpBar;
    }
}
