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
    [SerializeField] private GameObject selectionMarker = null;
    [SerializeField] private Transform _itemTransform = null;
    [SerializeField] private AudioClipPlayer dropSoundPlayer = null;
    [SerializeField] private AudioClipPlayer tillingSoundPlayer = null;

    public void PlayTillingSound() {
        tillingSoundPlayer.PlaySound();
    }

    public void ShowSelectionMarker() {
        selectionMarker.SetActive(true);
    }

    public void HideSelectionMarker() {
        selectionMarker.SetActive(false);
    }

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
            dropSoundPlayer.PlaySound();
        }
    }

    public Carryable DetachCurrentItem() {
        Debug.Assert(_currentItem is not null);
        _currentItem.transform.parent = null;
        var result = _currentItem;
        _currentItem = null;
        dropSoundPlayer.PlaySound();
        return result;
    }

    public void DestroyCurrentItem() {
        Debug.Assert(_currentItem is not null);
        GameObject.Destroy(_currentItem.gameObject);
        _currentItem = null;
        dropSoundPlayer.PlaySound();
    }

    public Carryable CurrentItem() {
        return _currentItem;
    }

    public List<T> GetTasksOfType<T>() where T : Task {
        var result = new List<T>();
        foreach (var task in _tasks) {
            if (task is T t) {
                result.Add(t);
            }
        }
        return result;
    }

    private void TryStartNextTask() {
        if (_currentTask is null && _tasks.Any()) {
            _currentTask = _tasks.Dequeue();
        } else {
            Debug.Log($"No task to execute ({_tasks.Count} in queue)");
        }
    }

    private void Update() {
        if (_currentTask is not null) {
            if (_currentTask.HasFinished()) {
                _currentTask = null;
                TryStartNextTask();
            } else {
                _currentTask.Execute();
            }
        }
    }
}