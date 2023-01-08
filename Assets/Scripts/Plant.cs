using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Plant : TaskTarget {
    [SerializeField] private GrowthDurationsScriptableObject growthDurations = null;

    public PlantType type = PlantType.Invalid;

    private float _growth = 0f;
    private const float MinScaling = 0.2f;
    private ProgressBar _progressBar = null;

    private void Awake() {
        _progressBar = GetComponentInChildren<ProgressBar>();
    }

    private void Start() {
        foreach (var plantCollider in GetComponentsInChildren<Collider>(true)) {
            plantCollider.enabled = false;
        }
        _progressBar.Visible = true;
    }

    private void Update() {
        _growth = Mathf.Min(_growth + GrowthRate() * Time.deltaTime, 1f);
        transform.localScale = Vector3.one * Mathf.Lerp(MinScaling, 1f, _growth);
        _progressBar.Value = _growth;
        if (_growth >= 1f) {
            _progressBar.Visible = false;
        }
    }

    private float GrowthRate() => 1f / growthDurations.durations[type];

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