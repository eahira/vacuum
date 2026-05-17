using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static ArtifactManager Instance;

    [SerializeField] private int collectedParts;
    [SerializeField] private int totalParts = 12;

    public int CollectedParts => collectedParts;
    public int TotalParts => totalParts;

    public System.Action<int, int> OnPartsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddPart()
    {
        collectedParts++;
        OnPartsChanged?.Invoke(collectedParts, totalParts);

        if (collectedParts >= totalParts)
        {
            Debug.Log("Все части часов собраны!");
        }
    }
}