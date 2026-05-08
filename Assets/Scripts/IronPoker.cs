using UnityEngine;

public class IronPoker : MonoBehaviour
{
    public float pokeRange = 4f;
    public LayerMask interactableMask;

    public void TryPoke(Transform cameraTransform)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, pokeRange);

        foreach (RaycastHit hit in hits)
        {
            DebrisLogic debris = hit.collider.GetComponent<DebrisLogic>();
            if (debris != null)
            {
                debris.Poke();
                break;
            }
        }
    }
}