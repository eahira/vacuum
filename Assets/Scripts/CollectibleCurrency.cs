using UnityEngine;

public class CollectibleCurrency : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCurrency(amount);
        }

        Destroy(gameObject);
    }
}