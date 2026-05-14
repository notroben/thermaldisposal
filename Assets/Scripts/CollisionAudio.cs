using UnityEngine;

public class CollisionAudio : MonoBehaviour
{
    public string impactSoundName = "DropBagDry";
    public float minImpactForce = 1.0f;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > minImpactForce)
        {
            if (ServiceLocator.AudioManager != null)
            {
                ServiceLocator.AudioManager.PlaySFXAtPosition(impactSoundName, transform.position);
            }
        }
    }
}
