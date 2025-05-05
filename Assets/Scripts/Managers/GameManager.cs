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
        // Cargar el juego si hay datos guardados
        LoadGameIfSaveExists();
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Intentar encontrar el Player y EnemyManager después de cargar la escena
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

        // Cargar el juego si hay datos guardados
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
            enemyManager.RestoreEnemyState(loadedData.deadEnemyIDs);

            // Si tenemos playerController, restaurar su posición
            if (playerController != null && lastBonfirePosition == null)
            {
                playerController.transform.position = loadedData.playerPosition;
                playerController.transform.rotation = loadedData.playerRotation;
            }

            Debug.Log($"Estado de juego cargado. Enemigos muertos: {loadedData.deadEnemyIDs.Count}");
        }
    }

    private void HandlePlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
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

        // No respawneamos enemigos muertos
        // enemyManager.RespawnAllEnemies();

        // En su lugar, cargamos el estado guardado si existe
        LoadGameIfSaveExists();

        yield return new WaitForSeconds(1f);
        EnableInputSystem();
    }

    // Método para testing
    public void SaveAndRestart()
    {
        if (enemyManager != null && SaveSystem.Instance != null)
        {
            // Guardar estado actual
            Transform playerTransform = playerController != null ? playerController.transform : null;
            SaveSystem.Instance.SaveGame(enemyManager.GetDeadEnemyIDs(), playerTransform);

            // Reiniciar escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}