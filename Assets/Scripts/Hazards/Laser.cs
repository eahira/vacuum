using UnityEngine;

public class Laser : MonoBehaviour, IVacuumAffected
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Collider2D col;

    private float cooldownTime = 2f;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (col == null) col = GetComponent<Collider2D>();
    }

    public void ActivateVacuum()
    {
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        Invoke(nameof(DeactivateVacuum), cooldownTime);
    }

    public void DeactivateVacuum()
    {
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }
}