using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public PlayerInput inputSystem;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableInputSystem();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EnableInputSystem();
        }
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

    private void HandlePlayerDeath()
    {
        DisableInputSystem();
    }
}
