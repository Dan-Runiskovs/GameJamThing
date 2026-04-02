using UnityEngine;

public class PlayerAvatarVisuals : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Material[] bodyMaterials; // 4 skin color materials

    [Header("Hats")]
    [SerializeField] private GameObject[] hats; // 5 entries: 0 = no hat slot can be null, 1..4 = hats

    public void ApplySelection(int colorIndex, int hatIndex)
    {
        ApplyColor(colorIndex);
        ApplyHat(hatIndex);
    }

    public void ApplyColor(int colorIndex)
    {
        if (bodyRenderer == null) return;
        if (bodyMaterials == null || bodyMaterials.Length == 0) return;
        if (colorIndex < 0 || colorIndex >= bodyMaterials.Length) return;

        bodyRenderer.material = bodyMaterials[colorIndex];
    }

    public void ApplyHat(int hatIndex)
    {
        if (hats == null || hats.Length == 0) return;

        for (int i = 0; i < hats.Length; i++)
        {
            if (hats[i] != null)
                hats[i].SetActive(false);
        }

        if (hatIndex >= 0 && hatIndex < hats.Length)
        {
            if (hats[hatIndex] != null)
                hats[hatIndex].SetActive(true);
        }
    }
}