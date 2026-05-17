using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [SerializeField] private SphereAbility sphereAbility;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TMP_Text cooldownText;

    private void Update()
    {
        if (sphereAbility == null || cooldownFill == null || cooldownText == null)
            return;

        if (sphereAbility.IsOnCooldown)
        {
            float progress = sphereAbility.CooldownTimer / sphereAbility.CooldownTime;

            cooldownFill.fillAmount = progress;
            cooldownText.text = Mathf.CeilToInt(sphereAbility.CooldownTimer).ToString();
        }
        else
        {
            cooldownFill.fillAmount = 0f;
            cooldownText.text = "E";
        }
    }
}