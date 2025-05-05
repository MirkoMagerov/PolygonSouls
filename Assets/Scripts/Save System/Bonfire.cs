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
                // Guardar los IDs de enemigos muertos y la posición del jugador
                Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
                SaveSystem.Instance.SaveGame(enemyManager.GetDeadEnemyIDs(), playerTransform);
                Debug.Log("Estado del juego guardado en la hoguera.");
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