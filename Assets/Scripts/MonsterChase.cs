using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MonsterChase : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform playerTransform;
    public string playerTag = "Player";

    [Header("Movement Settings")]
    public float walkSpeed = 2.0f;
    public float dashSpeed = 6.0f;
    public float dashRange = 5.0f;
    public float detectionRange = 15.0f;

    private NavMeshAgent agent;
    private Animator animator;

    // Animator Parameter Hashes
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > detectionRange)
        {
            StopChase();
        }
        else if (distance <= dashRange)
        {
            DashChase();
        }
        else
        {
            WalkChase();
        }

        // Update animator parameters based on speed or state
        UpdateAnimation(distance);
    }

    private void WalkChase()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.SetDestination(playerTransform.position);
    }

    private void DashChase()
    {
        agent.isStopped = false;
        agent.speed = dashSpeed;
        agent.SetDestination(playerTransform.position);
    }

    private void StopChase()
    {
        agent.isStopped = true;
        // Optionally rotate to face player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    private void UpdateAnimation(float distance)
    {
        if (animator == null) return;

        bool isMoving = !agent.isStopped && agent.velocity.magnitude > 0.1f;
        bool isDashing = isMoving && distance <= dashRange;

        animator.SetBool(IsWalkingHash, isMoving && !isDashing);
        animator.SetBool(IsDashingHash, isDashing);
    }
}
