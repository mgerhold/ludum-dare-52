using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Order {
    public float spawnTime;
    public List<PlantType> ingredients;
    public GameObject customer = null;
}

public class OrderQueueManager : MonoBehaviour {
    [SerializeField] private Transform[] customerSpawnLocations = null;
    [SerializeField] private Transform customerWaitLocation = null;

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

    private void Update() {
        foreach (var order in Orders) {
            var remainingTime = order.spawnTime - Time.time;
            if (remainingTime <= 0f && order.customer is null) {
                // spawn customer
                var prefab = PrefabManager.Instance.customerPrefabs[
                    UnityEngine.Random.Range(0, PrefabManager.Instance.customerPrefabs.Length)];
                var spawnLocation =
                    customerSpawnLocations[UnityEngine.Random.Range(0, customerSpawnLocations.Length)];
                order.customer = GameObject.Instantiate(prefab, spawnLocation.position, Quaternion.identity);
                var navMeshAgent = order.customer.GetComponent<NavMeshAgent>();
                navMeshAgent.SetDestination(customerWaitLocation.position);
            }
        }
    }
}