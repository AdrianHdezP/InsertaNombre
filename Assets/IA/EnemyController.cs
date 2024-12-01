using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Audios audios;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [SerializeField] States currentState;

    [Header("MOVEMENT")]
    [SerializeField] Vector3 targetPosition;
    [SerializeField] float attackingSpeed;
    [SerializeField] float wanderingSpeed;
    [SerializeField] Vector2 distanceRange;

    [SerializeField] Transform[] pathPoints;
    [SerializeField] int pathIndex;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] Transform visionTf;

    [Header("COMBAT")]
    [SerializeField] int maxHp;
    [SerializeField] float detectionRange;
    int currentHp;
    [SerializeField] float stoppedTime;
    float stoppedT;

    [Header("WEAPON")]
    [SerializeField] bool startClosest;
    [SerializeField] float width;
    [SerializeField] float shotTime;
    [SerializeField] Proyectile proyectilePF;

    NavMeshAgent agent;
    Transform playerCameraTf;

    bool canSeePlayer;
    float shotT;
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

        currentHp = maxHp;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (currentState == States.wandering)
        {
            agent.speed = wanderingSpeed;

            if (pathPoints.Length > 0)
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
                Debug.Log("DISTANCE CHECK");

                Debug.DrawRay(visionTf.position, playerCameraTf.position - visionTf.position);

                if (Physics.Raycast(visionTf.position, (playerCameraTf.position - visionTf.position).normalized, out RaycastHit hit, detectionRange))
                {
                    if (hit.transform.gameObject.CompareTag("Player"))
                    {
                        Debug.Log("VISION CHECK");
                        currentState = States.attacking;
                    }
                }
            }
        }
        else if (currentState == States.attacking)
        {
            if (Vector3.Distance(transform.position, playerCameraTf.position) < detectionRange) agent.speed = attackingSpeed;
            else agent.speed = wanderingSpeed;


            FindHitPosition();

            if (canSeePlayer && shotT <= 0)
            {
                anim.SetTrigger("Attack");
                shotT = shotTime;
            }


            if (shotT > 0)
            {
                shotT -= Time.deltaTime;
            }
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
                if(!startClosest)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(audios.enemyRangeDying);
                }
            }
            else
            {
                //DIE HERE
            }
        }

        
        if (currentState != States.stopped && agent.isActiveAndEnabled) agent.destination = targetPosition;



    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerMovement move)) move.PoisonDamage(1, 10, 0.8f);
    }

    public void RecieveDamage(int amount)
    {
        if (currentState == States.dying) return;

            Debug.Log("RECIEVED DAMAGE");
        if (currentHp - amount > 0)
        {
            currentHp -= amount;
            currentState = States.stopped;
            stoppedT = 0;
            anim.SetTrigger("Hit");
        }
        else
        {
            agent.enabled = false;
            GetComponent<Collider>().enabled = false;

            currentHp = 0;
            currentState = States.dying;
            anim.SetTrigger("Death");
        }
    }

    bool FindHitPosition()
    {

        Vector3 direction = (transform.position - playerCameraTf.position).normalized;

        if (CheckRays(visionTf.position))
        {
            targetPosition = transform.position;
            canSeePlayer = true;
            return true;
        }
        else if (CheckRays(targetPosition))
        {
            canSeePlayer = false;
            return true;
        }
        else
        {
            canSeePlayer = false;

            if (startClosest)
            {
                float range = distanceRange.x;

                for (; range > distanceRange.y; range += (distanceRange.y - distanceRange.x) * 0.1f)
                {
                    Vector3 middleFire = playerCameraTf.position + direction * range;

                    if (CheckRays(middleFire))
                    {
                        targetPosition = middleFire;
                        return true;
                    }

                    for (float i = 0; i < 1; i += 0.05f)
                    {
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                        rotation *= Quaternion.AngleAxis(-45 + 90 * i, Vector3.up);

                        Vector3 firePosition = playerCameraTf.position + (rotation * Vector3.forward) * range;

                        if (CheckRays(firePosition))
                        {
                            targetPosition = firePosition;
                            return true;
                        }
                    }
                }
            }
            else
            {
                float range = distanceRange.y;

                for (; range > distanceRange.x; range -= (distanceRange.y - distanceRange.x) * 0.1f)
                {
                    Vector3 middleFire = playerCameraTf.position + direction * range;

                    if (CheckRays(middleFire))
                    {
                        targetPosition = middleFire;
                        return true;
                    }

                    for (float i = 0; i < 1; i += 0.05f)
                    {
                        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                        rotation *= Quaternion.AngleAxis(-45 + 90 * i, Vector3.up);

                        Vector3 firePosition = playerCameraTf.position + (rotation * Vector3.forward) * range;

                        if (CheckRays(firePosition))
                        {
                            targetPosition = firePosition;
                            return true;
                        }
                    }
                }
            }



            targetPosition = playerCameraTf.position;
            return false;
        }
    }

    bool CheckRays(Vector3 position)
    {
        Vector3 baseOffset = visionTf.localPosition + Vector3.up * agent.baseOffset;

        Ray ray = new Ray();
        ray.origin = position;
        ray.direction = (playerCameraTf.position - ray.origin).normalized;
        Quaternion rotation = Quaternion.LookRotation(ray.direction, Vector3.up);

        if (NavMesh.SamplePosition(ray.origin, out NavMeshHit meshHit, 50, -1))
        {
            ray.origin = meshHit.position + baseOffset;

            if (Physics.Raycast(ray, out RaycastHit hit, distanceRange.y, obstacleLayer))
            {
                if (!hit.transform.gameObject.CompareTag("Player"))
                {
                    return false;
                    
                }
                else
                {
                    Debug.DrawRay(ray.origin, playerCameraTf.position - ray.origin, Color.white);
                }
            }
            else return false;
        }
        

        ray.origin = position + rotation * Vector3.right * width * 0.5f;
        ray.direction = (playerCameraTf.position - ray.origin).normalized;

        if (NavMesh.SamplePosition(ray.origin, out meshHit, 50, -1))
        {
            ray.origin = meshHit.position + baseOffset;

            if (Physics.Raycast(ray, out RaycastHit hit, distanceRange.y, obstacleLayer))
            {
                if (!hit.transform.gameObject.CompareTag("Player"))
                {
                    return false;
                }
                else
                {
                    Debug.DrawRay(ray.origin, playerCameraTf.position - ray.origin, Color.white);
                }
            }
            else return false;
        }


        ray.origin = position - rotation * Vector3.right * width * 0.5f;
        ray.direction = (playerCameraTf.position - ray.origin).normalized;

       
        if (NavMesh.SamplePosition(ray.origin, out meshHit, 50, -1))
        {
            ray.origin = meshHit.position + baseOffset;

            if (Physics.Raycast(ray, out RaycastHit hit, distanceRange.y, obstacleLayer))
            {
                if (!hit.transform.gameObject.CompareTag("Player"))
                {
                    return false;
                }
                else
                {
                    Debug.DrawRay(ray.origin, playerCameraTf.position - ray.origin, Color.white);
                }
            }
            else return false;
        }

        return true;
    }

    public void Attack()
    {
        Proyectile proyectile = Instantiate(proyectilePF, visionTf.position, Quaternion.LookRotation(playerCameraTf.position - visionTf.position));
    }

    public void RangeAttackSound()
    {
        audioSource.Play();
    }
}