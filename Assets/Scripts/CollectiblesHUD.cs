using TMPro;
using UnityEngine;

public class CollectiblesHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text clockPartsText;
    [SerializeField] private TMP_Text currencyText;

    private void Start()
    {
        if (CollectiblesManager.Instance == null)
        {
            Debug.LogWarning("CollectiblesManager не найден на сцене.");
            return;
        }

        CollectiblesManager.Instance.OnCollectiblesChanged += UpdateHUD;

        UpdateHUD(
            CollectiblesManager.Instance.CollectedClockParts,
            CollectiblesManager.Instance.TotalClockParts,
            CollectiblesManager.Instance.CollectedCurrency,
            CollectiblesManager.Instance.TotalCurrency
        );
    }

    private void OnDestroy()
    {
        if (CollectiblesManager.Instance != null)
        {
            CollectiblesManager.Instance.OnCollectiblesChanged -= UpdateHUD;
        }
    }

    private void UpdateHUD(
        int collectedClockParts,
        int totalClockParts,
        int collectedCurrency,
        int totalCurrency
    )
    {
        clockPartsText.text = $"{collectedClockParts}/{totalClockParts}";
        currencyText.text = $"{collectedCurrency}/{totalCurrency}";
    }
}