using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingStone : MonoBehaviour, IFreezeable
{
    private enum StoneState
    {
        Idle,
        Warning,
        Falling,
        Landed,
        Resetting
    }

    [Header("Trigger Zone")]
    [Tooltip("Если оставить пустым, игрок будет искаться по тегу Player среди всех коллайдеров в зоне.")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2 triggerOffset = new Vector2(0f, -2.5f);
    [SerializeField] private Vector2 triggerSize = new Vector2(1.25f, 4f);

    [Header("Warning / Shake")]
    [SerializeField] private float warningTime = 0.65f;
    [SerializeField] private float shakeStrength = 0.06f;

    [Header("Fall")]
    [SerializeField] private float fallGravityScale = 3f;
    [SerializeField] private float maxFallSpeed = 18f;
    [SerializeField] private float maxFallDistance = 25f;
    [SerializeField] private bool resetAfterLanding = true;
    [SerializeField] private float landResetDelay = 1f;
    [SerializeField] private float damageResetDelay = 0.15f;

    [Header("Freeze")]
    [SerializeField] private float freezeTime = 2f;
    [SerializeField] private Color frozenColor = new Color(0.55f, 0.9f, 1f, 1f);

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Color startColor = Color.white;

    private RigidbodyType2D bodyTypeBeforeFreeze;
    private float gravityBeforeFreeze;
    private Vector2 velocityBeforeFreeze;
    private float angularVelocityBeforeFreeze;

    private StoneState state = StoneState.Idle;
    private Coroutine currentRoutine;
    private bool isFrozen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (spriteRenderer != null)
            startColor = spriteRenderer.color;

        PrepareIdleState();
    }

    private void Update()
    {
        if (state == StoneState.Idle && IsPlayerUnderStone())
        {
            StartWarning();
        }
    }

    private void FixedUpdate()
    {
        if (state != StoneState.Falling || isFrozen)
            return;

        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);

        if (transform.position.y <= startPosition.y - maxFallDistance)
            StartReset(damageResetDelay);
    }

    public void Freeze()
    {
        if (isFrozen)
        {
            CancelInvoke(nameof(Unfreeze));
            Invoke(nameof(Unfreeze), freezeTime);
            return;
        }

        isFrozen = true;

        bodyTypeBeforeFreeze = rb.bodyType;
        gravityBeforeFreeze = rb.gravityScale;
        velocityBeforeFreeze = rb.linearVelocity;
        angularVelocityBeforeFreeze = rb.angularVelocity;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (spriteRenderer != null)
            spriteRenderer.color = frozenColor;

        CancelInvoke(nameof(Unfreeze));
        Invoke(nameof(Unfreeze), freezeTime);
    }

    public void Unfreeze()
    {
        if (!isFrozen)
            return;

        isFrozen = false;

        if (spriteRenderer != null)
            spriteRenderer.color = startColor;

        if (state == StoneState.Falling)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = fallGravityScale;
            rb.linearVelocity = velocityBeforeFreeze;
            rb.angularVelocity = angularVelocityBeforeFreeze;
            return;
        }

        rb.bodyType = bodyTypeBeforeFreeze;
        rb.gravityScale = gravityBeforeFreeze;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    private bool IsPlayerUnderStone()
    {
        Vector2 center = (Vector2)transform.position + triggerOffset;

        Collider2D[] hits = playerLayer.value == 0
            ? Physics2D.OverlapBoxAll(center, triggerSize, 0f)
            : Physics2D.OverlapBoxAll(center, triggerSize, 0f, playerLayer);

        foreach (Collider2D hit in hits)
        {
            if (IsPlayerCollider(hit))
                return true;
        }

        return false;
    }

    private bool IsPlayerCollider(Collider2D other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        Transform parent = other.transform.parent;

        while (parent != null)
        {
            if (parent.CompareTag("Player"))
                return true;

            parent = parent.parent;
        }

        return false;
    }

    private void StartWarning()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(WarningThenFallRoutine());
    }

    private IEnumerator WarningThenFallRoutine()
    {
        state = StoneState.Warning;

        float timer = 0f;

        while (timer < warningTime)
        {
            if (!isFrozen)
            {
                timer += Time.deltaTime;

                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shakeStrength, shakeStrength),
                    Random.Range(-shakeStrength * 0.35f, shakeStrength * 0.35f),
                    0f
                );

                transform.position = startPosition + shakeOffset;
            }

            yield return null;
        }

        transform.position = startPosition;
        StartFalling();
    }

    private void StartFalling()
    {
        state = StoneState.Falling;
        currentRoutine = null;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravityScale;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != StoneState.Falling || isFrozen)
            return;

        if (TryDamagePlayer(collision.collider))
            return;

        Land();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state != StoneState.Falling || isFrozen)
            return;

        TryDamagePlayer(other);
    }

    private bool TryDamagePlayer(Collider2D other)
    {
        if (!IsPlayerCollider(other))
            return false;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Die();

        StartReset(damageResetDelay);
        return true;
    }

    private void Land()
    {
        state = StoneState.Landed;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (resetAfterLanding)
            StartReset(landResetDelay);
    }

    private void StartReset(float delay)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ResetRoutine(delay));
    }

    private IEnumerator ResetRoutine(float delay)
    {
        state = StoneState.Resetting;

        float timer = 0f;

        while (timer < delay)
        {
            if (!isFrozen)
                timer += Time.deltaTime;

            yield return null;
        }

        PrepareIdleState();
        currentRoutine = null;
    }

    private void PrepareIdleState()
    {
        CancelInvoke(nameof(Unfreeze));

        isFrozen = false;
        state = StoneState.Idle;

        transform.position = startPosition;
        transform.rotation = startRotation;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.freezeRotation = true;

        if (spriteRenderer != null)
            spriteRenderer.color = startColor;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((Vector2)transform.position + triggerOffset, triggerSize);
    }
}