using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private Image progressBarFrontImage = null;

    public float Value {
        get => progressBarFrontImage.fillAmount;
        set => progressBarFrontImage.fillAmount = value;
    }

    public bool Visible {
        get => canvas.activeSelf;
        set => canvas.SetActive(value);
    }

    private void Awake() {
        Value = 0f;
        Visible = false;
    }
}