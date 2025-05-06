using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerInput inputSystem;
    public PlayerController playerController;
    [SerializeField] private Transform lastBonfirePosition;

    private EnemyManager enemyManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        inputSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Start()
    {
        LoadGameIfSaveExists();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveAndRestart();
        }
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindReferencesAfterSceneLoad());
    }

    private IEnumerator FindReferencesAfterSceneLoad()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            inputSystem = player.GetComponent<PlayerInput>();
            playerController = player.GetComponent<PlayerController>();
        }

        LoadGameIfSaveExists();
    }

    public void DisableInputSystem()
    {
        if (inputSystem != null)
            inputSystem.DeactivateInput();
    }

    public void EnableInputSystem()
    {
        if (inputSystem != null)
            inputSystem.ActivateInput();
    }

    public void SetLastBonfirePosition(Transform bonfireTransform)
    {
        lastBonfirePosition = bonfireTransform;
    }

    public void RegisterEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;
        LoadGameIfSaveExists();
    }

    private void LoadGameIfSaveExists()
    {
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile() && enemyManager != null)
        {
            GameData loadedData = SaveSystem.Instance.LoadGame();

            // Restaurar el estado de los enemigos
            enemyManager.RestoreEnemyState(loadedData.deadEnemyIDs, loadedData.enemyDeathData);

            // Si tenemos playerController y datos del jugador, restaurar su estado
            if (playerController != null && loadedData.playerData != null && lastBonfirePosition == null)
            {
                playerController.transform.SetPositionAndRotation(loadedData.playerData.position.ToVector3(), loadedData.playerData.rotation.ToQuaternion());

                // Restaurar salud si existe el componente
                if (playerController.TryGetComponent<PlayerHealth>(out var healthComponent))
                {
                    healthComponent.SetHealth(loadedData.playerData.health);
                }
            }

            Debug.Log($"Estado de juego cargado. Enemigos muertos: {loadedData.deadEnemyIDs.Count}");
        }
    }

    private IEnumerator PlayerDeathCoroutine()
    {
        DisableInputSystem();
        playerController.PlayDeathAnimation();

        yield return new WaitForSeconds(3f);

        if (lastBonfirePosition != null)
        {
            playerController.gameObject.transform.SetPositionAndRotation(lastBonfirePosition.position, lastBonfirePosition.rotation);
        }
        playerController.ResetHealth();

        LoadGameIfSaveExists();

        yield return new WaitForSeconds(1f);
        EnableInputSystem();
    }

    // Método para testing
    public void SaveAndRestart()
    {
        if (enemyManager != null && SaveSystem.Instance != null)
        {
            int playerHealth = 50;
            if (playerController != null)
            {
                if (playerController.TryGetComponent<PlayerHealth>(out var healthComponent))
                {
                    playerHealth = healthComponent.GetCurrentHealth();
                }

                SaveSystem.Instance.SaveGame(
                    enemyManager.GetDeadEnemyIDs(),
                    enemyManager.GetEnemyDeathData(),
                    playerController.transform,
                    playerHealth
                );
            }

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}