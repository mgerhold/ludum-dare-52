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
                var timeUntilSpawn = orders[i].spawnTime - Time.time;
                var hasSpawned = timeUntilSpawn < 0f;
                var waitTime = Time.time - orders[i].spawnTime;
                if (hasSpawned) {
                    var remainingTime = orders[i].maxWaitTime - waitTime;
                    var littleTimeRemaining = (remainingTime <= orders[i].maxWaitTime / 2f);
                    orderQueueTexts[i].color = littleTimeRemaining ? Color.red : Color.yellow;
                    orderQueueTexts[i].text = $"{ingredientList} ({remainingTime:f1} s remaining)";
                } else {
                    orderQueueTexts[i].color = Color.green;
                    orderQueueTexts[i].text =
                        $"{ingredientList} ({Math.Max(0f, orders[i].spawnTime - Time.time):f1} s)";
                }
            } else {
                orderQueueTexts[i].text = "";
            }
        }
    }
}