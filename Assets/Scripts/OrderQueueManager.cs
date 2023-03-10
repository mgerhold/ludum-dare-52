using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class OrderQueueManager : MonoBehaviour {
    [SerializeField] private Transform[] customerSpawnLocations = null;
    [SerializeField] private Transform[] customerLeaveLocations = null;
    [SerializeField] private Transform customerWaitLocation = null;
    private List<Dish> _deliveredDishes = new();
    private float timeOfNextOrder;

    private const float DishDistanceThreshold = 1f;
    private const float MinMaxWaitTime = 20f;
    private const float MaxMaxWaitTime = 45f;

    public List<Order> Orders { get; private set; } = new();

    public static OrderQueueManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        SetTimeOfNextOrder();
    }

    private void SetTimeOfNextOrder() {
        var spawnInterval = 30f * Mathf.Pow(0.999f, Time.time);
        var actualInterval = spawnInterval + UnityEngine.Random.Range(-4f, 4f);
        timeOfNextOrder = Time.time + actualInterval;
    }

    public void OnDishDelivered(Dish dish) {
        Debug.Log("A dish got delivered");
        _deliveredDishes.Add(dish);
    }

    public void OnDishTakenAway(Dish dish) {
        foreach (var order in Orders) {
            if (order.targetDish == dish) {
                order.customer.GetComponent<NavMeshAgent>().SetDestination(customerWaitLocation.position);
                order.targetDish = null;
            }
        }
        var successfullyRemoved = _deliveredDishes.Remove(dish);
        Debug.Assert(successfullyRemoved);
    }

    private static List<PlantType> GetRandomIngredients() {
        var maxNumIngredients = (int)(Time.time / 70f);
        var numIngredients = UnityEngine.Random.Range(1, Math.Clamp(maxNumIngredients + 1, 2, 4));
        if (maxNumIngredients <= 1) {
            return new List<PlantType> { PlantType.Wheat };
        }
        var result = new List<PlantType>();
        var ingredients = (PlantType[])Enum.GetValues(typeof(PlantType));
        for (int i = 0; i < numIngredients; ++i) {
            var ingredient = ingredients[UnityEngine.Random.Range(0, ingredients.Length - 1)];
            result.Add(ingredient);
        }
        return result;
    }

    private void Update() {
        TryCreateNewOrder();

        var ordersToDelete = new List<Order>();

        foreach (var order in Orders) {
            var isCustomerOnMap = order.customer is not null;
            if (Time.time >= order.spawnTime && !isCustomerOnMap) {
                // spawn customer
                var prefab = PrefabManager.Instance.customerPrefabs[
                    UnityEngine.Random.Range(0, PrefabManager.Instance.customerPrefabs.Length)];
                var spawnLocation =
                    customerSpawnLocations[UnityEngine.Random.Range(0, customerSpawnLocations.Length)];
                order.customer = GameObject.Instantiate(prefab, spawnLocation.position, Quaternion.identity)
                    .GetComponent<Customer>();
                var navMeshAgent = order.customer.GetComponent<NavMeshAgent>();
                navMeshAgent.SetDestination(customerWaitLocation.position);
                isCustomerOnMap = true;
            }

            var waitTime = Time.time - order.spawnTime;
            if (isCustomerOnMap && waitTime >= order.maxWaitTime) {
                // wait time exceeded
                ++FrustratedCustomersManager.Instance.Count;
                LeaveMap(order);
                ordersToDelete.Add(order);
                order.customer.PlayFailureSound();
                continue;
            }

            if (order.fulfilled) {
                continue;
            }

            var customerHasTargetDish = order.targetDish is not null;
            if (isCustomerOnMap && customerHasTargetDish) {
                var dishGroundPosition = order.targetDish.transform.position;
                dishGroundPosition.y = 0f;
                var distance = Vector3.Distance(order.customer.transform.position, dishGroundPosition);
                if (distance < DishDistanceThreshold) {
                    Debug.Log("Order fulfilled");
                    // take dish
                    if (order.customer.handPosition is null) {
                        Debug.LogError("Hand position is not set");
                    } else {
                        order.targetDish.transform.position = order.customer.handPosition.position;
                    }
                    order.targetDish.transform.parent = order.customer.gameObject.transform;

                    order.targetDish.OnPickedUp();
                    order.targetDish = null;
                    order.fulfilled = true;
                    order.customer.PlayMoneySound();

                    MoneyManager.Instance.Money += order.MoneyValue();
                    LeaveMap(order);
                    ordersToDelete.Add(order);
                }
            }

            var dishTargetKnown = order.targetDish is not null;
            if (order.fulfilled || !isCustomerOnMap || dishTargetKnown) {
                continue;
            }
            foreach (var dish in _deliveredDishes) {
                if (order.AcceptsDish(dish)) {
                    order.customer.GetComponent<NavMeshAgent>()
                        .SetDestination(Utilities.GetValidTargetPosition(dish.transform.position));
                    order.targetDish = dish;
                    break;
                }
            }
        }

        foreach (var orderToDelete in ordersToDelete) {
            var successfullyRemoved = Orders.Remove(orderToDelete);
            Debug.Assert(successfullyRemoved);
        }
    }

    private void LeaveMap(Order order) {
        var leaveLocation =
            customerLeaveLocations[UnityEngine.Random.Range(0, customerLeaveLocations.Length)];
        order.customer.GetComponent<NavMeshAgent>().SetDestination(leaveLocation.position);
    }

    private void TryCreateNewOrder() {
        if (Time.time >= timeOfNextOrder) {
            var ingredients = GetRandomIngredients();
            Orders.Add(new Order {
                spawnTime = Time.time + 25f,
                ingredients = ingredients,
                maxWaitTime = UnityEngine.Random.Range(MinMaxWaitTime * ingredients.Count,
                    MaxMaxWaitTime * ingredients.Count),
            });
            SetTimeOfNextOrder();
        }
    }
}