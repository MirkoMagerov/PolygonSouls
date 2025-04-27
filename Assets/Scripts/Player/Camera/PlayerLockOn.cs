using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    [SerializeField] private Transform currentTarget;
    [SerializeField] SimpleLockOn simpleLockOn;
    private Animator anim;

    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private Transform enemyTargetLocator;
    [SerializeField] Animator cinemachineAnimator;

    [Header("Settings")]
    [SerializeField] private bool zeroVertLook;
    [SerializeField] private float noticeZone = 10f;
    [SerializeField] private float lookAtSmoothing = 2f;
    [SerializeField] private float maxNoticeAngle = 60f;
    [SerializeField] private float crosshairScale = 0.1f;

    [SerializeField] GameObject lockOnCanvas;

    Transform cam;
    float currentYOffset;
    Vector3 pos;

    private void Start()
    {
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            if (!TargetOnRange()) ResetTarget();
            LookAtTarget();
        }
    }

    void HandleLockOnInput(bool isPressed)
    {
        if (isPressed)
        {
            if (currentTarget)
            {
                ResetTarget();
                return;
            }

            if ((currentTarget = ScanNearBy()) != null)
            {
                FoundTarget();
            }
            else
            {
                ResetTarget();
            }
        }
    }

    void OnEnable()
    {
        StarterAssetsInputs.OnLockOnPressed += HandleLockOnInput;
    }

    void OnDisable()
    {
        StarterAssetsInputs.OnLockOnPressed -= HandleLockOnInput;
    }

    private void FoundTarget()
    {
        lockOnCanvas.SetActive(true);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 1f);
        cinemachineAnimator.Play("LockOn Camera");

        currentTarget.TryGetComponent(out EnemyHealth enemyHealth);
        enemyHealth.OnEnemyDeath += NotifyTargetDeath;
    }

    private void ResetTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.TryGetComponent(out EnemyHealth enemyHealth);
            enemyHealth.OnEnemyDeath -= NotifyTargetDeath;
        }

        lockOnCanvas.SetActive(false);
        anim.SetLayerWeight(2, 0f);
        anim.SetLayerWeight(1, 1f);
        cinemachineAnimator.Play("FreeLook Camera");
        currentTarget = null;
    }

    public void NotifyTargetDeath(Transform deadEnemy)
    {
        if (currentTarget == deadEnemy)
        {
            ResetTarget();
        }
    }

    private Transform ScanNearBy()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        Transform closestTarget = null;
        if (nearbyTargets.Length <= 0) return null;

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);

            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform;
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null;
        float h1 = closestTarget.GetComponent<CharacterController>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = h / 2 / 2;
        currentYOffset = h - half_h;
        if (zeroVertLook && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null;
        return closestTarget;
    }

    bool Blocked(Vector3 t)
    {
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out RaycastHit hit))
        {
            if (!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

    bool TargetOnRange()
    {
        float dis = (transform.position - pos).magnitude;
        if (dis / 2 > noticeZone) return false;
        else return true;
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget();
            return;
        }

        CharacterController enemyController = currentTarget.GetComponent<CharacterController>();
        float chestHeight = enemyController.height * 0.65f;

        Vector3 targetPosition = currentTarget.position + new Vector3(0, chestHeight, 0);

        lockOnCanvas.transform.position = targetPosition;
        lockOnCanvas.transform.localScale = (cam.position - targetPosition).magnitude * crosshairScale * Vector3.one;

        enemyTargetLocator.position = targetPosition;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing);
    }

    public bool GetEnemyLockedOn() { return currentTarget != null; }

    public Transform GetCurrentTarget() { return currentTarget; }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticeZone);
    }
}
