using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public float mainVolume = 1f;
	public float sfxVolume = 1f;
	public float endSfxVolume = 1f;
	
	public Sound[] sounds;
	
	void Awake()
	{
		foreach(Sound s in sounds){
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
			s.source.playOnAwake = s.playOnStart;
			if(s.playOnStart){
				s.source.Play();
			}
		}
		
		UpdateSoundVolume();
	}
	
	public void UpdateSoundVolume()
	{
		foreach(Sound s in sounds){
			switch(s.type){
				case Sound.AudioTypes.SFX:
					s.source.volume = mainVolume * sfxVolume;
					break;
				case Sound.AudioTypes.EndSFX:
					s.source.volume = mainVolume * endSfxVolume;
					break;
				default:
				case Sound.AudioTypes.Main:
					s.source.volume = mainVolume;
					break;
			}
		}
	}
	
	/// <returns>Length of the sound clip</returns>
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
	
	public enum AudioTypes 
	{ 
		SFX, 
		EndSFX, 
		Main 
	}
	public AudioTypes type;
	
	[Range(.1f, 3f)]
	public float pitch;
	public bool loop;
	public bool playOnStart;
	
	[HideInInspector]
	public AudioSource source;
}
