using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float speed = 2f;
    public Vector3 moveDirection = Vector3.forward;

    [Header("System Status")]
    public bool isBroken = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("ConveyorBelt requires a Rigidbody component set to Is Kinematic!");
        }
    }

    void FixedUpdate()
    {
        if (isBroken || rb == null) return;

        Vector3 currentPos = rb.position;
        rb.position -= moveDirection.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(currentPos);
    }
}