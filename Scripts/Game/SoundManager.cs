using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

	static bool musicCreated;

	public AudioSource music;
	public AudioSource buttonSound;

	GameObject[] sfxs;

	void Awake()
	{
		UpdateSounds();

		//allows continuous music
		if(!musicCreated)
		{
			music.Play();
			DontDestroyOnLoad(gameObject);
			musicCreated = true;
		}

		//when scene changes sfxs needs to be updated
		SceneManager.activeSceneChanged += OnChangedScene;
	}

	void OnChangedScene(Scene current, Scene next)
	{
		UpdateSounds();
	}

	void UpdateSounds()
	{
		sfxs = GameObject.FindGameObjectsWithTag("SFXPlayer");

		//default is on, but need to set to off otherwise
		if(PlayerPrefs.GetInt("SFX", 1) == 1) ToggleSFX(true);
		else ToggleSFX(false);
	}

	public void ToggleMusic(bool isOn)
	{
		if(isOn) music.Play();
		else music.Stop();
	}

	public void ToggleSFX(bool isOn)
	{
		foreach(GameObject sfx in sfxs)
		{
			if(sfx != null)
			{
				AudioSource[] audioSources = sfx.GetComponents<AudioSource>();
				//don't mute music
				if(sfx.name == "SoundManager") audioSources[1].mute = !isOn;
				else
				{
					foreach(AudioSource audioSource in audioSources)
						audioSource.mute = !isOn;
				}
			}
		}
	}
}
