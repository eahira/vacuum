using System.Collections.Generic;
using UnityEngine;

public class TimeSphere : MonoBehaviour
{
    private readonly List<IFreezeable> frozenObjects = new();

    private void OnTriggerEnter2D(Collider2D other)
    {
        IFreezeable freezeable = other.GetComponent<IFreezeable>();

        if (freezeable != null && !frozenObjects.Contains(freezeable))
        {
            freezeable.Freeze();
            frozenObjects.Add(freezeable);
        }
    }

    private void OnDestroy()
    {
        foreach (IFreezeable freezeable in frozenObjects)
        {
            if (freezeable != null)
            {
                freezeable.Unfreeze();
            }
        }
    }
}