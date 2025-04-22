using System.Collections;
using UnityEngine;

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
            Vector3 targetPosition = target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(targetPosition);
            yield return null;
        }
    }
}