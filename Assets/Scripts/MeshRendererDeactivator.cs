using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererDeactivator : MonoBehaviour {
    private void Awake() {
        foreach (var component in GetComponentsInChildren<MeshRenderer>()) {
            component.enabled = false;
        }
    }
}