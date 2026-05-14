using UnityEngine;

public class IronPoker : MonoBehaviour
{
    public float pokeRange = 4f;
    public LayerMask interactableMask;

    public void TryPoke(Transform cameraTransform)
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, pokeRange);

        bool hitValid = false;

        foreach (RaycastHit hit in hits)
        {
            DebrisLogic debris = hit.collider.GetComponent<DebrisLogic>();
            if (debris != null)
            {
                debris.Poke();
                hitValid = true;
                break;
            }
        }

        if (ServiceLocator.AudioManager != null)
        {
            if (hitValid) ServiceLocator.AudioManager.PlayGlobalSFX("IronPokerPokeObject");
            else ServiceLocator.AudioManager.PlayGlobalSFX("IronPokerPoke");
        }
    }
}