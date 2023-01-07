using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Plant : TaskTarget {
    public float growthRate = 0.5f;
    public PlantType type = PlantType.Invalid;
    
    private float _growth = 0f;
    private const float MinScaling = 0.2f;

    private void Start() {
        foreach (var plantCollider in GetComponentsInChildren<Collider>(true)) {
            plantCollider.enabled = false;
        }
    }

    private void Update() {
        _growth = Mathf.Min(_growth + growthRate * Time.deltaTime, 1f);
        transform.localScale = Vector3.one * Mathf.Lerp(MinScaling, 1f, _growth);
    }

    public bool IsCarryable() {
        return GetComponent<Carryable>() is not null;
    }

    public Carryable MakeCarryable() {
        var result = gameObject.AddComponent<Carryable>();
        foreach (var plantCollider in GetComponentsInChildren<Collider>(true)) {
            plantCollider.enabled = true;
        }
        return result;
    }

    public bool IsFullyGrown() {
        return _growth >= 1f;
    }
}