using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string mainMenuSceneName = "Main Menu";
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Referencias UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject settingsCanvas;

    [Header("Opciones")]
    [SerializeField] private bool saveOnPause = false;

    private void Start()
    {
        pauseCanvas.SetActive(false);

        StarterAssetsInputs.OnPausePressed += TogglePause;
    }

    private void OnDestroy()
    {
        StarterAssetsInputs.OnPausePressed -= TogglePause;
    }

    public void TogglePause()
    {
        bool isPausing = !pauseCanvas.activeInHierarchy;

        pauseCanvas.SetActive(isPausing);

        if (isPausing)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    public void PauseGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.paused = true;
        }

        gameObject.SetActive(true);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (saveOnPause)
        {
            SaveGameState();
        }
    }

    public void ResumeGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.paused = false;
        }

        gameObject.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenSettings()
    {
        settingsCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        if (SceneManager.GetActiveScene().name == gameSceneName)
        {
            SaveGameState();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.paused = false;
        }

        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ExitGame()
    {
        if (SceneManager.GetActiveScene().name == gameSceneName)
        {
            SaveGameState();
        }

        Application.Quit();
    }

    private void SaveGameState()
    {
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        int playerHealth = 100;
        if (player.TryGetComponent<PlayerHealth>(out var healthComponent))
        {
            playerHealth = healthComponent.GetCurrentHealth();
        }

        string lastBonfireID = null;
        if (BonfireManager.Instance != null)
        {
            lastBonfireID = BonfireManager.Instance.GetLastActiveBonfireID();
        }

        SaveSystem.Instance.SaveGameWithBonfire(
            enemyManager.GetDeadEnemyIDs(),
            enemyManager.GetEnemyDeathData(),
            player.transform,
            playerHealth,
            lastBonfireID
        );
    }
}