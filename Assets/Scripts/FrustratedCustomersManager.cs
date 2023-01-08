using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrustratedCustomersManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI frustratedCustomersText = null;
    private const int MaxFrustratedCustomers = 10;
    private int _count = 0;

    public int Count {
        get => _count;
        set {
            _count = value;
            frustratedCustomersText.text = $"{_count}/{MaxFrustratedCustomers} frustrated customers";
        }
    }

    public bool GameOver => Count >= MaxFrustratedCustomers;

    public static FrustratedCustomersManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }
}