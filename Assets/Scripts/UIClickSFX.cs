using UnityEngine;

public class UIClickSFX : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlayClick()
    {
        if (clickSound != null && audioSource != null) audioSource.PlayOneShot(clickSound, volume);
    }
}
