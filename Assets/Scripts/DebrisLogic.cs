using UnityEngine;

public class DebrisLogic : MonoBehaviour
{
    [Header("Debris State")]
    public bool isCharred = false;

    [Header("Visual")]
    public Material charredMaterial; // BLACKWASHING!!!

    private MeshRenderer[] meshRenderers;

    void Start()
    {
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        if (isCharred) ApplyCharVisual();
    }

    public void CharDebris()
    {
        if (isCharred) return;

        isCharred = true;

        if (meshRenderers == null || meshRenderers.Length == 0)
            meshRenderers = GetComponentsInChildren<MeshRenderer>();

        ApplyCharVisual();
    }

    void ApplyCharVisual()
    {
        if (meshRenderers != null)
        {
            foreach (MeshRenderer mr in meshRenderers)
            {
                if (charredMaterial != null)
                    mr.sharedMaterial = charredMaterial;
                else
                    mr.material.color = new Color(0.1f, 0.1f, 0.1f);
            }
        }

        gameObject.layer = 0;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = 0;
        }
    }

    public void Poke()
    {
        if (isCharred)
        {
            Destroy(gameObject);
        }
    }
}