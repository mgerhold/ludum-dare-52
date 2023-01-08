using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Order {
    public float spawnTime;
    public List<PlantType> ingredients;
    public float maxWaitTime = 0f;
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