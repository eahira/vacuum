using UnityEngine;

public class ClockPartCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (ArtifactManager.Instance != null)
        {
            ArtifactManager.Instance.AddPart();
        }

        Destroy(gameObject);
    }
}