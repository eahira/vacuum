using UnityEngine;

public class TestEnemyMovement : MonoBehaviour, IFreezeable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float moveDistance = 3f;

    private Vector2 startPosition;
    private int direction = 1;
    private bool isFrozen;

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
        {
            direction = -1;
        }
        else if (distanceFromStart <= -moveDistance)
        {
            direction = 1;
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    public void Freeze()
    {
        isFrozen = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
    }

    public void Unfreeze()
    {
        rb.gravityScale = 3f;
        isFrozen = false;
    }
}