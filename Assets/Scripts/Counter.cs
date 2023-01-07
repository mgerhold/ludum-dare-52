using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum DishLocationStatus {
    Free,
    Occupied,
    Reserved,
}

public struct DishLocationInfo {
    public Counter counter;
    public Transform transform;
    public int index;
}

public class Counter : TaskTarget {
    [SerializeField] private Transform[] dishLocations;
    private Dish[] _dishes;
    private DishLocationStatus[] _dishLocationStatuses;

    private void Start() {
        _dishes = new Dish[dishLocations.Length];
        _dishLocationStatuses = new DishLocationStatus[dishLocations.Length];
        for (int i = 0; i < _dishLocationStatuses.Length; ++i) {
            _dishLocationStatuses[i] = DishLocationStatus.Free;
        }
    }

    public DishLocationInfo? ReserveDishLocation() {
        var possibilities = new List<DishLocationInfo>();
        for (int i = 0; i < dishLocations.Length; ++i) {
            if (_dishLocationStatuses[i] == DishLocationStatus.Free) {
                possibilities.Add(new DishLocationInfo {
                    counter = this,
                    transform = dishLocations[i],
                    index = i,
                });
            }
        }
        if (!possibilities.Any()) {
            return null;
        }
        var index = UnityEngine.Random.Range(0, possibilities.Count);
        _dishLocationStatuses[possibilities[index].index] = DishLocationStatus.Reserved;
        return possibilities[index];
    }

    public void DeliverDish(DishLocationInfo location, Dish dish) {
        var index = location.index;
        Debug.Assert(_dishLocationStatuses[index] == DishLocationStatus.Reserved);
        _dishLocationStatuses[index] = DishLocationStatus.Occupied;
        dish.transform.position = location.transform.position;
        _dishes[index] = dish;
        dish.PickedUpCallback = carryable => {
            _dishLocationStatuses[index] = DishLocationStatus.Free;
            _dishes[index] = null;
            carryable.PickedUpCallback = null;
        };
    }
}