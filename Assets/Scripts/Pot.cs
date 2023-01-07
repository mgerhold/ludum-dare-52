using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : TaskTarget {
    public IngredientDropOff ingredientDropOff = null;
    [SerializeField] private Transform dishSpawnLocation = null;
    private bool _spawnLocationOccupied = false;

    public bool CanCook() {
        return !_spawnLocationOccupied;
    }

    public void Cook(PlantType[] ingredients) {
        Debug.Assert(CanCook());
        Debug.Assert(ingredients is not null && ingredients.Length > 0);
        // cook
        var prefab =
            PrefabManager.Instance.dishPrefabs[
                UnityEngine.Random.Range(0, PrefabManager.Instance.dishPrefabs.Length)];
        var dish = GameObject.Instantiate(prefab, dishSpawnLocation.position, Quaternion.identity)
            .GetComponent<Dish>();
        dish.ingredients = ingredients;
        _spawnLocationOccupied = true;
        dish.PickedUpCallback = carryable => {
            _spawnLocationOccupied = false;
            carryable.PickedUpCallback = null;
        };
    }
}