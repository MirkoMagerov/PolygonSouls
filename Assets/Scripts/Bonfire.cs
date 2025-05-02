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
        }
    }

    private void SaveGameState()
    {
        Debug.Log("Game state saved.");
    }
}
