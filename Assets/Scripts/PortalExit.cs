using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PortalExit : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite inactiveSprite;
    [SerializeField] private Sprite activeSprite;

    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.F;

    private SpriteRenderer spriteRenderer;
    private bool playerInside;
    private bool isActive;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Collider2D portalCollider = GetComponent<Collider2D>();
        portalCollider.isTrigger = true;

        SetInactive();
    }

    private void Start()
    {
        if (CollectiblesManager.Instance == null)
        {
            Debug.LogError("На сцене нет CollectiblesManager.");
            return;
        }

        CollectiblesManager.Instance.OnCollectiblesChanged += OnCollectiblesChanged;

        RefreshPortalState();
    }

    private void OnDestroy()
    {
        if (CollectiblesManager.Instance != null)
            CollectiblesManager.Instance.OnCollectiblesChanged -= OnCollectiblesChanged;
    }

    private void Update()
    {
        if (!isActive)
            return;

        if (!playerInside)
            return;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current[interactKey].wasPressedThisFrame)
        {
            if (VictoryScreenUI.Instance == null)
            {
                Debug.LogError("На сцене нет VictoryScreenUI.");
                return;
            }

            VictoryScreenUI.Instance.ShowVictory();
        }
    }

    private void OnCollectiblesChanged(
        int collectedClockParts,
        int totalClockParts,
        int collectedCurrency,
        int totalCurrency
    )
    {
        RefreshPortalState();
    }

    private void RefreshPortalState()
    {
        if (CollectiblesManager.Instance == null)
            return;

        bool hasAllClockParts =
            CollectiblesManager.Instance.CollectedClockParts >=
            CollectiblesManager.Instance.TotalClockParts;

        if (hasAllClockParts)
            SetActive();
        else
            SetInactive();
    }

    private void SetInactive()
    {
        isActive = false;

        if (spriteRenderer != null && inactiveSprite != null)
            spriteRenderer.sprite = inactiveSprite;
    }

    private void SetActive()
    {
        isActive = true;

        if (spriteRenderer != null && activeSprite != null)
            spriteRenderer.sprite = activeSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = false;
    }

    private bool IsPlayer(Collider2D other)
    {
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
}