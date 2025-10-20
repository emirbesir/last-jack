using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GhostMenuWander : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float idleWanderSpeed = 1f;
    [SerializeField] private float wanderRadius = 5f;

    // State
    private Rigidbody rb;
    private Vector3 startingPosition;
    private Vector3 idleTargetPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        startingPosition = transform.position;
        StartCoroutine(UpdateIdleTargetPosition());
    }

    void FixedUpdate()
    {
        HandleIdle();
    }

    private void HandleIdle()
    {
        Vector3 directionToTarget = (idleTargetPosition - transform.position).normalized;
        RotateTowards(idleTargetPosition);
        rb.MovePosition(rb.position + idleWanderSpeed * Time.fixedDeltaTime * directionToTarget);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        // Rotate only on the Y axis
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            Quaternion targetRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private IEnumerator UpdateIdleTargetPosition()
    {
        while (true)
        {
            idleTargetPosition = new Vector3(
                startingPosition.x + Random.Range(-wanderRadius, wanderRadius),
                startingPosition.y,
                startingPosition.z + Random.Range(-wanderRadius, wanderRadius)
            );
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }
}
