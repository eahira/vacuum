using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private Vector3 respawnPoint;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnPoint = transform.position;
    }

    public void SetCheckpoint(Vector3 checkpointPosition)
    {
        respawnPoint = checkpointPosition;
        Debug.Log("Checkpoint saved: " + checkpointPosition);
    }

    public void Die()
    {
        Respawn();
    }

    private void Respawn()
    {
        transform.position = respawnPoint;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player respawned");
    }
}