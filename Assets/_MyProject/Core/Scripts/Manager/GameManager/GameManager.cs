using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public enum GameState
{
    Booting,
    MainMenu,
    Playing,
    InGameMenu,
    Cutscene
}
public enum ActionInputMapType
{
    UI,
    Player
}
public class GameManager : MonoBehaviour
{
    [Header("--- DATA REFERENCES ---")]
    [SerializeField] private PlayerInventorySO _playerInventorySO;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _setupCam;

    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    public event Action<ActionInputMapType> ChangeActionInputMap;

    private @InputSystem_Actions _globalInput;
    private SpawnPointID _targetSpawnID = SpawnPointID.Default_NewGame;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _globalInput = new @InputSystem_Actions();
        _globalInput.Global.ToggleInventory.performed += HandleToggleInventory;

        if (_playerInventorySO != null)
        {
            _playerInventorySO.InitializeData();
        }
        else
        {
            Debug.LogError("Cannot find playerInventorySO!");
        }
    }
    private void OnEnable()
    {
        _globalInput.Global.Enable();
    }
    private void OnDisable()
    {
        _globalInput.Global.Disable();
    }
    private void Start()
    {
        ChangeGameState(GameState.MainMenu);
    }
    private void HandleToggleInventory(InputAction.CallbackContext context)
    {
        HandleInvToggle();
    }
    private void HandleInvToggle()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeGameState(GameState.InGameMenu);
            ChangeActionInputMap(ActionInputMapType.UI);
        }
        else if (CurrentState == GameState.InGameMenu)
        {
            ChangeGameState(GameState.Playing);
            ChangeActionInputMap(ActionInputMapType.Player);
        }
        else if (CurrentState == GameState.MainMenu)
        {
            Debug.Log("Do nothing");
        }
    }
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;

        MainMenuController.Instance.ChangeUIState(newState);

        if (newState == GameState.InGameMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (newState == GameState.Playing)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // NEW GAME PROCESS
    public void StartNewGame(string firstMapName)
    {
        //StartCoroutine(LoadGameRoutine(firstMapName));
    }

    private IEnumerator InitGameplayRoutine()
    {
        PlayerSpawnPoint[] allSpawns = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        if (allSpawns.Length == 0)
        {
            Debug.LogError("No SpawnPoint could be founded!");
            yield break;
        }
        PlayerSpawnPoint correctSpawn = null;
        foreach (PlayerSpawnPoint sp in allSpawns)
        {
            if (sp.PointID == _targetSpawnID)
            {
                correctSpawn = sp;
                break;
            }
        }
        if (correctSpawn == null)
        {
            correctSpawn = allSpawns[0];
        }
        GameObject playerInstance = Instantiate(_playerPrefab, correctSpawn.transform.position, correctSpawn.transform.rotation);

        if (playerInstance == null) yield break;

        CinemachineCamera freeLook = _setupCam.GetComponentInChildren<CinemachineCamera>();
        freeLook.Follow = playerInstance.transform;
        freeLook.LookAt = playerInstance.transform;

        yield return new WaitForSeconds(1f);
    }
}
