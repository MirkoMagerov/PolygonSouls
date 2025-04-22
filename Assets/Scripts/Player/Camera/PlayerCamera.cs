using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Camera")]
    public GameObject CameraTarget;
    private float topClampFreeLook = 60.0f;
    private float bottomClampFreeLook = -25.0f;
    private float topClampLockOn = 35.0f;
    private float bottomClampLockOn = -15.0f;

    private float targetYaw;
    private float targetPitch;
    private StarterAssetsInputs input;
    private PlayerLockOn playerLockOn;

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        playerLockOn = GetComponent<PlayerLockOn>();
    }

    private void Start()
    {
        if (CameraTarget == null)
        {
            CameraTarget = GameObject.FindGameObjectWithTag("CinemachineTarget");
        }

        targetYaw = CameraTarget.transform.rotation.eulerAngles.y;
    }

    public void UpdateCamera()
    {
        const float threshold = 0.01f;

        if (playerLockOn.GetEnemyLockedOn())
        {
            Transform target = playerLockOn.GetCurrentTarget();
            Vector3 direction = target.position - CameraTarget.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

            Vector3 euler = targetRotation.eulerAngles;
            euler.x = ClampAngle(euler.x, bottomClampLockOn, topClampLockOn);

            CameraTarget.transform.rotation = Quaternion.Euler(euler.x, euler.y, 0.0f);

            return;
        }

        if (input.look.sqrMagnitude >= threshold)
        {
            float deltaTime = IsUsingMouse() ? 1.0f : Time.deltaTime;
            targetYaw += input.look.x * deltaTime;
            targetPitch += input.look.y * deltaTime;
        }

        targetYaw = ClampAngle(targetYaw, float.MinValue, float.MaxValue);
        targetPitch = ClampAngle(targetPitch, bottomClampFreeLook, topClampFreeLook);

        CameraTarget.transform.rotation = Quaternion.Euler(targetPitch, targetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    private bool IsUsingMouse()
    {
        return GetComponent<PlayerInput>().currentControlScheme == "KeyboardMouse";
    }
}