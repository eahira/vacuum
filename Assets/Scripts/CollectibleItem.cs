using UnityEngine;

public enum CollectibleType
{
    ClockPart,
    Currency
}

public class CollectibleItem : MonoBehaviour
{
    [SerializeField] private CollectibleType type;
    [SerializeField] private int amount = 1;

    public CollectibleType Type => type;
    public int Amount => amount;

    private bool isCollected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
            return;

        if (!other.CompareTag("Player"))
            return;

        isCollected = true;

        if (CollectiblesManager.Instance != null)
        {
            CollectiblesManager.Instance.Collect(type, amount);
        }

        Destroy(gameObject);
    }
}