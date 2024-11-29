using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInput inputActions;
    private Transform cameraPosition;

    [Header("Movement")]
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
    private float moveSpeed;
    private bool readyToJump;
    private bool crouching = false;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

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

    private void Start()
    {
        inputActions = GetComponent<PlayerInput>();
        cameraPosition = FindFirstObjectByType<Camera>().transform;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveSpeed = walkSpeed;
        desiredMoveSpeed = walkSpeed;
        lastDesiredMoveSpeed = walkSpeed;
        startYscale = transform.localScale.y;
        readyToJump = true;
    }

    private void Update()
    {
        AxixInputs();
        StateHandler();
        SpeedControl();
        GroundCheck();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void AxixInputs()
    {
        Vector2 input = inputActions.actions["Move"].ReadValue<Vector2>();
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

    public void InteactAction()
    {
        Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        Interactable closetInteractable = null;
        float currentDot = 0;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (Vector3.Distance(interactables[i].transform.position, transform.position) < interactRange)
            {
                Vector3 direction = interactables[i].transform.position - cameraPosition.position;
                float dot = Vector3.Dot(direction, cameraPosition.forward);

                if (dot < currentDot)
                {
                    currentDot = dot;
                    closetInteractable = interactables[i];
                }
            }
        }

        if (closetInteractable != null)
            closetInteractable.Interact();
    }

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

}
