using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
    [SerializeField] private float movementSpeed = 5f;
    
    void Update() {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var movement = new Vector3(-x, 0f, -y);
        transform.position = transform.position + movement * Time.deltaTime * movementSpeed;
    }
}
