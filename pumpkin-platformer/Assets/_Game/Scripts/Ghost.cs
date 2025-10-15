using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Ghost : MonoBehaviour
{
    private const string PLAYER_LAYER_NAME = "Player";

    private enum State
    {
        Idle,
        Chasing
    }

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float idleWanderSpeed = 1f;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float detectionRange = 10f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackForce = 300f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float damageToPlayerFlame = 5f;

    // State
    private State currentState = State.Idle;
    private Rigidbody rb;
    private Transform playerTransform;
    private Rigidbody playerRb;
    private float attackTimer = 0f;
    private Vector3 startingPosition;
    private Vector3 idleTargetPosition;

    // References
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Start()
    {
        player = Flame.Instance.gameObject.transform;
        playerTransform = player.transform;
        playerRb = player.GetComponent<Rigidbody>();

        startingPosition = transform.position;
        StartCoroutine(UpdateIdleTargetPosition());
    }

    void Update()
    {
        if (Flame.Instance.IsGlowing && Vector3.Distance(transform.position, playerTransform.position) <= detectionRange)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Idle;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        if (currentState == State.Chasing)
        {
            HandleChasing();
        }
        else
        {
            HandleIdle();
        }
    }

    private void HandleIdle()
    {
        Vector3 directionToTarget = (idleTargetPosition - transform.position).normalized;
        RotateTowards(idleTargetPosition);
        rb.MovePosition(rb.position + idleWanderSpeed * Time.fixedDeltaTime * directionToTarget);
    }

    private void HandleChasing()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        RotateTowards(playerTransform.position);
        rb.MovePosition(rb.position + chaseSpeed * Time.fixedDeltaTime * directionToPlayer);

        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange && attackTimer <= 0)
        {
            PerformWindGustAttack();
        }
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

    private void PerformWindGustAttack()
    {
        if (playerRb == null) return;

        ScreenManager.Instance.SetGlitchEffect();

        attackTimer = attackCooldown;

        Vector3 pushDirection = (playerTransform.position - transform.position).normalized;
        playerRb.AddForce(pushDirection * attackForce, ForceMode.Force);
        playerRb.AddTorque(0.5f * attackForce * Vector3.up, ForceMode.Force);
        Flame.Instance.DamageFlame(damageToPlayerFlame);

        StartCoroutine(ClearGlitchEffectAfterDelay(0.5f));
    }

    private IEnumerator ClearGlitchEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ScreenManager.Instance.ClearGlitchEffect();
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
