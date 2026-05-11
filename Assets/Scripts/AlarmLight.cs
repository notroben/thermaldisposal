using UnityEngine;

public class AlarmLight : MonoBehaviour
{
    public float rotationSpeed = 360f;

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}