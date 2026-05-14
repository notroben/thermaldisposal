using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SoundEffect
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.5f, 1.5f)] public float pitch = 1f;
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
            globalAudioSource.PlayOneShot(s.clip, s.volume);
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
            tempSource.clip = s.clip;
            tempSource.volume = s.volume;
            tempSource.pitch = s.pitch;
            tempSource.spatialBlend = 1f;
            tempSource.minDistance = 2f;
            tempSource.maxDistance = 20f;
            tempSource.rolloffMode = AudioRolloffMode.Linear;

            tempSource.Play();
            Destroy(tempAudioObj, s.clip.length);
        }
    }
}
