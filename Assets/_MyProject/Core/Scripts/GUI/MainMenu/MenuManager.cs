using UnityEngine;

public enum MenuBtnList
{
    NewGame,
    Load,
    Options,
    Credits,
    Quit
}
public class MenuManager : MonoBehaviour
{
    [Header("--- MENU UI (not btn) ---")]
    [SerializeField] private GameObject _newGame;
    [SerializeField] private GameObject _load;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _quit;

    [Header("--- MENU BTN SCRIPT ---")]
    [SerializeField] private MenuBtn _newGameBtnScript;
    [SerializeField] private MenuBtn _loadBtnScript;
    [SerializeField] private MenuBtn _optionBtnScript;
    [SerializeField] private MenuBtn _creditBtnScript;
    [SerializeField] private MenuBtn _quitBtnScript;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        if (_newGameBtnScript != null) _newGameBtnScript.BtnClicked += ChangeUIMenu;
        if (_loadBtnScript != null) _loadBtnScript.BtnClicked += ChangeUIMenu;
        if (_optionBtnScript != null) _optionBtnScript.BtnClicked += ChangeUIMenu;
        if (_creditBtnScript != null) _creditBtnScript.BtnClicked += ChangeUIMenu;
        if (_quitBtnScript != null) _quitBtnScript.BtnClicked += ChangeUIMenu;
    }
    private void OnDisable()
    {
        if (_newGameBtnScript != null) _newGameBtnScript.BtnClicked -= ChangeUIMenu;
        if (_loadBtnScript != null) _loadBtnScript.BtnClicked -= ChangeUIMenu;
        if (_optionBtnScript != null) _optionBtnScript.BtnClicked -= ChangeUIMenu;
        if (_creditBtnScript != null) _creditBtnScript.BtnClicked -= ChangeUIMenu;
        if (_quitBtnScript != null) _quitBtnScript.BtnClicked -= ChangeUIMenu;
    }
    public void ChangeUIMenu(MenuBtnList btn)
    {
        //_newGame.SetActive(false);
        //_load.SetActive(false);
        //_options.SetActive(false);
        //_credits.SetActive(false);
        //_quit.SetActive(false);

        switch (btn)
        {
            case MenuBtnList.NewGame:
                GameManager.Instance.ChangeGameState(GameState.Playing);
                GameManager.Instance.StartNewGame();
                break;
            case MenuBtnList.Load:
                break;
            case MenuBtnList.Options:
                break;
            case MenuBtnList.Credits:
                break;
            case MenuBtnList.Quit:
                break;
        }
    }
}
