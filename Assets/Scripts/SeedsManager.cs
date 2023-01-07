using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedsManager : MonoBehaviour {
    [SerializeField] private Transform[] seedSpawns = null;
    [SerializeField] private PricesScriptableObject prices = null;
    private Seeds[] seeds = null;

    public static SeedsManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        seeds = new Seeds[seedSpawns.Length];
    }

    public bool IsFull() {
        foreach (var status in seeds) {
            if (status is null) {
                return false;
            }
        }
        return true;
    }

    public void SpawnSeedsById(int id) {
        SpawnSeeds((PlantType)id);
    }

    public void SpawnSeeds(PlantType plantType) {
        Debug.Assert(!IsFull());
        var possibleIndices = new List<int>();
        for (int i = 0; i < seedSpawns.Length; ++i) {
            if (seeds[i] is null) {
                possibleIndices.Add(i);
            }
        }
        var price = prices.prices[plantType];
        MoneyManager.Instance.Money -= price;
        var index = possibleIndices[UnityEngine.Random.Range(0, possibleIndices.Count)];
        var prefab = PrefabManager.Instance.seedsPrefabs[plantType];
        var spawnedObject = GameObject.Instantiate(prefab, seedSpawns[index].position, Quaternion.identity);
        var seedsScript = spawnedObject.GetComponent<Seeds>();
        seeds[index] = seedsScript;
        seedsScript.PickedUpCallback = carryable => {
            seeds[index] = null;
            carryable.PickedUpCallback = null;
        };
    }
}