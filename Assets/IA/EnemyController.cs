using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] States currentState;

    [Header("MOVEMENT")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Transform[] pathPoints;
    [SerializeField] int pathIndex;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Transform visionTf;

    [Header("COMBAT")]
    [SerializeField] int maxHp;
    [SerializeField] float detectionRange;
    int currentHp;
    [SerializeField] float stoppedTime;
    float stoppedT;


    NavMeshAgent agent;
    Transform playerCameraTf;

    enum States
    {
        wandering,
        attacking,
        stopped,
        dying
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerCameraTf = FindFirstObjectByType<Camera>().transform;
    }

    private void Update()
    {
        if (currentState == States.wandering)
        {
            if (pathPoints.Length >= 0)
            {
                if (NavMesh.SamplePosition(pathPoints[pathIndex].position, out NavMeshHit hit, 100, -1) && Vector3.Distance(hit.position, transform.position - Vector3.up * agent.baseOffset) < 0.2f)
                {
                    if (pathIndex < pathPoints.Length - 1) pathIndex++;
                    else pathIndex = 0;
                }

                targetPosition = pathPoints[pathIndex].position;
            }
            else
            {
                targetPosition = transform.position;
            }

            if (Vector3.Distance(playerCameraTf.position, transform.position) < detectionRange)
            {
                if (Physics.Raycast(visionTf.position, (playerCameraTf.position - visionTf.position).normalized, out RaycastHit hit))
                {
                    if (hit.rigidbody && hit.rigidbody.gameObject.layer == playerLayer)
                    {
                        currentState = States.attacking;
                    }
                }
            }
        }
        else if (currentState == States.attacking)
        {

        }
        else if (currentState == States.stopped)
        {
            if (stoppedT < stoppedTime)
            {
                stoppedT += Time.deltaTime;
                targetPosition = transform.position;
                agent.velocity = Vector3.zero;
            }
            else
            {
                stoppedT = 0;
                currentState = States.attacking;
            }
        }
        else if (currentState == States.dying)
        {
            if (stoppedT < stoppedTime)
            {
                stoppedT += Time.deltaTime;
                targetPosition = transform.position;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        if (currentState != States.stopped) agent.destination = targetPosition;

    }

    public void RecieveDamage(int amount)
    {
        if (currentHp - amount > 0)
        {
            currentHp -= amount;
            currentState = States.stopped;
            stoppedT = 0;
        }
        else
        {
            currentHp = 0;
            currentState = States.dying;
        }
    }
}