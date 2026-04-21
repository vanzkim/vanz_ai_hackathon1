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

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isResetting = false;
    private Vector3 lastTargetPosition;
    private const float targetMoveThreshold = 0.2f;

    // Animator Parameter Hashes
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Initial setup for agent responsiveness
        if (agent != null)
        {
            agent.acceleration = 40f;
            agent.autoBraking = false;
            agent.stoppingDistance = 0.5f;
        }
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
        if (isResetting)
        {
            CheckIfReturned();
            return;
        }

        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > detectionRange)
        {
            StopChase();
        }
        else
        {
            // Only update destination if player has moved significantly
            if (Vector3.Distance(playerTransform.position, lastTargetPosition) > targetMoveThreshold)
            {
                lastTargetPosition = playerTransform.position;
                agent.SetDestination(playerTransform.position);
            }

            if (distance <= dashRange)
            {
                DashChase();
            }
            else
            {
                WalkChase();
            }
        }

        UpdateAnimation(distance);
    }

    public void ResetMonster()
    {
        isResetting = true;
        agent.isStopped = false;
        agent.speed = walkSpeed;
        agent.autoBraking = true; // Use auto-braking for returning to base
        agent.SetDestination(initialPosition);
        
        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, true);
            animator.SetBool(IsDashingHash, false);
        }
    }

    private void CheckIfReturned()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            isResetting = false;
            agent.isStopped = true;
            agent.autoBraking = false;
            transform.rotation = initialRotation;

            if (animator != null)
            {
                animator.SetBool(IsWalkingHash, false);
                animator.SetBool(IsDashingHash, false);
            }
        }
    }

    private void WalkChase()
    {
        agent.isStopped = false;
        agent.speed = walkSpeed;
    }

    private void DashChase()
    {
        agent.isStopped = false;
        agent.speed = dashSpeed;
    }

    private void StopChase()
    {
        agent.isStopped = true;
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

        // More robust movement detection: check if agent is trying to move and has some velocity
        bool isMoving = !agent.isStopped && (agent.velocity.magnitude > 0.2f || agent.remainingDistance > agent.stoppingDistance);
        
        bool isDashing = isMoving && distance <= dashRange;
        bool isWalking = isMoving && !isDashing;

        animator.SetBool(IsWalkingHash, isWalking);
        animator.SetBool(IsDashingHash, isDashing);
    }
    }
