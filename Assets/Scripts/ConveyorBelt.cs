using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float speed = 2f;
    public Vector3 moveDirection = Vector3.forward;

    [Header("System Status")]
    public bool isBroken = false;

    [Header("Belt Visual")]
    public Renderer beltRenderer;
    public float textureScrollSpeed = 0.5f;
    public bool scrollAxisX = false;

    private Rigidbody rb;
    private Material beltMaterial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogWarning("ConveyorBelt requires a Rigidbody component set to Is Kinematic!");
        if (beltRenderer != null) beltMaterial = beltRenderer.material;
    }

    void FixedUpdate()
    {
        if (isBroken || rb == null) return;

        Vector3 currentPos = rb.position;
        rb.position -= moveDirection.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(currentPos);

        if (beltMaterial != null)
        {
            Vector2 offset = beltMaterial.mainTextureOffset;
            if (scrollAxisX) offset.x += textureScrollSpeed * Time.fixedDeltaTime;
            else offset.y += textureScrollSpeed * Time.fixedDeltaTime;
            beltMaterial.mainTextureOffset = offset;
        }
    }
}