using Cinemachine;
using UnityEngine;

public class EnemyLookAtCamera : MonoBehaviour
{
    private CinemachineVirtualCamera lockOnCamera;

    void Start()
    {
        lockOnCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void OnEnable()
    {
        PlayerLockOn.OnLockOnTargetFound += SetCameraLookAt;
    }

    void OnDisable()
    {
        PlayerLockOn.OnLockOnTargetFound -= SetCameraLookAt;
    }

    public void SetCameraLookAt(Transform enemyCameraRoot)
    {
        if (enemyCameraRoot == null)
        {
            lockOnCamera.LookAt = null;
            return;
        }
        lockOnCamera.LookAt = enemyCameraRoot;
    }
}
