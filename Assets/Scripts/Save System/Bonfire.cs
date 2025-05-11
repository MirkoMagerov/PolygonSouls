using UnityEngine;

public class Bonfire : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string bonfireID;
    [SerializeField] private GameObject particlesAndLight;
    [SerializeField] private GameObject interactionCanvas;
    [SerializeField] private Transform bonfireSpawnPoint;

    [Header("Estado")]
    [SerializeField] private bool isLit = false;
    private bool playerInRange = false;

    void Start()
    {
        StarterAssetsInputs.OnInteractPerformed += HandleInteraction;

        CheckInitialState();
    }

    private void CheckInitialState()
    {
        if (BonfireManager.Instance != null && BonfireManager.Instance.IsBonfireLit(bonfireID))
        {
            isLit = true;
            particlesAndLight.SetActive(true);
        }
        else
        {
            isLit = false;
            particlesAndLight.SetActive(false);
        }

        interactionCanvas.SetActive(false);
    }

    void OnEnable()
    {
        Invoke(nameof(CheckInitialState), 0.1f);
    }

    void OnDestroy()
    {
        StarterAssetsInputs.OnInteractPerformed -= HandleInteraction;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!isLit)
            {
                interactionCanvas.SetActive(true);
            }
            else
            {
                // Si ya está encendida, mostrar UI para descansar
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionCanvas.SetActive(false);
        }
    }

    private void HandleInteraction()
    {
        if (!playerInRange) return;

        if (!isLit)
        {
            LightUpBonfire();
        }
        else
        {
            // Posible mecánica de descanso
            // RestAtBonfire();
        }
    }

    private void LightUpBonfire()
    {
        if (isLit) return;

        particlesAndLight.SetActive(true);
        interactionCanvas.SetActive(false);
        isLit = true;

        if (BonfireManager.Instance != null)
        {
            BonfireManager.Instance.LightBonfire(bonfireID, bonfireSpawnPoint);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetLastBonfirePosition(bonfireSpawnPoint);
            GameManager.Instance.SetLastBonfireID(bonfireID);
        }
    }

    private void RestAtBonfire()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.TryGetComponent<PlayerHealth>(out var healthComponent))
        {
            healthComponent.ResetHealth();
        }

        if (BonfireManager.Instance != null)
        {
            BonfireManager.Instance.LightBonfire(bonfireID, transform);
        }
    }

    public void ForceLight()
    {
        if (isLit) return;

        isLit = true;
        particlesAndLight.SetActive(true);
    }

    public string GetBonfireID()
    {
        return bonfireID;
    }
}