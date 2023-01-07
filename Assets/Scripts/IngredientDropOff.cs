using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDropOff : MonoBehaviour {
    [SerializeField] private Transform[] dropOffLocations = null;
    private Carryable[] droppedOffItems = null;

    private void Start() {
        droppedOffItems = new Carryable[dropOffLocations.Length];
    }

    private int FirstFreeIndex() {
        Debug.Assert(dropOffLocations.Length == droppedOffItems.Length);
        for (int i = 0; i < dropOffLocations.Length; ++i) {
            if (droppedOffItems[i] is null) {
                return i;
            }
        }
        throw new InvalidOperationException();
    }

    public bool IsFull() {
        foreach (var carryable in droppedOffItems) {
            if (carryable is null) {
                return false;
            }
        }
        return true;
    }

    public bool IsEmpty() {
        foreach (var carryable in droppedOffItems) {
            if (carryable is not null) {
                return false;
            }
        }
        return true;
    }

    private void DetachItem(Carryable item) {
        for (int i = 0; i < droppedOffItems.Length; ++i) {
            if (droppedOffItems[i] == item) {
                droppedOffItems[i] = null;
                return;
            }
        }
        throw new InvalidOperationException("Item to remove is not contained inside ingredient drop off");
    }

    public bool DropOff(Carryable item) {
        if (IsFull()) {
            return false;
        }
        var index = FirstFreeIndex();
        item.transform.position = dropOffLocations[index].position;
        droppedOffItems[index] = item;
        item.PickedUpCallback = carryable => {
            DetachItem(carryable);
            carryable.PickedUpCallback = null;
        };
        return true;
    }

    public PlantType[] TakeAllIngredients() {
        var result = new List<PlantType>();
        foreach (var item in droppedOffItems) {
            if (item is null) {
                continue;
            }
            var plant = item.GetComponent<Plant>();
            if (plant is null) {
                Debug.LogError("This carryable doesn't seem to be a plant");
                return null;
            }
            result.Add(plant.Type);
            GameObject.Destroy(item.gameObject);
        }
        droppedOffItems = new Carryable[dropOffLocations.Length];
        return result.ToArray();
    }
}