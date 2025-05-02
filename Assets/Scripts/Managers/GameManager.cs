using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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
        }
        else
        {
            Destroy(gameObject);
        }

        inputSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
    }

    void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
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

    private void HandlePlayerDeath()
    {
        StartCoroutine(PlayerDeathCoroutine());
    }

    public void RegisterEnemyManager(EnemyManager manager)
    {
        enemyManager = manager;
    }

    private IEnumerator PlayerDeathCoroutine()
    {
        DisableInputSystem();
        playerController.PlayDeathAnimation();

        yield return new WaitForSeconds(3f);

        //FadeScreenToBlack();
        //yield return new WaitForSeconds(1f);

        playerController.gameObject.transform.SetPositionAndRotation(lastBonfirePosition.position, lastBonfirePosition.rotation);
        playerController.ResetHealth();

        //enemyManager.RespawnAllEnemies();

        //saveSystem.LoadGame(playerController, enemyManager);

        // Fade-out y activar input
        //FadeScreenFromBlack();
        yield return new WaitForSeconds(1f);

        EnableInputSystem();
    }
}
