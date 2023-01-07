using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : TaskTarget {
    public Action<Carryable> PickedUpCallback { get; set; } = null;

    public void OnPickedUp() {
        PickedUpCallback?.Invoke(this);
    }
}