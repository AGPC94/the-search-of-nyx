using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : MonoBehaviour
{
    enum States { Patrol, Chase, Back, Sleep };
    [Header("Other")]
    [SerializeField] States state;
    [SerializeField] GameObject hitbox;

    [Header("View")]
    [SerializeField] float viewRange;
    [Range(1, 360)] [SerializeField] float viewAngle = 45;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] bool canSeePlayer;

    [Header("Patrol")]
    [SerializeField] float patrolRange;
    [SerializeField] float patrolSpeed;
    [SerializeField] float patrolAcceleration;
    [SerializeField] float patrolStopDistance;

    [Header("Chase")]
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseAcceleration;
    [SerializeField] float chaseStopDistance;

    [Header("Attack")]
    [SerializeField] Vector2 atkSize;
    [SerializeField] Vector2 atkOffset;

    [Header("Back")]
    [SerializeField] float backSpeed;
    [SerializeField] float backAcceleration;

    Vector2 originalPosition;

    Animator anim;
    NavMeshAgent agent;
    Player player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        anim = GetComponent<Animator>();

        originalPosition = transform.position;

        player = GameManager.instance.player;
    }

    void Update()
    {
        transform.rotation = new Quaternion(0, 0, transform.rotation.z, transform.rotation.w);

        Animation();

        View();

        switch (state)
        {
            case States.Patrol:
                Patrol();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Back:
                Back();
                break;
            case States.Sleep:
                Sleep();
                break;
        }

        if (DayNightSystem2D.instance.IsDay())
            state = States.Sleep;

        if (state == States.Chase)
            agent.stoppingDistance = chaseStopDistance;
        else
            agent.stoppingDistance = patrolStopDistance;


        if (state == States.Sleep)
            hitbox.SetActive(false);
        else
            hitbox.SetActive(true);
    }

    void Animation()
    {
        anim.SetBool("isIdle", state != States.Sleep && agent.velocity.magnitude < 0.1f);
        anim.SetBool("isSleeping", state == States.Sleep && agent.velocity.magnitude < 0.1f);
        anim.SetBool("isWalking", state != States.Chase && agent.velocity.magnitude >= 0.1f);
        anim.SetBool("isRunning", state == States.Chase && agent.velocity.magnitude >= 0.1f);
    }

    void Patrol()
    {
        ChangeAgentSpeedAndAcceleration(patrolSpeed, patrolAcceleration);

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(originalPosition, patrolRange, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }

        if (canSeePlayer)
        {
            AudioManager.instance.PlayOneShot("wolfHowling");
            state = States.Chase;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void Chase()
    {
        ChangeAgentSpeedAndAcceleration(chaseSpeed, chaseAcceleration);
        agent.SetDestination(player.transform.position);

        if (!canSeePlayer)
            state = States.Back;
    }

    void Back()
    {
        ChangeAgentSpeedAndAcceleration(backSpeed, backAcceleration);

        agent.SetDestination(originalPosition);

        if (canSeePlayer)
        {
            AudioManager.instance.PlayOneShot("wolfHowling");
            state = States.Chase;
        }
        else if (agent.remainingDistance <= agent.stoppingDistance)
            state = States.Patrol;

    }
    void Sleep()
    {
        ChangeAgentSpeedAndAcceleration(backSpeed, backAcceleration);

        agent.SetDestination(originalPosition);
        
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //animacion dormir
        }

        if (DayNightSystem2D.instance.IsNight())
        {
            state = States.Patrol;

        }
    }

    void View()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, viewRange, targetLayer);

        if (rangeCheck.Length > 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector2 direction = (target.position - transform.position).normalized;

            //transform.up
            if (Vector2.Angle(transform.up, direction) < viewAngle / 2)
            {
                float distance = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, direction, distance, obstacleLayer))
                    canSeePlayer = true;
                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }

    void ChangeAgentSpeedAndAcceleration(float speed, float acceleration)
    {
        agent.speed = speed;
        agent.acceleration = acceleration;
    }

    void OnDrawGizmos()
    {
        //Patrol
        if (originalPosition == Vector2.zero)
            Gizmos.DrawWireSphere(transform.position, patrolRange);
        else
            Gizmos.DrawWireSphere(originalPosition, patrolRange);

        //View
        //Gizmos.DrawWireSphere(originalPosition, viewRange);

        Vector2 viewAngleOrigin = transform.position;
        Vector2 angle01 = DirectionFromAngle(-transform.eulerAngles.z, -viewAngle / 2);
        Vector2 angle02 = DirectionFromAngle(-transform.eulerAngles.z, viewAngle / 2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(viewAngleOrigin, viewAngleOrigin + angle01 * viewRange);
        Gizmos.DrawLine(viewAngleOrigin, viewAngleOrigin + angle02 * viewRange);

        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }

    }

    Vector2 DirectionFromAngle(float eulerY, float angleInDegress)
    {
        angleInDegress += eulerY;

        return new Vector2(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), Mathf.Cos(angleInDegress * Mathf.Deg2Rad));

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            agent.velocity = Vector3.zero;
        }
    }

    void OnBecameVisible()
    {
        if (DayNightSystem2D.instance.IsNight())
        {
            gameObject.SetActive(true);
        }
    }
}
