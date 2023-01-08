using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenuManager : MonoBehaviour {
    public static IngameMenuManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void OnContinueButtonClicked() {
        Hide();
    }

    public void OnBackToMenuButtonClicked() {
        SceneManager.LoadScene(0);
    }
}