using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardAligner : MonoBehaviour {
    void Update() {
        transform.LookAt(Camera.main.transform.position);
    }
}