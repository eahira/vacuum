using UnityEngine;
using UnityEngine.InputSystem;

public class SphereAbility : MonoBehaviour
{
    [Header("Sphere Settings")]
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private float activeTime = 2f;
    [SerializeField] private float cooldownTime = 4f;

    private float cooldownTimer;

    public float CooldownTime => cooldownTime;
    public float CooldownTimer => cooldownTimer;
    public bool IsOnCooldown => cooldownTimer > 0f;
    public bool CanUseSphere => cooldownTimer <= 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer < 0f)
                cooldownTimer = 0f;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && CanUseSphere)
        {
            CreateSphere();
        }
    }

    private void CreateSphere()
    {
        GameObject sphere = Instantiate(
            spherePrefab,
            transform.position,
            Quaternion.identity
        );

        Destroy(sphere, activeTime);

        cooldownTimer = cooldownTime;
    }
}