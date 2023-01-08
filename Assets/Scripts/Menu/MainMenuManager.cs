using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private AudioMixer audioMixer = null;
    [SerializeField] private Slider mainVolumeSlider = null;
    [SerializeField] private Slider sfxVolumeSlider = null;
    [SerializeField] private Slider musicVolumeSlider = null;

    private void Start() {
        //OnMainVolumeChanged(0.6f);
        mainVolumeSlider.value = 0.6f;
        sfxVolumeSlider.value = 0.8f;
        musicVolumeSlider.value = 0.5f;
    }

    public void OnStartButtonClicked() {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButtonClicked() {
        Application.Quit();
    }

    public void OnMainVolumeChanged(float volume) {
        audioMixer.SetFloat("MainVolume", Mathf.Log10(volume) * 20f);
    }
    
    public void OnSfxVolumeChanged(float volume) {
        audioMixer.SetFloat("SfxVolume", Mathf.Log10(volume) * 20f);
    }
    
    public void OnMusicVolumeChanged(float volume) {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }
}