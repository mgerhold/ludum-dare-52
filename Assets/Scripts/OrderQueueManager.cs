using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Order {
    public float spawnTime;
    public List<PlantType> ingredients;
    public Customer customer = null;
    public Dish targetDish = null;
    public bool fulfilled = false;

    public bool AcceptsDish(Dish dish) {
        var providedIngredients = dish.ingredients.ToList();
        foreach (var desiredIngredient in ingredients) {
            bool foundIngredient = false;
            for (int i = 0; i < providedIngredients.Count; ++i) {
                if (providedIngredients[i] == desiredIngredient) {
                    providedIngredients.RemoveAt(i);
                    foundIngredient = true;
                    break;
                }
            }
            if (!foundIngredient) {
                return false;
            }
        }
        return providedIngredients.Count == 0;
    }

    public long MoneyValue() {
        var elapsedTime = Time.time - spawnTime;
        Debug.LogWarning($"elapsed time {elapsedTime}");
        return ingredients.Count * 5 + Mathf.RoundToInt(60f * Mathf.Pow(0.8f, elapsedTime));
    }
}

public class OrderQueueManager : MonoBehaviour {
    [SerializeField] private Transform[] customerSpawnLocations = null;
    [SerializeField] private Transform[] customerLeaveLocations = null;
    [SerializeField] private Transform customerWaitLocation = null;
    private List<Dish> _deliveredDishes = new();
    private float timeOfNextOrder;

    private const float DishDistanceThreshold = 1f;

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
        var spawnInterval = 20f * Mathf.Pow(0.994f, Time.time);
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
        var maxNumIngredients = (int)(Time.time / 20f);
        var numIngredients = UnityEngine.Random.Range(1, Math.Min(2, maxNumIngredients + 1));
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
        if (Time.time >= timeOfNextOrder) {
            Orders.Add(new Order {
                spawnTime = Time.time + 15f,
                ingredients = GetRandomIngredients(),
            });
            SetTimeOfNextOrder();
        }

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

                    // todo: rating...
                    MoneyManager.Instance.Money += order.MoneyValue();
                    var leaveLocation =
                        customerLeaveLocations[UnityEngine.Random.Range(0, customerLeaveLocations.Length)];
                    order.customer.GetComponent<NavMeshAgent>().SetDestination(leaveLocation.position);
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
}