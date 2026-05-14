using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 1.5f)] public float pitch = 1f;
    public bool randomizePitch = true;
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource globalAudioSource;

    [Header("Sound Library")]
    public List<SoundEffect> sounds = new List<SoundEffect>();

    private Dictionary<string, SoundEffect> soundDictionary = new Dictionary<string, SoundEffect>();

    void Awake()
    {
        ServiceLocator.RegisterAudioManager(this);

        foreach (SoundEffect s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.name))
            {
                soundDictionary.Add(s.name, s);
            }
        }
        
        if (globalAudioSource == null)
        {
            globalAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private SoundEffect GetSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out SoundEffect sound))
        {
            return sound;
        }
        Debug.LogWarning("AudioManager: Sound not found: " + name);
        return null;
    }

    public void PlayGlobalSFX(string name)
    {
        SoundEffect s = GetSound(name);
        if (s != null && s.clip != null)
        {
            GameObject tempAudioObj = new GameObject("TempAudio_2D_" + name);
            AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
            float safeVolume = s.volume <= 0.01f ? 1f : s.volume;
            float safePitch = s.pitch <= 0.1f ? 1f : s.pitch;
            float randomPitch = s.randomizePitch ? (safePitch * Random.Range(0.9f, 1.1f)) : safePitch;

            tempSource.clip = s.clip;
            tempSource.volume = safeVolume;
            tempSource.pitch = randomPitch;
            tempSource.spatialBlend = 0f;
            
            tempSource.Play();
            Destroy(tempAudioObj, s.clip.length / randomPitch);
        }
    }

    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        SoundEffect s = GetSound(name);
        if (s != null && s.clip != null)
        {
            GameObject tempAudioObj = new GameObject("TempAudio_" + name);
            tempAudioObj.transform.position = position;

            AudioSource tempSource = tempAudioObj.AddComponent<AudioSource>();
            float safeVolume = s.volume <= 0.01f ? 1f : s.volume;
            float safePitch = s.pitch <= 0.1f ? 1f : s.pitch;
            float randomPitch = s.randomizePitch ? (safePitch * Random.Range(0.9f, 1.1f)) : safePitch;

            tempSource.clip = s.clip;
            tempSource.volume = safeVolume;
            tempSource.pitch = randomPitch;
            tempSource.spatialBlend = 1f;
            tempSource.minDistance = 2f;
            tempSource.maxDistance = 20f;
            tempSource.rolloffMode = AudioRolloffMode.Linear;

            tempSource.Play();
            Destroy(tempAudioObj, s.clip.length / randomPitch);
        }
    }
}
