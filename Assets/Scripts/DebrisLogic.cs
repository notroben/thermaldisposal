using UnityEngine;

public class DebrisLogic : MonoBehaviour
{
    [Header("Debris State")]
    public bool isCharred = false; // BLACKWASHING!

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (isCharred) ApplyCharVisual();
    }

    public void CharDebris()
    {
        if (isCharred) return;

        isCharred = true;
        ApplyCharVisual();
    }

    void ApplyCharVisual()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = new Color(0.1f, 0.1f, 0.1f);
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