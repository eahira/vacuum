using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour, IFreezeable
{
    private enum MovementMode
    {
        Waypoints,
        Circle
    }

    private enum WaypointLoopMode
    {
        PingPong,
        Loop
    }

    [Header("Movement Mode")]
    [SerializeField] private MovementMode movementMode = MovementMode.Waypoints;

    [Header("Waypoints Movement")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private WaypointLoopMode waypointLoopMode = WaypointLoopMode.PingPong;
    [SerializeField] private bool snapToFirstWaypoint = true;
    [SerializeField] private bool teleportToFirstPointAtEnd = false;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float arriveDistance = 0.04f;

    [Header("Circle Movement")]
    [SerializeField] private Transform circleCenter;
    [SerializeField] private float circleRadius = 2f;
    [SerializeField] private float circleDegreesPerSecond = 90f;
    [SerializeField] private bool clockwise = true;

    [Header("Freeze Settings")]
    [SerializeField] private float freezeTime = 2f;
    [SerializeField] private Color frozenColor = new Color(0.55f, 0.9f, 1f, 1f);
    [SerializeField] private bool becomeGroundWhenFrozen = true;
    [SerializeField] private string frozenGroundLayerName = "Ground";

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Color startColor;
    private int startLayer;

    private bool isFrozen;
    private int targetWaypointIndex = 1;
    private int waypointDirection = 1;
    private float circleAngle;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        startLayer = gameObject.layer;

        if (spriteRenderer != null)
            startColor = spriteRenderer.color;

        PrepareStartPosition();
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        switch (movementMode)
        {
            case MovementMode.Waypoints:
                MoveByWaypoints();
                break;

            case MovementMode.Circle:
                MoveByCircle();
                break;
        }
    }

    private void PrepareStartPosition()
    {
        if (movementMode == MovementMode.Waypoints)
        {
            if (waypoints == null || waypoints.Length < 2)
                return;

            if (snapToFirstWaypoint && waypoints[0] != null)
                transform.position = waypoints[0].position;

            targetWaypointIndex = Mathf.Clamp(targetWaypointIndex, 1, waypoints.Length - 1);
        }

        if (movementMode == MovementMode.Circle)
        {
            if (circleCenter == null)
                return;

            Vector2 offset = (Vector2)transform.position - (Vector2)circleCenter.position;

            if (offset.sqrMagnitude > 0.001f)
            {
                circleRadius = offset.magnitude;
                circleAngle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            }
        }
    }

    private void MoveByWaypoints()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Transform targetPoint = waypoints[targetWaypointIndex];

        if (targetPoint == null)
            return;

        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = targetPoint.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, targetPosition) <= arriveDistance)
        {
            GoToNextWaypoint();
        }
    }

    private void GoToNextWaypoint()
    {
        if (waypointLoopMode == WaypointLoopMode.Loop)
        {
            if (targetWaypointIndex >= waypoints.Length - 1)
            {
                if (teleportToFirstPointAtEnd)
                {
                    rb.position = waypoints[0].position;
                    transform.position = waypoints[0].position;
                    targetWaypointIndex = 1;
                }
                else
                {
                    targetWaypointIndex = 0;
                }
            }
            else
            {
                targetWaypointIndex++;
            }

            return;
        }

        if (targetWaypointIndex >= waypoints.Length - 1)
            waypointDirection = -1;
        else if (targetWaypointIndex <= 0)
            waypointDirection = 1;

        targetWaypointIndex += waypointDirection;
    }

    private void MoveByCircle()
    {
        if (circleCenter == null)
            return;

        float direction = clockwise ? -1f : 1f;

        circleAngle += direction * circleDegreesPerSecond * Time.fixedDeltaTime;

        float radians = circleAngle * Mathf.Deg2Rad;

        Vector2 center = circleCenter.position;
        Vector2 nextPosition = center + new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ) * circleRadius;

        rb.MovePosition(nextPosition);
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

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (spriteRenderer != null)
            spriteRenderer.color = frozenColor;

        if (becomeGroundWhenFrozen)
        {
            int groundLayer = LayerMask.NameToLayer(frozenGroundLayerName);

            if (groundLayer != -1)
                SetLayerRecursively(gameObject, groundLayer);
        }

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

        SetLayerRecursively(gameObject, startLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (isFrozen)
            return;

        if (!IsPlayer(other))
            return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Die();
    }

    private bool IsPlayer(Collider2D other)
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

    private void SetLayerRecursively(GameObject targetObject, int layer)
    {
        targetObject.layer = layer;

        foreach (Transform child in targetObject.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (movementMode == MovementMode.Waypoints)
        {
            if (waypoints == null || waypoints.Length < 2)
                return;

            Gizmos.color = Color.yellow;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null)
                    continue;

                Gizmos.DrawWireSphere(waypoints[i].position, 0.12f);

                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            if (waypointLoopMode == WaypointLoopMode.Loop && !teleportToFirstPointAtEnd)
            {
                if (waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
                    Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            }
        }

        if (movementMode == MovementMode.Circle)
        {
            if (circleCenter == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(circleCenter.position, circleRadius);
        }
    }
}