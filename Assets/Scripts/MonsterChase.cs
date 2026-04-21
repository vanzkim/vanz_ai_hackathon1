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
    }

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.5f;

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
        else if (distance <= dashRange)
        {
            DashChase();
        }
        else
        {
            WalkChase();
        }

        UpdateAnimation(distance);
    }

    public void ResetMonster()
    {
        isResetting = true;
        agent.isStopped = false;
        agent.speed = walkSpeed;
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

        // Determine if the agent is actively moving
        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;
        
        bool isDashing = isMoving && distance <= dashRange;
        bool isWalking = isMoving && !isDashing;

        animator.SetBool(IsWalkingHash, isWalking);
        animator.SetBool(IsDashingHash, isDashing);
    }
    }
