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

    // Animator Parameter Hashes
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsDashingHash = Animator.StringToHash("IsDashing");

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        if (agent != null)
        {
            agent.acceleration = 60f; // High acceleration for immediate response
            agent.autoBraking = false;
            agent.stoppingDistance = 0.5f;
        }
    }

    void Start()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null) playerTransform = player.transform;
        }

        // Snap to NavMesh if not already on it
        if (agent != null && !agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    void Update()
    {
        if (VanzAI.Managers.CutsceneManager.Instance.IsCutsceneActive)
        {
            StopChase();
            UpdateAnimation(detectionRange + 1f); // Ensure idle animation
            return;
        }

        if (isResetting)
        {
            CheckIfReturned();
            return;
        }

        if (playerTransform == null || agent == null) return;

        // Recovery: If agent is not on NavMesh, try to warp it back
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
            else
            {
                // If still not on NavMesh, we can't do pathfinding
                UpdateAnimation(0f);
                return;
            }
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > detectionRange)
        {
            StopChase();
        }
        else
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
                
                if (distance <= dashRange)
                {
                    agent.speed = dashSpeed;
                }
                else
                {
                    agent.speed = walkSpeed;
                }
            }
        }

        UpdateAnimation(distance);
    }

    public void ResetMonster()
    {
        isResetting = true;
        if (agent != null && agent.isActiveAndEnabled)
        {
            // Ensure on NavMesh before setting properties
            if (!agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(initialPosition, out hit, 5.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = walkSpeed;
                agent.autoBraking = true;
                agent.SetDestination(initialPosition);
            }
        }
        
        if (animator != null)
        {
            animator.SetBool(IsWalkingHash, true);
            animator.SetBool(IsDashingHash, false);
        }
    }

    private void CheckIfReturned()
    {
        if (agent != null && agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
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

    private void StopChase()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
        
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    private void UpdateAnimation(float distance)
    {
        if (animator == null || agent == null) return;

        // Use velocity and isOnNavMesh to determine if actually moving
        bool isMoving = agent.isOnNavMesh && agent.velocity.sqrMagnitude > 0.1f;
        bool isInRange = distance <= detectionRange;
        
        bool isDashing = isInRange && distance <= dashRange && isMoving;
        bool isWalking = isInRange && !isDashing && isMoving;

        animator.SetBool(IsWalkingHash, isWalking);
        animator.SetBool(IsDashingHash, isDashing);
    }
    }
