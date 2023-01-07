using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : SerializedMonoBehaviour {
    public GameObject meeplePrefab = null;
    public GameObject tilledGroundPrefab = null;
    public Dictionary<PlantType, GameObject> plantPrefabs = null;
    public GameObject[] dishPrefabs = null;
    public GameObject[] customerPrefabs = null;
    public Dictionary<PlantType, GameObject> seedsPrefabs = null;

    public static PrefabManager Instance { get; private set; }
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }
}
