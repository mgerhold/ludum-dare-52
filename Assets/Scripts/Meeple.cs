using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tasks;
using UnityEngine;

public class Meeple : MonoBehaviour {
    private readonly Queue<Task> _tasks = new();
    private Task _currentTask = null;
    private Carryable _currentItem = null;
    [SerializeField] private Transform _itemTransform = null;

    public void EnqueueTask(Task task) {
        _tasks.Enqueue(task);
        TryStartNextTask();
    }

    public void PickupItem(Carryable carryable) {
        if (_currentItem != null) {
            DropCurrentItem();
        }
        _currentItem = carryable;
        carryable.transform.position = _itemTransform.position;
        carryable.transform.parent = _itemTransform;
    }

    public void DropCurrentItem() {
        if (_currentItem is not null) {
            _currentItem.transform.parent = null;
            var oldPosition = _currentItem.transform.position;
            oldPosition.y = 0f;
            _currentItem.transform.position = oldPosition;
        }
    }

    public void DestroyCurrentItem() {
        Debug.Assert(_currentItem is not null);
        GameObject.Destroy(_currentItem.gameObject);
        _currentItem = null;
    }

    public Carryable CurrentItem() {
        return _currentItem;
    }

    public List<T> GetTasksOfType<T>() where T: Task {
        var result = new List<T>();
        foreach (var task in _tasks) {
            if (task is T t) {
                result.Add(t);
            }
        }
        return result;
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