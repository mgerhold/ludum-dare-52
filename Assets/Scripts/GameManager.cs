using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public List<Meeple> Meeples { get; private set; } = new();
    [SerializeField] private Transform _initialMeepleSpawn = null;
    private const int NumStartingSeeds = 4;

    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        SpawnMeeple(_initialMeepleSpawn.position);
        for (int i = 0; i < NumStartingSeeds; ++i) {
            SeedsManager.Instance.SpawnSeeds(PlantType.Wheat);
        }
        MoneyManager.Instance.Money = 0;
    }

    public void SpawnMeeple(Vector3 position) {
        Meeples.Add(GameObject.Instantiate(PrefabManager.Instance.meeplePrefab, position,
            Quaternion.identity).GetComponentInChildren<Meeple>());
    }

    private void Update() {
        if (FrustratedCustomersManager.Instance.GameOver) {
            GameOver.Instance.Show();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            IngameMenuManager.Instance.Show();
        }
    }
}