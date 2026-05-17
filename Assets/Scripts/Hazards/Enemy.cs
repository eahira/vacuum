using UnityEngine;

public class Enemy : MonoBehaviour, IFreezeable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;

    [Header("Freeze Settings")]
    [SerializeField] private float freezeTime = 2f;

    private Vector2 startPosition;
    private int direction = 1;
    private bool isFrozen = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float distanceFromStart = transform.position.x - startPosition.x;

        if (distanceFromStart >= moveDistance)
            direction = -1;
        else if (distanceFromStart <= -moveDistance)
            direction = 1;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    public void Freeze()
    {
        if (isFrozen) return;

        isFrozen = true;
        rb.linearVelocity = Vector2.zero;

        Invoke(nameof(Unfreeze), freezeTime);
    }

    public void Unfreeze()
    {
        isFrozen = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isFrozen) return;
        if (!collision.collider.CompareTag("Player")) return;

        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.Die();
        }
    }
}