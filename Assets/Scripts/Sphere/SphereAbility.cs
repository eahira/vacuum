using UnityEngine;
using UnityEngine.InputSystem;

public class SphereAbility : MonoBehaviour
{
    [Header("Sphere Settings")]
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private float activeTime = 2f;
    [SerializeField] private float cooldownTime = 4f;

    private bool canUseSphere = true;

    private void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && canUseSphere)
        {
            CreateSphere();
        }
    }

    private void CreateSphere()
    {
        canUseSphere = false;

        GameObject sphere = Instantiate(
            spherePrefab,
            transform.position,
            Quaternion.identity
        );

        Destroy(sphere, activeTime);

        Invoke(nameof(ResetCooldown), cooldownTime);
    }

    private void ResetCooldown()
    {
        canUseSphere = true;
    }
}