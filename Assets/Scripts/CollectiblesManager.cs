using System;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    public static CollectiblesManager Instance { get; private set; }

    [Header("Totals")]
    [SerializeField] private bool autoCalculateTotals = true;
    [SerializeField] private int totalClockParts = 12;
    [SerializeField] private int totalCurrency = 46;

    private int collectedClockParts;
    private int collectedCurrency;

    public int CollectedClockParts => collectedClockParts;
    public int TotalClockParts => totalClockParts;

    public int CollectedCurrency => collectedCurrency;
    public int TotalCurrency => totalCurrency;

    public event Action<int, int, int, int> OnCollectiblesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (autoCalculateTotals)
        {
            CalculateTotalsFromScene();
        }

        NotifyUI();
    }

    public void Collect(CollectibleType type, int amount)
    {
        if (amount <= 0)
            amount = 1;

        switch (type)
        {
            case CollectibleType.ClockPart:
                collectedClockParts += amount;
                collectedClockParts = Mathf.Min(collectedClockParts, totalClockParts);
                break;

            case CollectibleType.Currency:
                collectedCurrency += amount;
                collectedCurrency = Mathf.Min(collectedCurrency, totalCurrency);
                break;
        }

        NotifyUI();

        if (collectedClockParts >= totalClockParts)
        {
            Debug.Log("Все части часов собраны!");
        }
    }

    private void CalculateTotalsFromScene()
    {
        totalClockParts = 0;
        totalCurrency = 0;

        CollectibleItem[] collectibles = FindObjectsByType<CollectibleItem>(
            FindObjectsInactive.Exclude,
            FindObjectsSortMode.None
        );

        foreach (CollectibleItem collectible in collectibles)
        {
            switch (collectible.Type)
            {
                case CollectibleType.ClockPart:
                    totalClockParts += collectible.Amount;
                    break;

                case CollectibleType.Currency:
                    totalCurrency += collectible.Amount;
                    break;
            }
        }
    }

    private void NotifyUI()
    {
        OnCollectiblesChanged?.Invoke(
            collectedClockParts,
            totalClockParts,
            collectedCurrency,
            totalCurrency
        );
    }
}