using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MessagePanelManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMessage = null;
    private float _showUntil = 0f;

    private const float MessageDuration = 5f;
    
    public static MessagePanelManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void ShowMessage(string text) {
        _showUntil = Time.time + MessageDuration;
        textMessage.text = text;
        gameObject.SetActive(true);
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    private void Update() {
        if (Time.time >= _showUntil) {
            gameObject.SetActive(false);
        }
    }
}
