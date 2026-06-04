using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float acceleration = 70f;
    [SerializeField] private float deceleration = 110f;
    [SerializeField] private float airAcceleration = 45f;
    [SerializeField] private float airDeceleration = 25f;
    [SerializeField] private float stopThreshold = 0.05f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 13f;
    [SerializeField] private float jumpCutMultiplier = 0.45f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Gravity")]
    [SerializeField] private float fallGravityMultiplier = 2.2f;
    [SerializeField] private float maxFallSpeed = 18f;

    [Header("Ground Check")]
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundRayLength = 0.16f;
    [SerializeField] private float groundRayInset = 0.12f;

    [Header("Edge Fix")]
    [SerializeField] private float edgeFallForce = 2f;

    private Rigidbody2D rb;

    private float horizontalInput;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private float defaultGravityScale;

    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (bodyCollider == null)
            bodyCollider = GetComponent<Collider2D>();

        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        ReadInput();
        CheckGrounded();
        UpdateTimers();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferTimer = jumpBufferTime;
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCutMultiplier
            );
        }
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            Jump();
        }

        ApplyBetterGravity();
        ClampFallSpeed();
    }

    private void ReadInput()
    {
        horizontalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            horizontalInput = -1f;

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            horizontalInput = 1f;
    }

    private void CheckGrounded()
    {
        Bounds bounds = bodyCollider.bounds;

        Vector2 leftOrigin = new Vector2(
            bounds.min.x + groundRayInset,
            bounds.min.y + 0.02f
        );

        Vector2 centerOrigin = new Vector2(
            bounds.center.x,
            bounds.min.y + 0.02f
        );

        Vector2 rightOrigin = new Vector2(
            bounds.max.x - groundRayInset,
            bounds.min.y + 0.02f
        );

        bool leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, groundRayLength, groundLayer);
        bool centerHit = Physics2D.Raycast(centerOrigin, Vector2.down, groundRayLength, groundLayer);
        bool rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, groundRayLength, groundLayer);

        bool stableGround = centerHit || (leftHit && rightHit);
        bool onlyEdgeGround = !centerHit && (leftHit != rightHit);

        isGrounded = stableGround;

        if (onlyEdgeGround && rb.linearVelocity.y <= 0f)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -edgeFallForce
            );
        }
    }

    private void UpdateTimers()
    {
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;
    }

    private void Move()
    {
        float targetSpeed = horizontalInput * moveSpeed;

        float currentAcceleration;

        if (isGrounded)
        {
            currentAcceleration = Mathf.Abs(horizontalInput) > 0.01f
                ? acceleration
                : deceleration;
        }
        else
        {
            currentAcceleration = Mathf.Abs(horizontalInput) > 0.01f
                ? airAcceleration
                : airDeceleration;
        }

        float newXVelocity = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            currentAcceleration * Time.fixedDeltaTime
        );

        if (isGrounded && Mathf.Abs(horizontalInput) < 0.01f && Mathf.Abs(newXVelocity) < stopThreshold)
        {
            newXVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        jumpBufferTimer = 0f;
        coyoteTimer = 0f;
    }

    private void ApplyBetterGravity()
    {
        if (rb.linearVelocity.y <= 0f)
            rb.gravityScale = defaultGravityScale * fallGravityMultiplier;
        else
            rb.gravityScale = defaultGravityScale;
    }

    private void ClampFallSpeed()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -maxFallSpeed
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D selectedCollider = bodyCollider;

        if (selectedCollider == null)
            selectedCollider = GetComponent<Collider2D>();

        if (selectedCollider == null)
            return;

        Bounds bounds = selectedCollider.bounds;

        Vector2 leftOrigin = new Vector2(
            bounds.min.x + groundRayInset,
            bounds.min.y + 0.02f
        );

        Vector2 centerOrigin = new Vector2(
            bounds.center.x,
            bounds.min.y + 0.02f
        );

        Vector2 rightOrigin = new Vector2(
            bounds.max.x - groundRayInset,
            bounds.min.y + 0.02f
        );

        Gizmos.color = Color.green;

        Gizmos.DrawLine(leftOrigin, leftOrigin + Vector2.down * groundRayLength);
        Gizmos.DrawLine(centerOrigin, centerOrigin + Vector2.down * groundRayLength);
        Gizmos.DrawLine(rightOrigin, rightOrigin + Vector2.down * groundRayLength);
    }
}