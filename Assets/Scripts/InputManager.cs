using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour {
    [SerializeField] private GameObject tilledGroundPrefab = null;
    private const float GotoDistanceThreshold = 0.5f;
    private const float MaxNavMeshOffset = 1.0f;
    private Meeple _selection = null;
    private InputMode _mode = InputMode.Normal;
    private GameObject tilledGroundPreview = null;

    private T ScriptByRaycast<T>(out RaycastHit hit) where T : MonoBehaviour {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            return hit.collider.gameObject.GetComponentInParent<T>();
        }
        return null;
    }

    private Vector3 GetValidTargetPosition(Vector3 maybeInvalidTargetPosition) {
        var positionOnGround = maybeInvalidTargetPosition;
        positionOnGround.y = 0f;
        if (NavMesh.SamplePosition(positionOnGround, out var hit, MaxNavMeshOffset, NavMesh.AllAreas)) {
            return hit.position;
        }
        Debug.Assert(false, "unreachable");
        return default;
    }

    private bool HasSelection() => _selection != null;

    private InputMode DetermineInputMode() {
        if (!HasSelection()) {
            return InputMode.Normal;
        }
        var item = _selection.CurrentItem();
        if (item == null) {
            return InputMode.Normal;
        }
        if (item is Hoe) {
            return InputMode.Tilling;
        }
        // todo: watering can
        return InputMode.Normal;
    }

    void Update() {
        bool shouldShowTilledGroundPreview = false;
        if (_mode == InputMode.Tilling) {
            var ground = ScriptByRaycast<Soil>(out var hit);
            if (ground != null) {
                shouldShowTilledGroundPreview = true;
                if (tilledGroundPreview == null) {
                    tilledGroundPreview = GameObject.Instantiate(tilledGroundPrefab, hit.point, Quaternion.identity);
                    var colliders = tilledGroundPreview.GetComponentsInChildren<Collider>();
                    foreach (var collider in colliders) {
                        GameObject.Destroy(collider);
                    }
                }
                var gridPosition = new Vector3(Mathf.Round(hit.point.x + 0.5f) - 0.5f, 0f,
                    Mathf.Round(hit.point.z + 0.5f) - 0.5f);
                tilledGroundPreview.transform.position = gridPosition;
            }
        }

        if (!shouldShowTilledGroundPreview && tilledGroundPreview != null) {
            GameObject.Destroy(tilledGroundPreview);
            tilledGroundPreview = null;
        }

        if (Input.GetMouseButtonDown(0)) {
            var meeple = ScriptByRaycast<Meeple>(out _);
            if (meeple != null) {
                _selection = meeple;
                Debug.Log("Selected");
            } else {
                _selection = null;
                Debug.Log("Cleared selection");
            }
        } else if (HasSelection() && Input.GetMouseButtonDown(1)) {
            var taskTarget = ScriptByRaycast<TaskTarget>(out var hit);
            if (taskTarget != null) {
                if (taskTarget is Ground) {
                    _selection.EnqueueTask(new Goto(_selection, GetValidTargetPosition(hit.point),
                        GotoDistanceThreshold));
                    Debug.Log("Moving so a point on the ground");
                } else if (taskTarget is Carryable carryable) {
                    Debug.Log("Trying to pickup object");
                    // first walk to the target
                    _selection.EnqueueTask(new Goto(_selection, GetValidTargetPosition(taskTarget.transform.position),
                        GotoDistanceThreshold));
                    // pickup item
                    _selection.EnqueueTask(new Pickup(_selection, carryable));
                }
            }
        }

        var newInputMode = DetermineInputMode();
        if (newInputMode != _mode) {
            Debug.Log($"Changing mode from {_mode} to {newInputMode}");
        }
        _mode = newInputMode;
    }
}