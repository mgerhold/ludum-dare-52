using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI moneyText = null;
    public long Money { get; set; } = 0;
    public static MoneyManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Update() {
        moneyText.text = $"{Money} TKM";
    }
}