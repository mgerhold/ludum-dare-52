using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tasks;
using UnityEngine;

public class Meeple : MonoBehaviour {
    private readonly Queue<Task> _tasks = new();
    private Task _currentTask = null;
    private Carryable _carriedItem = null;
    [SerializeField] private Transform _itemTransform = null;

    public void EnqueueTask(Task task) {
        _tasks.Enqueue(task);
        TryStartNextTask();
    }

    public void PickupItem(Carryable carryable) {
        if (_carriedItem != null) {
            DropCurrentItem();
        }
        _carriedItem = carryable;
        carryable.transform.position = _itemTransform.position;
        carryable.transform.parent = _itemTransform;
    }

    public void DropCurrentItem() {
        if (_carriedItem != null) {
            _carriedItem.transform.parent = null;
            var oldPosition = _carriedItem.transform.position;
            oldPosition.y = 0f;
            _carriedItem.transform.position = oldPosition;
        }
    }
    
    private void TryStartNextTask() {
        if (_currentTask == null && _tasks.Any()) {
            _currentTask = _tasks.Dequeue();
            _currentTask.Execute();
            Debug.Log("Started next task");
        } else {
            Debug.Log("No task to execute");
        }
    }

    private void Update() {
        if (_currentTask != null && _currentTask.HasFinished()) {
            _currentTask = null;
            TryStartNextTask();
        }
    }
}