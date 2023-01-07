using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order {
    public float spawnTime;
    public List<PlantType> ingredients;
}

public class OrderQueueManager : MonoBehaviour {
    public List<Order> Orders { get; private set; } = new();

    public static OrderQueueManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        Orders.Add(new Order {
            spawnTime = Time.time + 10f,
            ingredients = new List<PlantType> { PlantType.Wheat, PlantType.Wheat },
        });
    }
}