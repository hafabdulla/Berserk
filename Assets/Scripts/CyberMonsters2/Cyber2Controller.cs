using UnityEngine;
using UnityEngine.AI;

public class Cyber2Controller : MonoBehaviour
{
    public float attackRange = 5f;
    public float damagePerHit = 20f;
    public float attackCooldown = 1.5f;
    public Transform[] patrolPoints;  // set in inspector or find by tag
    public float detectionRange = 10f;

    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;

    private int currentPatrolIndex = 0;
    private float lastAttackTime = 0f;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (!enabled || player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= detectionRange)
        {
            agent.speed = runSpeed;
            agent.SetDestination(player.position);
            agent.isStopped = false;

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0; // keep zombie upright
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 5f);

            if (anim != null)
            {
                anim.SetBool("Walking", false);
                anim.SetBool("Running", true); 
            }

            // Attack if in range
            if (distToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                agent.isStopped = true;
                anim?.SetTrigger("Attack");

                PlayerStats ps = player.GetComponent<PlayerStats>();
                if (ps != null) ps.TakeDamage(damagePerHit);

                lastAttackTime = Time.time;
            }


        }
        else
        {
            // PATROL MODE
            agent.speed = walkSpeed;
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false;
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);

        if (anim != null)
        {
            anim.SetBool("Walking", true);
            anim.SetBool("Running", false);
        }

        float distance = Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position);
        if (distance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
}
