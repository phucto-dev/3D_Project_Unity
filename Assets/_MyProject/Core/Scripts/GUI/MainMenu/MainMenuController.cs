using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MenuPageType
{
    Character,
    Combat
}

public class MainMenuController : MonoBehaviour
{
    [Header("--- UI ROOTS ---")]
    [SerializeField] private GameObject _mainMenuRoot;
    [SerializeField] private GameObject _hudRoot;
    [SerializeField] private GameObject _inGameMenuRoot;
    [SerializeField] private GameObject _loadingRoot;
    [SerializeField] private GameObject _globalRoot;
    [SerializeField] private GameObject _interactionUI;
    public static MainMenuController Instance { get; private set; }

    public event Action OnOpenInventory;

    [SerializeField] private List<MenuPage> pages;

    private MenuPage _currentPage;
    private HUDController _hudController;
    private TMP_Text _interactionText;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (_hudRoot != null)
        {
            _hudController = _hudRoot.GetComponent<HUDController>();
        }
        if (_interactionUI != null)
        {
            _interactionText = _interactionUI.GetComponentInChildren<TMP_Text>();
        }
    }
    private void Start()
    {
        foreach (MenuPage page in pages)
        {
            if (page.PageType == MenuPageType.Character)
            {
                _currentPage = page;
                OpenPage(_currentPage.PageType);
            }
        }
        HideInteractionUI();
    }

    public void ChangeUIState(GameState newState)
    {
        _mainMenuRoot.SetActive(false);
        _hudRoot.SetActive(false);
        _inGameMenuRoot.SetActive(false);
        _loadingRoot.SetActive(false);
        _globalRoot.SetActive(false);

        switch (newState)
        {
            case GameState.MainMenu:
                _mainMenuRoot.SetActive(true);
                break;
            case GameState.Playing:
                _hudRoot.SetActive(true);
                break;
            case GameState.Loading:
                _loadingRoot.SetActive(true);
                break;
            case GameState.InGameMenu:
                _hudRoot.SetActive(true);
                _inGameMenuRoot.SetActive(true);
                OnOpenInventory?.Invoke();
                break;
            case GameState.Die:
                _globalRoot.SetActive(true);
                break;
            case GameState.Cutscene:
                break;
        }
    }

    public void ShowInteractionUI()
    {
        if (_interactionUI != null)
        {
            _interactionUI.SetActive(true);
        }
    }

    public void HideInteractionUI()
    {
        if (_interactionUI != null)
        {
            _interactionUI.SetActive(false);
        }
    }
    public void SetInteractionText(string text)
    {
        if (_interactionText == null) return;
        _interactionText.SetText(text);
    }
    public void ShowBossHUD()
    {
        if (_hudController != null)
        {
            _hudController.OpenBossHUD();
        }
    }

    public void HideBossHUD()
    {
        if (_hudController != null)
        {
            _hudController.CloseBossHUD();
        }
    }

    public BarUI HPBossBar()
    {
        if (_hudController != null)
        {
            return _hudController.GetBossHPBar();
        }
        return null;
    }

    public void OpenPage(MenuPageType pageType)
    {
        foreach (MenuPage page in pages)
        {
            bool isTarget = page.PageType == pageType;

            page.gameObject.SetActive(isTarget);

            if (isTarget)
            {
                _currentPage = page;
            }
        }
    }
}
