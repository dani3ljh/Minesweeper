using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    
    void Awake()
    {
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnStart;
            if(s.playOnStart){
                s.source.Play();
            }
        }
    }
    
    // Method PlaySound returns the length of the sound
    public float PlaySound(string name)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning($"Sound {name} not found!");
            return 0f;
        }
        s.source.Play();
        return s.clip.length;
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    
    public AudioClip clip;
    
    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;
    public bool loop;
    public bool playOnStart;
    
    [HideInInspector]
    public AudioSource source;
}
