using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput inputActions;
    private InputSystem_Actions inputSystemActions;
    [HideInInspector] public Animator anim;
    private Transform cameraPosition;
    [SerializeField] private AudioSource playerEffects;
    [SerializeField] private AudioSource eqRadio;
    [SerializeField] private GameObject poisonEffect;
    public GameObject kick;

    [Header("Movement")]
    [SerializeField] private Animator flashAnim;
    [SerializeField] private Transform orientation;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private bool readyToJump;
    private bool crouching = false;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public Vector2 input {  get; private set; }

    [HideInInspector] public bool isSliding;

    public MovementState state;
    public enum MovementState
    {
        walking,
        air,
        crouching,
    }

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Ground Check")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float playerHeight;

    [HideInInspector] public bool isGrounded;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYscale;

    [Header("Interact")]
    [SerializeField] private float interactRange = 2.5f; 
    [HideInInspector] public Interactable closestInteractable = null;
    private bool canInteract;

    [Header("Health")]
    public int maxHealth = 100;
    [HideInInspector] public int health = 100;

    bool isEnded;

    int numberOfHits;
    Coroutine poisonCR;

    bool isPaused;
    public GameObject pausePanel;

    private void Awake()
    {
        inputSystemActions = new InputSystem_Actions();
        inputSystemActions.Player.Enable();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        inputActions = GetComponent<PlayerInput>();
        cameraPosition = FindFirstObjectByType<Camera>().transform;
        rb = GetComponent<Rigidbody>();
         

        inputSystemActions.Player.Interact.performed += InteractPerformed;

        rb.freezeRotation = true;

        moveSpeed = walkSpeed;
        desiredMoveSpeed = walkSpeed;
        lastDesiredMoveSpeed = walkSpeed;
        startYscale = transform.localScale.y;
        readyToJump = true;

        canInteract = false;

        health = maxHealth;
    }

    private void Update()
    {
        if (health < 0) health = 0;

        if (health <= 0) return;
        AxixInputs();
        StateHandler();
        SpeedControl();
        GroundCheck();

        GetInteractions();

        if (canInteract)
        {
            InteactAction();
            canInteract = false;
        }

        if (state == MovementState.crouching)
        {
            kick.SetActive(false);
        }
        else
        {
            kick.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void Pause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        }
    }

    #region Inputs & Actions

    private void AxixInputs()
    {
        input = inputActions.actions["Move"].ReadValue<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;
    }

    public void JumpAction()
    {
        if (readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    public void CrouchAction()
    {
        if (!crouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            crouching = true;
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
            crouching = false;
        }
        
    }

    #region Interact

    void GetInteractions()
    {
        Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);

        float currentDot = 0;
        closestInteractable = null;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (Vector3.Distance(interactables[i].transform.position, transform.position) < interactRange && !interactables[i].triggered)
            {
                Vector3 direction = interactables[i].transform.position - cameraPosition.position;
                float dot = Vector3.Dot(direction, cameraPosition.forward);

                if (dot > currentDot)
                {
                    currentDot = dot;
                    closestInteractable = interactables[i];
                }
            }
        }
    }

    private void InteractPerformed(InputAction.CallbackContext context)
    {
        canInteract = true;
        StartCoroutine(Wait());
    }

    public void InteactAction()
    {
        if (closestInteractable != null)
            closestInteractable.Interact();
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.01f);
        canInteract = false;
    }

    #endregion

    #endregion

    private void StateHandler()
    {
        if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        else
            state = MovementState.air;

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
            moveSpeed = desiredMoveSpeed;

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    #region Move Things

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        if (isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!isGrounded)
            rb.AddForce(moveDirection.normalized * moveSpeed / 2 * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.25f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.25f, whatIsGround);

        if (isGrounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    #endregion

    #region Health

    public void RecieveDamage(int amount)
    {
        health -= amount;

        if (health <= 0 && !isEnded)
        {
            health = 0;
            isEnded = true;

            flashAnim.SetTrigger("Die");

            rb.constraints = RigidbodyConstraints.None;
            rb.AddTorque(Random.Range(-1,1), Random.Range(-1, 1), Random.Range(-1, 1) * 0.35f, ForceMode.Impulse);
            rb.linearDamping = 1;
        }
        else if (health > 0) 
        {
            flashAnim.SetTrigger("Flash");
        }
    }

    public void PoisonDamage(int amount, int numberOfHits, float timeBetween)
    {
        if (poisonCR == null) poisonCR = StartCoroutine(RecievePoisonDamage(amount, numberOfHits, timeBetween));
        else this.numberOfHits = numberOfHits;
    }

    IEnumerator RecievePoisonDamage(int amount, int numberOfHits, float timeBetween)
    {
        this.numberOfHits = numberOfHits;

        poisonEffect.SetActive(true);

        for (; this.numberOfHits > 0; this.numberOfHits--)
        {
            float t = 0;
            RecieveDamage(amount);

            while (t < timeBetween)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        poisonEffect.SetActive(false);
        poisonCR = null;
    }

    public void HealDamage(int amount)
    {
        if (health + amount < maxHealth) 
            health += amount;
        else 
            health = maxHealth;
    }

    #endregion

}
