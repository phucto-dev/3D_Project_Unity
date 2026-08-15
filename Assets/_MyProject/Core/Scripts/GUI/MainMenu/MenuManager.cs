using UnityEngine;

public enum MenuBtnList
{
    NewGame,
    Continue,
    Load,
    Options,
    Credits,
    SaveAndQuit,
    Quit
}
public class MenuManager : MonoBehaviour
{
    [Header("--- MENU UI (not btn) ---")]
    [SerializeField] private GameObject _newGame;
    [SerializeField] private GameObject _continue;
    [SerializeField] private GameObject _load;
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _credits;
    [SerializeField] private GameObject _saveAndQuit;
    [SerializeField] private GameObject _quit;

    [Header("--- MENU BTN SCRIPT ---")]
    [SerializeField] private MenuBtn _newGameBtnScript;
    [SerializeField] private MenuBtn _continueBtnScript;
    [SerializeField] private MenuBtn _loadBtnScript;
    [SerializeField] private MenuBtn _optionsBtnScript;
    [SerializeField] private MenuBtn _creditBtnScript;
    [SerializeField] private MenuBtn _saveAndQuitBtnScript;
    [SerializeField] private MenuBtn _quitBtnScript;

    [Header("--- MENU CONTENT ---")]
    [SerializeField] private GameObject _optionsContent;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        if (_newGameBtnScript != null) _newGameBtnScript.BtnClicked += ChangeUIMenu;
        if (_continueBtnScript != null) _continueBtnScript.BtnClicked += ChangeUIMenu;
        if (_loadBtnScript != null) _loadBtnScript.BtnClicked += ChangeUIMenu;
        if (_optionsBtnScript != null) _optionsBtnScript.BtnClicked += ChangeUIMenu;
        if (_creditBtnScript != null) _creditBtnScript.BtnClicked += ChangeUIMenu;
        if (_saveAndQuitBtnScript != null) _saveAndQuitBtnScript.BtnClicked += ChangeUIMenu;
        if (_quitBtnScript != null) _quitBtnScript.BtnClicked += ChangeUIMenu;
    }
    private void OnDisable()
    {
        if (_newGameBtnScript != null) _newGameBtnScript.BtnClicked -= ChangeUIMenu;
        if (_continueBtnScript != null) _continueBtnScript.BtnClicked -= ChangeUIMenu;
        if (_loadBtnScript != null) _loadBtnScript.BtnClicked -= ChangeUIMenu;
        if (_optionsBtnScript != null) _optionsBtnScript.BtnClicked -= ChangeUIMenu;
        if (_creditBtnScript != null) _creditBtnScript.BtnClicked -= ChangeUIMenu;
        if (_saveAndQuitBtnScript != null) _saveAndQuitBtnScript.BtnClicked -= ChangeUIMenu;
        if (_quitBtnScript != null) _quitBtnScript.BtnClicked -= ChangeUIMenu;
    }
    private void Start()
    {
        OpenMainMenu(false);
    }
    public void OpenMainMenu(bool isIngame)
    {
        _newGame.SetActive(false);
        _continue.SetActive(false);
        _load.SetActive(false);
        _options.SetActive(false);
        _credits.SetActive(false);
        _saveAndQuit.SetActive(false);
        _quit.SetActive(false);

        if (isIngame)
        {
            _continue.SetActive(true);
            _options.SetActive(true);
            _saveAndQuit.SetActive(true);
        }
        else
        {
            _newGame.SetActive(true);
            _load.SetActive(true);
            _options.SetActive(true);
            _credits.SetActive(true);
            _quit.SetActive(true);
        }
    }
    public void ChangeUIMenu(MenuBtnList btn)
    {
        switch (btn)
        {
            case MenuBtnList.NewGame:
                //GameManager.Instance.ChangeGameState(GameState.Playing);
                GameManager.Instance.StartNewGame();
                break;
            case MenuBtnList.Continue:
                GameManager.Instance.ContinueGame();
                gameObject.SetActive(false);
                break;
            case MenuBtnList.Load:
                break;
            case MenuBtnList.Options:
                OpenContent(_optionsContent);
                break;
            case MenuBtnList.Credits:
                break;
            case MenuBtnList.SaveAndQuit:
                GameManager.Instance.QuitToMenu();
                break;
            case MenuBtnList.Quit:
                Application.Quit();
                break;
        }
    }

    private void OpenContent(GameObject objectContent)
    {
        if (objectContent == null) return;
        objectContent.SetActive(!objectContent.activeSelf);
    }
}
