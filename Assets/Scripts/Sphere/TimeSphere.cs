using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    public float activeTime = 2f;  // длительность сферы
    public float radius = 3f;

    private readonly List<IFreezeable> frozenObjects = new();
    private readonly List<IVacuumAffected> vacuumObjects = new();

    private void Start()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D hit in hits)
        {
            // враги
            IFreezeable freezeable = hit.GetComponentInParent<IFreezeable>();
            if (freezeable != null && !frozenObjects.Contains(freezeable))
            {
                freezeable.Freeze();
                frozenObjects.Add(freezeable);
            }

            // лазеры Ч передаЄм длительность сферы
            IVacuumAffected vacuum = hit.GetComponentInParent<IVacuumAffected>();
            if (vacuum != null && !vacuumObjects.Contains(vacuum))
            {
                vacuum.ActivateVacuum();
                vacuumObjects.Add(vacuum);
            }
        }

        // удалить сферу после activeTime
        Destroy(gameObject, activeTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}