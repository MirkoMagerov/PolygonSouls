using Cinemachine;
using StarterAssets;
using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    [SerializeField] private Transform lockOnTarget;
    [SerializeField] private GameObject freeCamera;
    [SerializeField] private GameObject lockOnCamera;

    private StarterAssetsInputs input;

    private void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        if (input.lockOn)
        {
            LockOn(lockOnTarget);
        }
    }

    public void LockOn(Transform target)
    {
        freeCamera.SetActive(false);
        lockOnCamera.SetActive(true);
        lockOnTarget = target;
        lockOnCamera.TryGetComponent(out CinemachineFreeLook cameraController);
        cameraController.LookAt = lockOnTarget; 
    }

    public void Unlock()
    {
        freeCamera.SetActive(true);
        lockOnCamera.SetActive(false);
        lockOnTarget = null;
    }
}
