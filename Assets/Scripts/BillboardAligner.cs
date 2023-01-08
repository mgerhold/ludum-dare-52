using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardAligner : MonoBehaviour {
    void Update() {
        var cameraForward = Camera.main.transform.forward;
        transform.LookAt(transform.position - cameraForward);
    }
}