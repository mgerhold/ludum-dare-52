using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        GameObject.Destroy(other.gameObject.GetComponentInParent<Customer>().gameObject);
    }
}