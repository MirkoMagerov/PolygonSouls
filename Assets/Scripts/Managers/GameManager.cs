using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias")]
    [HideInInspector] public PlayerInput inputSystem;
    [HideInInspector] public PlayerController playerController;
    [SerializeField] private EnemyManager enemyManager;

    [Header("Configuración")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameScene = "GameScene";
    [SerializeField] private float respawnDelay = 4f;
    [SerializeField] private float sceneTransitionDelay = 1f;
    [SerializeField] private float fadeInDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("Estado")]
    private Transform lastBonfirePosition;
    private string lastBonfireID;
    private Transform initialSpawnPosition;
    private bool isReloading = false;
    private GameData cachedGameData = null;
    public bool isNewGame = true;
    public bool paused = false;

    private GameObject player;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        HandleDebugKeys();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameScene)
        {
            StartCoroutine(InitializeGameScene());
        }
    }

    private IEnumerator InitializeGameScene()
    {
        yield return new WaitForSeconds(sceneTransitionDelay);

        FindGameReferences();

        if (isNewGame)
        {
            SetupNewGame();
        }
        else if (cachedGameData != null)
        {
            ApplyGameData(cachedGameData);
            cachedGameData = null;
        }
        else if (!isNewGame)
        {
            LoadGameIfSaveExists();
        }

        isReloading = false;
    }

    private void FindGameReferences()
    {
        initialSpawnPosition = GameObject.FindGameObjectWithTag("SpawnPos").transform;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputSystem = player.GetComponent<PlayerInput>();
            playerController = player.GetComponent<PlayerController>();
        }

        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }
    }

    public void CreateNewGame()
    {
        ResetGameState();
        isNewGame = true;

        SaveSystem.Instance.DeleteSaveFile();
        SaveSystem.Instance.CreateInitialSaveFile();

        SceneManager.LoadScene(gameScene);
    }

    public void LoadExistingGame()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile())
        {
            isNewGame = false;
            SceneManager.LoadScene(gameScene);
        }
        else
        {
            CreateNewGame();
        }
    }

    public void ReturnToMainMenu()
    {
        SaveAndReturnToMainMenu();
    }

    public void DisableInputSystem()
    {
        inputSystem.DeactivateInput();
    }

    public void EnableInputSystem()
    {
        inputSystem.ActivateInput();
    }

    public void SetLastBonfirePosition(Transform bonfireTransform)
    {
        lastBonfirePosition = bonfireTransform;
    }

    public void SetLastBonfireID(string bonfireID)
    {
        lastBonfireID = bonfireID;
    }

    public void RegisterEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;

        if (cachedGameData != null)
        {
            StartCoroutine(DelayedApplyGameData(cachedGameData));
        }
    }

    public void RestoreLastBonfireReference()
    {
        if (string.IsNullOrEmpty(lastBonfireID) || BonfireManager.Instance == null) return;

        Transform bonfireTransform = BonfireManager.Instance.GetBonfireTransform(lastBonfireID);
        if (bonfireTransform != null)
        {
            lastBonfirePosition = bonfireTransform;
        }
    }

    public void SaveAndReturnToMainMenu()
    {
        SaveGame();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void ResetGameState()
    {
        cachedGameData = null;
        lastBonfireID = null;
        lastBonfirePosition = null;
    }

    private void HandleDebugKeys()
    {
        if (isReloading) return;

        if (Input.GetKeyDown(KeyCode.F6))
        {
            ReturnToMainMenu();
        }
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }

    private IEnumerator DelayedApplyGameData(GameData data)
    {
        yield return new WaitForSeconds(0.5f);
        ApplyGameData(data);
    }

    private void LoadGameIfSaveExists()
    {
        if (SaveSystem.Instance == null || !SaveSystem.Instance.HasSaveFile())
        {
            isNewGame = true;
            CreateNewGame();
            return;
        }

        GameData loadedData = SaveSystem.Instance.LoadGame();

        if (enemyManager != null)
        {
            ApplyGameData(loadedData);
        }
        else
        {
            cachedGameData = loadedData;
        }
    }

    private void SetupNewGame()
    {
        if (player == null || initialSpawnPosition == null) return;

        TeleportPlayer(player, initialSpawnPosition.position, initialSpawnPosition.rotation);

        if (player.TryGetComponent<PlayerHealth>(out var healthComponent))
        {
            healthComponent.ResetHealth();
        }

        enemyManager.ResetAllEnemies();
    }

    private void ApplyGameData(GameData data)
    {
        enemyManager.RestoreEnemyState(data.enemyDeathData);

        if (player != null)
        {
            TeleportPlayer(player, data.playerData.position.ToVector3(), data.playerData.rotation.ToQuaternion());

            if (playerController.TryGetComponent<PlayerHealth>(out var healthComponent))
            {
                healthComponent.SetHealth(data.playerData.health);
            }
        }

        if (!string.IsNullOrEmpty(data.lastActiveBonfireID))
        {
            lastBonfireID = data.lastActiveBonfireID;
            StartCoroutine(LoadBonfireState(data));
        }
    }

    private IEnumerator LoadBonfireState(GameData data)
    {
        yield return new WaitForSeconds(0.3f);

        FindBonfireByID(data.lastActiveBonfireID);

        if (data.litBonfireIDs != null && data.litBonfireIDs.Count > 0 && BonfireManager.Instance != null)
        {
            BonfireManager.Instance.InitializeLitBonfires(data.litBonfireIDs);

            if (!string.IsNullOrEmpty(data.lastActiveBonfireID))
            {
                BonfireManager.Instance.SetLastActiveBonfireID(data.lastActiveBonfireID);
            }

            BonfireManager.Instance.RecoverLitBonfires();

            yield return new WaitForSeconds(0.2f);
            RestoreLastBonfireReference();
        }
    }

    private void FindBonfireByID(string bonfireID)
    {
        Bonfire[] bonfires = FindObjectsOfType<Bonfire>();
        foreach (Bonfire bonfire in bonfires)
        {
            if (bonfire.GetBonfireID() == bonfireID)
            {
                lastBonfirePosition = bonfire.transform;
                bonfire.ForceLight();
                break;
            }
        }
    }

    private IEnumerator PlayerDeathCoroutine()
    {
        DisableInputSystem();

        playerController.playerDead = true;
        playerController.PlayDeathAnimation();

        yield return new WaitForSeconds(respawnDelay);

        yield return new WaitForSeconds(fadeOutDuration);

        if (lastBonfirePosition == null && !string.IsNullOrEmpty(lastBonfireID))
        {
            FindBonfireReference();
        }

        Vector3 respawnPosition;
        Quaternion respawnRotation;

        if (lastBonfirePosition != null)
        {
            lastBonfirePosition.GetPositionAndRotation(out respawnPosition, out respawnRotation);
        }
        else
        {
            initialSpawnPosition.GetPositionAndRotation(out respawnPosition, out respawnRotation);
        }

        TeleportPlayer(player, respawnPosition, respawnRotation);
        enemyManager.ResetAllEnemies();

        playerController.playerDead = false;
        playerController.CallRespawnTrigger();
        playerController.ResetHealth();

        yield return new WaitForSeconds(fadeInDuration);
        EnableInputSystem();
    }

    private void FindBonfireReference()
    {
        if (BonfireManager.Instance != null)
        {
            lastBonfirePosition = BonfireManager.Instance.GetBonfireTransform(lastBonfireID);
            if (lastBonfirePosition != null)
            {
                return;
            }
        }

        Bonfire[] bonfires = FindObjectsOfType<Bonfire>();
        foreach (Bonfire bonfire in bonfires)
        {
            if (bonfire.GetBonfireID() == lastBonfireID)
            {
                lastBonfirePosition = bonfire.transform;
                break;
            }
        }
    }

    private void TeleportPlayer(GameObject player, Vector3 position, Quaternion rotation)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        bool controllerWasEnabled = false;

        if (controller.enabled)
        {
            controllerWasEnabled = true;
            controller.enabled = false;
        }

        player.transform.SetPositionAndRotation(position, rotation);

        if (controllerWasEnabled)
        {
            controller.enabled = true;
        }
    }

    private void SaveGame()
    {
        if (enemyManager == null || SaveSystem.Instance == null || playerController == null) return;

        List<string> deadEnemyIDs = enemyManager.GetDeadEnemyIDs();
        List<EnemyDeathData> deathData = enemyManager.GetEnemyDeathData();

        int playerHealth = 100;
        if (playerController.TryGetComponent<PlayerHealth>(out var healthComponent))
        {
            playerHealth = healthComponent.GetCurrentHealth();
        }

        SaveSystem.Instance.SaveGameWithBonfire(
            deadEnemyIDs,
            deathData,
            playerController.transform,
            playerHealth,
            lastBonfireID
        );
    }
}