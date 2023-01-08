using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilledGround : Ground {
    public Plant Plant { get; private set; }

    public void PlantSeed(PlantType plantType) {
        var plantObject = GameObject.Instantiate(PrefabManager.Instance.plantPrefabs[plantType],
            transform.position, Quaternion.identity);
        Plant = plantObject.GetComponentInChildren<Plant>();
        Debug.Assert(Plant is not null);
        Plant.type = plantType;
    }

    private void Update() {
        if (Plant is not null && Plant.IsFullyGrown() && !Plant.IsCarryable()) {
            Debug.Log("Plant can be harvested");
            var carryable = Plant.MakeCarryable();
            carryable.PickedUpCallback = plant => {
                // plant was harvested
                Debug.Log("Tilled ground is now empty");
                Plant = null;
                plant.PickedUpCallback = null;
                GameObject.Destroy(gameObject);
            };
        }
    }
}