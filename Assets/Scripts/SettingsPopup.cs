using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
	public Slider mainVolumeSlider;
	public Slider sfxVolumeSlider;
	public Slider endSfxVolumeSlider;
	
	private Animator anim;
	
	[HideInInspector] public GameManager gm;
	[HideInInspector] public AudioManager am;

	void Start()
	{
		anim = GetComponent<Animator>();
	}
	
	public void ReturnToMainMenu()
	{
		am.UpdateSoundVolume();
		anim.SetTrigger("Rise");
		Destroy(gameObject, 0.5f);
	}
	
	public void SetVolume(string volumeType)
	{
		switch (volumeType.ToEnum<Sound.AudioTypes>(Sound.AudioTypes.Main))
		{
			case Sound.AudioTypes.Main:
				am.mainVolume = mainVolumeSlider.value;
				break;
			case Sound.AudioTypes.SFX:
				am.sfxVolume = sfxVolumeSlider.value;
				break;
			case Sound.AudioTypes.EndSFX:
				am.endSfxVolume = endSfxVolumeSlider.value;
				break;
		}
	}
	
}

