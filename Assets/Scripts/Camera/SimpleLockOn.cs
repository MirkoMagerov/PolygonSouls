using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleLockOn : MonoBehaviour
{
    [SerializeField] Transform target;

    void OnEnable()
    {
        target = Camera.main.transform;
        StartCoroutine(LookAtTarget());
    }

    IEnumerator LookAtTarget()
    {
        while (gameObject.activeInHierarchy)
        {
            if (target != null)
            {
                Vector3 targetPosition = target.position - transform.position;
                targetPosition.y = transform.position.y; // Keep the y position of the camera
                transform.rotation = Quaternion.LookRotation(targetPosition);
            }
            yield return null;
        }
    }
}