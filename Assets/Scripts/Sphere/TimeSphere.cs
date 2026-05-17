using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    public float activeTime = 2f;
    public float radius = 3f;

    private readonly List<IFreezeable> frozenObjects = new();
    private readonly List<IVacuumAffected> vacuumObjects = new();

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            IFreezeable freezeable = hit.GetComponentInParent<IFreezeable>();
            if (freezeable != null && !frozenObjects.Contains(freezeable))
            {
                freezeable.Freeze();
                frozenObjects.Add(freezeable);
            }

            IVacuumAffected vacuum = hit.GetComponentInParent<IVacuumAffected>();
            if (vacuum != null && !vacuumObjects.Contains(vacuum))
            {
                vacuum.ActivateVacuum();
                vacuumObjects.Add(vacuum);
            }
        }

        Destroy(gameObject, activeTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}