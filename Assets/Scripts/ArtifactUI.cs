using TMPro;
using UnityEngine;

public class ArtifactUI : MonoBehaviour
{
    [SerializeField] private TMP_Text artifactText;

    private void Start()
    {
        if (ArtifactManager.Instance != null)
        {
            ArtifactManager.Instance.OnPartsChanged += UpdateArtifactText;
            UpdateArtifactText(
                ArtifactManager.Instance.CollectedParts,
                ArtifactManager.Instance.TotalParts
            );
        }
    }

    private void OnDestroy()
    {
        if (ArtifactManager.Instance != null)
        {
            ArtifactManager.Instance.OnPartsChanged -= UpdateArtifactText;
        }
    }

    private void UpdateArtifactText(int collected, int total)
    {
        artifactText.text = "Части часов: " + collected + "/" + total;
    }
}