using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OrderQueueUi : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI[] orderQueueTexts = null;

    private void Update() {
        var orders = OrderQueueManager.Instance.Orders;
        for (int i = 0; i < orderQueueTexts.Length; ++i) {
            if (i < orders.Count) {
                var ingredientList = string.Join(", ",
                    orders[i].ingredients.Select(ingredient => ingredient.ToString()));
                orderQueueTexts[i].text =
                    $"{ingredientList} ({Math.Max(0f, orders[i].spawnTime - Time.time):f1} s)";
            } else {
                orderQueueTexts[i].text = "";
            }
        }
    }
}