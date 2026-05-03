using UnityEngine;
using System.Collections.Generic;

public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public float speed = 2f;
    public Vector3 moveDirection = Vector3.forward;

    [Header("System Status")]
    public bool isBroken = false;

    private HashSet<Rigidbody> rigidbodiesToMove = new HashSet<Rigidbody>();

    void FixedUpdate()
    {
        if (isBroken)
        {
            rigidbodiesToMove.Clear();
            return;
        }

        foreach (Rigidbody rb in rigidbodiesToMove)
        {
            if (rb != null)
            {
                Vector3 movement = speed * Time.fixedDeltaTime * moveDirection.normalized;
                rb.MovePosition(rb.position + movement);
            }
        }

        rigidbodiesToMove.Clear();
    }

    void OnCollisionStay(Collision collision)
    {
        if (isBroken) return;

        Rigidbody rb = collision.rigidbody;

        if (rb != null && !rb.isKinematic)
        {
            rigidbodiesToMove.Add(rb);
        }
    }
}