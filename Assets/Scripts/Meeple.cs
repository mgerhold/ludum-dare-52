using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tasks;
using UnityEngine;

public class Meeple : MonoBehaviour {
    private readonly Queue<Task> _tasks = new();
    private Task _currentTask = null;

    public void EnqueueTask(Task task) {
        _tasks.Enqueue(task);
        TryStartNextTask();
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