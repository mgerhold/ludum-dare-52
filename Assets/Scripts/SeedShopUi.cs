using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedShopUi : MonoBehaviour {
    [SerializeField] private GameObject[] seedShopButtons = null;
    [SerializeField] private PricesScriptableObject prices = null;

    void Update() {
        var plantTypes = (PlantType[])Enum.GetValues(typeof(PlantType));
        for (int i = 0; i < seedShopButtons.Length; ++i) {
            if (i >= plantTypes.Length - 1) {
                seedShopButtons[i].SetActive(false);
                continue;
            }
            var price = prices.prices[plantTypes[i]];
            var affordable = !SeedsManager.Instance.IsFull() && MoneyManager.Instance.Money >= price;
            seedShopButtons[i].SetActive(affordable);
            var text = seedShopButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (affordable) {
                text.text = $"buy {plantTypes[i]} seeds ({price} TKM)";
            }
        }
    }
}