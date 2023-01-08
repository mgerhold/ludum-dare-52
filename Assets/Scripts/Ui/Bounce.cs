using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bounce : MonoBehaviour {
    [SerializeField] private float amount = 0.7f;
    [SerializeField] private float speed = 1f;
    private Vector3 _startPosition;

    private void Start() {
        _startPosition = transform.position;
    }

    private void Update() {
        transform.position = _startPosition + transform.up * Mathf.Sin(Time.time * speed) * amount;
    }
}
