using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float ySmoothSpeed;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        float targetX = target.position.x;
        float targetZ = target.position.z;

        float newY = Mathf.Lerp(transform.position.y, target.position.y, ySmoothSpeed * Time.deltaTime);

        transform.position = new Vector3(targetX, newY, targetZ);
    }
}
