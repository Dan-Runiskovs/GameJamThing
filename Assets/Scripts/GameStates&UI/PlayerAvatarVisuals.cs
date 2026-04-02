using UnityEngine;

public class PlayerAvatarVisuals : MonoBehaviour
{
    [Header("Body")]
    [SerializeField] private Renderer bodyRenderer;
    [SerializeField] private Material[] bodyMaterials; 

    [Header("Hat Spawning")]
    [SerializeField] private Transform hatSocket;
    [SerializeField] private GameObject[] hatPrefabs; //1 empty option

    private GameObject currentHatInstance;

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

        Material[] mats = bodyRenderer.materials;

        if (mats.Length < 2)
        {
            Debug.LogWarning("Expected at least 2 materials (eyes + body).");
            return;
        }

        mats[1] = bodyMaterials[colorIndex]; // only changes body material

        bodyRenderer.materials = mats;
    }

    public void ApplyHat(int hatIndex)
    {
        if (hatSocket == null) return;

        // Remove old hat
        if (currentHatInstance != null)
        {
            Destroy(currentHatInstance);
            currentHatInstance = null;
        }

        
        if (hatPrefabs == null || hatPrefabs.Length == 0) return;
        if (hatIndex <= 0 || hatIndex >= hatPrefabs.Length) return;

        GameObject hatPrefab = hatPrefabs[hatIndex];
        if (hatPrefab == null) return;

        currentHatInstance = Instantiate(hatPrefab, hatSocket);
        currentHatInstance.transform.localPosition = Vector3.zero;
        currentHatInstance.transform.localRotation = Quaternion.identity;
        currentHatInstance.transform.localScale = Vector3.one;
    }
}