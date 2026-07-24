using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public enum GameState
{
    Booting,
    Die,
    Loading,
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
    [SerializeField] private GameObject _freeLookCam;
    [SerializeField] private GameObject _lockOnCam;
    [SerializeField] private GameObject _mainCamera;
    [SerializeField] private string _firstMapName;

    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }

    public event Action<ActionInputMapType> ChangeActionInputMap;
    public event Action RespawnPlayer;

    private @InputSystem_Actions _globalInput;
    private SpawnPointID _targetSpawnID = SpawnPointID.Default_NewGame;
    private GameObject _playerInstance;
    private Transform _currentCheckPoint;
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
            Debug.Log("Tat");
            ChangeGameState(GameState.InGameMenu);
            ChangeActionInputMap?.Invoke(ActionInputMapType.UI);
        }
        else if (CurrentState == GameState.InGameMenu)
        {
            Debug.Log("Bat");
            ChangeGameState(GameState.Playing);
            ChangeActionInputMap?.Invoke(ActionInputMapType.Player);
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

        if (newState == GameState.InGameMenu || newState == GameState.MainMenu || newState == GameState.Die)
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
    public void StartNewGame()
    {
        StartCoroutine(LoadSceneAndInitRoutine(_firstMapName));
    }
    private IEnumerator LoadSceneAndInitRoutine(string mapName)
    {
        ChangeGameState(GameState.Loading);
        float startTime = Time.realtimeSinceStartup;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            float progressValue = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log("Process: " + progressValue * 100 + "%");
            yield return null;
        }

        float totalLoadTime = Time.realtimeSinceStartup - startTime;
        Debug.Log($"<color=green>Đã tải xong Map {mapName} trong: {totalLoadTime} giây!</color>");

        yield return StartCoroutine(InitGameplayRoutine());
        ChangeGameState(GameState.Playing);
    }
    private IEnumerator InitGameplayRoutine()
    {
        yield return null;
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
        _currentCheckPoint = correctSpawn.transform;
        Scene targetMapScene = correctSpawn.gameObject.scene;
        _playerInstance = Instantiate(_playerPrefab, correctSpawn.transform.position, correctSpawn.transform.rotation);

        if (_playerInstance == null) yield break;

        SceneManager.MoveGameObjectToScene(_playerInstance, targetMapScene);

        PlayerCamTarget camTarget = _playerInstance.GetComponentInChildren<PlayerCamTarget>();
        PlayerMovement playerMovement = _playerInstance.GetComponent<PlayerMovement>();
        PlayerCamManager playerCamManager = _playerInstance.GetComponent<PlayerCamManager>();

        RegisterPlayerEvent();

        CinemachineCamera freeLookCam = _freeLookCam.GetComponent<CinemachineCamera>();
        CinemachineCamera lockonCam = _lockOnCam.GetComponent<CinemachineCamera>();
        if (camTarget == null) yield break;
        freeLookCam.Follow = camTarget.transform;
        lockonCam.Follow = camTarget.transform;

        if (playerMovement != null) playerMovement.SetMainCamera(_mainCamera.transform);
        if (playerCamManager != null) playerCamManager.SetLockOnCamera(lockonCam);

        yield return new WaitForSeconds(0.1f);
    }
    private void RegisterPlayerEvent()
    {
        if (_playerInstance == null) return;
        HealthSystem playerHealth = _playerInstance.GetComponentInChildren<HealthSystem>();
        if (playerHealth != null) playerHealth.OnDeath += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        ChangeGameState(GameState.Die);
        ChangeActionInputMap?.Invoke(ActionInputMapType.UI);
    }
    public void HandleRespawn()
    {
        if (_playerInstance == null) return;
        CooldownTimer timer = new CooldownTimer(1f);
        ChangeGameState(GameState.Loading);
        _playerInstance.transform.position = _currentCheckPoint.position;
        _playerInstance.transform.rotation = _currentCheckPoint.rotation;
        if (timer.Tick())
        {
            ChangeGameState(GameState.Playing);
            RespawnPlayer?.Invoke();
            ChangeActionInputMap?.Invoke(ActionInputMapType.Player);
        }
    }
}
