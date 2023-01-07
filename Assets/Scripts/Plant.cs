using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : TaskTarget {
    private float _growth = 0f;
    public float GrowthRate { get; set; } = 0.1f;
    private const float MinScaling = 0.2f;

    private void Update() {
        _growth = Mathf.Min(_growth + GrowthRate * Time.deltaTime, 1f);
        transform.localScale = Vector3.one * Mathf.Lerp(MinScaling, 1f, _growth);
    }

    public bool IsFullyGrown() {
        return _growth >= 1f;
    }
}