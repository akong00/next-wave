using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour {
	public static bool gameActive;

	public GameObject pauseMask;
	public GameObject settingsMask;
	public GameObject helpMask;
	public float buttonCooldown = 5f;
	public GameObject[] NextWaveButtons;
	public Toggle musicToggle;
	public Toggle sfxToggle;
	public Slider sensitivitySlider;
    public Slider powerupsSlider;

	GameObject[] audioPlayers;
	Button[] buttons;
	SoundManager soundManager;

	void Awake()
	{
		gameActive = true;
		audioPlayers = GameObject.FindGameObjectsWithTag("SFXPlayer");
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		buttons = Resources.FindObjectsOfTypeAll<Button>();
		foreach(Button button in buttons)
			button.onClick.AddListener(PlayButtonSound);
	}

	void Start()
	{
		//needs to be in start() or else will invoke callback from toggle change and SoundManager references not established yet
		if(PlayerPrefs.GetInt("Music", 1) == 0) musicToggle.isOn = false;
		if(PlayerPrefs.GetInt("SFX", 1) == 0) sfxToggle.isOn = false;

		//Set Sensitivity Slider
		sensitivitySlider.value = GunMechanics.sensitivity = PlayerPrefs.GetFloat("Sensitivity", 3);

        //Set Powerups Slider
        powerupsSlider.value = PowerupSpawner.powerupMultiplier = PlayerPrefs.GetInt("PowerupMultiplier", 1);
    }

	public void PlayButtonSound()
	{
		soundManager.buttonSound.Play();
	}

	public void OnClickNextWave()
	{
		StartCoroutine(DisableButtons());
	}

	void Update()
	{
		if(PlayerHealth.gameEnd)
		{
			//Disable buttons for good
			foreach(GameObject button in NextWaveButtons)
			{
				if(button != null)
					button.GetComponent<Button>().interactable = false;
			}
		}
	}

	IEnumerator DisableButtons()
	{
		foreach(GameObject button in NextWaveButtons)
		{
			if(button != null)
				button.GetComponent<Button>().interactable = false;
		}
		yield return new WaitForSeconds(buttonCooldown);
		foreach(GameObject button in NextWaveButtons)
		{
			if(button != null)
				button.GetComponent<Button>().interactable = true;
		}
	}

	public void OnClickPauseButton()
	{
		Time.timeScale = 0;
		gameActive = false;
		foreach(GameObject audioPlayer in audioPlayers)
		{
			if(audioPlayer != null)
			{
				if(audioPlayer.name != "SoundManager")//don't pause music
					audioPlayer.GetComponent<AudioSource>().Pause();
			}
		}
		pauseMask.SetActive(true);
	}

	public void OnClickResumeButton()
	{
		Time.timeScale = 1;
		gameActive = true;
		foreach(GameObject audioPlayer in audioPlayers)
		{
			if(audioPlayer != null)
				audioPlayer.GetComponent<AudioSource>().UnPause();
		}
		pauseMask.SetActive(false);
	}

	public void OnClickHomeButton()
	{
		SceneManager.LoadScene("Menu");
	}

	public void OnClickRestartButton()
	{
		SceneManager.LoadScene("Game");
	}

	public void OnClickSettingsButton()
	{
		settingsMask.SetActive(true);
	}

	public void OnClickReturnButton()
	{
		settingsMask.SetActive(false);
		if(helpMask != null)
			helpMask.SetActive(false);
		PlayerPrefs.Save();
	}

	public void OnClickMusicToggle(Toggle toggle)
	{
		if(toggle.isOn) PlayerPrefs.SetInt("Music", 1);
		else PlayerPrefs.SetInt("Music", 0);
		soundManager.ToggleMusic(toggle.isOn);
	}

	public void OnClickSFXToggle(Toggle toggle)
	{
		if(toggle.isOn) PlayerPrefs.SetInt("SFX", 1);
		else PlayerPrefs.SetInt("SFX", 0);
		soundManager.ToggleSFX(toggle.isOn);
	}

    public void OnClickPowerupsSlider(Slider slider)
    {
        PowerupSpawner.powerupMultiplier = slider.value;
        PlayerPrefs.SetInt("PowerupMultiplier", Mathf.RoundToInt(slider.value));
    }

	public void OnClickSensitivitySlider(Slider slider)
	{
		GunMechanics.sensitivity = slider.value;
		PlayerPrefs.SetFloat("Sensitivity", slider.value);
	}

	public void OnClickResetButton()
	{
		GunMechanics.sensitivity = sensitivitySlider.value = 3;
		PlayerPrefs.SetFloat("Sensitivity", 3);
        PowerupSpawner.powerupMultiplier = powerupsSlider.value = 1;
        PlayerPrefs.SetInt("PowerupMultiplier", 1);
	}

	//Menu Buttons------------------------------------

	public void OnClickSinglePlayer()
	{
		PlayerPrefs.SetInt("mode", 1);
		PlayerPrefs.Save();
		SceneManager.LoadScene("Game");
	}

	public void OnClickTwoPlayer()
	{
		PlayerPrefs.SetInt("mode", 2);
		PlayerPrefs.Save();
		SceneManager.LoadScene("Game");
	}

	public void OnClickHelpButton()
	{
		helpMask.SetActive(true);
	}
}
