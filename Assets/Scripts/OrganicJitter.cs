using UnityEngine;

public class OrganicJitter : MonoBehaviour
{
    [Header("Spasm Timers")]
    public float minWaitTime = 0.5f;
    public float maxWaitTime = 3.5f;
    public float spasmDuration = 0.05f;

    [Header("Spasm Intensity")]
    public float scaleDistortion = 0.05f;
    public float positionalShakeForce = 5f;

    private Vector3 originalScale;
    private Rigidbody rb;

    private float nextSpasmStartTime = 0f;
    private float currentSpasmEndTime = 0f;
    private bool hasPlayedSpasmAudio = false;

    void Start()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        ScheduleNextSpasm();
    }

    void Update()
    {
        if (Time.time >= nextSpasmStartTime)
        {
            if (Time.time <= currentSpasmEndTime)
            {
                if (!hasPlayedSpasmAudio)
                {
                    if (ServiceLocator.AudioManager != null) ServiceLocator.AudioManager.PlaySFXAtPosition("BagJitter", transform.position);
                    hasPlayedSpasmAudio = true;
                }

                float randX = Random.Range(-scaleDistortion, scaleDistortion);
                // float randY = Random.Range(-scaleDistortion, scaleDistortion);
                float randZ = Random.Range(-scaleDistortion, scaleDistortion);
                transform.localScale = originalScale + new Vector3(randX, randZ);

                if (rb != null)
                {
                    Vector3 randomDirection = Random.insideUnitSphere;
                    rb.AddForce(randomDirection * positionalShakeForce, ForceMode.Impulse);
                }
            }
            else
            {
                transform.localScale = originalScale;
                ScheduleNextSpasm();
            }
        }
    }

    void ScheduleNextSpasm()
    {
        nextSpasmStartTime = Time.time + Random.Range(minWaitTime, maxWaitTime);
        currentSpasmEndTime = nextSpasmStartTime + spasmDuration;
        hasPlayedSpasmAudio = false;
    }
}