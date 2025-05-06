using UnityEngine;

public class Bonfire : MonoBehaviour
{
    [SerializeField] private GameObject particlesAndLight;
    [SerializeField] private GameObject interactionCanvas;
    private bool lightUp = false;
    private bool playerInRange = false;

    void Start()
    {
        StarterAssetsInputs.OnInteractPerformed += LightUpBonfire;
        particlesAndLight.SetActive(false);
        interactionCanvas.SetActive(false);
    }

    void OnDestroy()
    {
        StarterAssetsInputs.OnInteractPerformed -= LightUpBonfire;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !lightUp)
        {
            playerInRange = true;
            interactionCanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !lightUp)
        {
            playerInRange = false;
            interactionCanvas.SetActive(false);
        }
    }

    private void LightUpBonfire()
    {
        if (!lightUp && playerInRange)
        {
            particlesAndLight.SetActive(true);
            interactionCanvas.SetActive(false);
            lightUp = true;
            SaveGameState();

            // Registrar esta hoguera como punto de respawn
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetLastBonfirePosition(transform);
            }
        }
    }

    private void SaveGameState()
    {
        if (SaveSystem.Instance != null && GameManager.Instance != null)
        {
            // Obtener EnemyManager
            EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager != null)
            {
                // Obtener el transform del jugador y su salud actual
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Transform playerTransform = player.transform;
                    int playerHealth = 100;

                    // Intentar obtener la salud actual si existe el componente
                    if (player.TryGetComponent<PlayerHealth>(out var healthComponent))
                    {
                        playerHealth = healthComponent.GetCurrentHealth();
                    }

                    SaveSystem.Instance.SaveGame(
                     enemyManager.GetDeadEnemyIDs(),
                     enemyManager.GetEnemyDeathData(),
                     playerTransform,
                     playerHealth
                 );
                    Debug.Log("Estado del juego guardado en la hoguera.");
                }
            }
            else
            {
                Debug.LogWarning("No se encontró EnemyManager para guardar estado de enemigos.");
            }
        }
        else
        {
            Debug.LogWarning("SaveSystem o GameManager no encontrados. No se puede guardar el juego.");
        }
    }
}