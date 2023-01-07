using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour {
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

    private void TryDestroyTilledGroundPreview() {
        if (tilledGroundPreview != null) {
            GameObject.Destroy(tilledGroundPreview);
            tilledGroundPreview = null;
        }
    }

    void Update() {
        bool shouldShowTilledGroundPreview = false;
        if (_mode == InputMode.Tilling) {
            var ground = ScriptByRaycast<Soil>(out var hit);
            if (ground != null) {
                shouldShowTilledGroundPreview = true;
                var gridPosition = new Vector3Int(Mathf.RoundToInt(hit.point.x), 0,
                    Mathf.RoundToInt(hit.point.z));
                foreach (var meeple in GameManager.Instance.Meeples) {
                    foreach (var position in meeple.EnqueuedTillingPositions()) {
                        if (position == gridPosition) {
                            shouldShowTilledGroundPreview = false;
                            break;
                        }
                    }
                    if (!shouldShowTilledGroundPreview) {
                        break;
                    }
                }

                if (shouldShowTilledGroundPreview) {
                    if (tilledGroundPreview == null) {
                        tilledGroundPreview = GameObject.Instantiate(
                            PrefabManager.Instance.tilledGroundPrefab,
                            hit.point,
                            Quaternion.identity);
                        var colliders = tilledGroundPreview.GetComponentsInChildren<Collider>();
                        foreach (var collider in colliders) {
                            GameObject.Destroy(collider);
                        }
                    }
                    tilledGroundPreview.transform.position = gridPosition;
                }
            }
        }

        if (!shouldShowTilledGroundPreview) {
            TryDestroyTilledGroundPreview();
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
            switch (_mode) {
                case InputMode.Normal:
                    HandleMovementAndPickupInput();
                    break;
                case InputMode.Tilling:
                    if (tilledGroundPreview == null) {
                        // preview is invisible => normal movement
                        HandleMovementAndPickupInput();
                    } else {
                        // place down tilled ground
                        // 1. cache position and destroy preview
                        var position = tilledGroundPreview.transform.position;
                        TryDestroyTilledGroundPreview();

                        // 2. move to the location
                        _selection.EnqueueTask(new Goto(_selection, position, GotoDistanceThreshold));

                        // 3. till ground
                        _selection.EnqueueTask(new TillGround(_selection,
                            Vector3Int.RoundToInt(position)
                        ));

                        Debug.Log($"Creating task for tilling as positio {Vector3Int.RoundToInt(position)}");
                    }
                    break;
            }
        }

        var newInputMode = DetermineInputMode();
        if (newInputMode != _mode) {
            Debug.Log($"Changing mode from {_mode} to {newInputMode}");
        }
        _mode = newInputMode;
    }

    private void HandleMovementAndPickupInput() {
        var taskTarget = ScriptByRaycast<TaskTarget>(out var hit);
        if (taskTarget != null) {
            if (taskTarget is Ground) {
                _selection.EnqueueTask(new Goto(_selection, GetValidTargetPosition(hit.point),
                    GotoDistanceThreshold));
                Debug.Log("Moving so a point on the ground");
            } else if (taskTarget is Carryable carryable) {
                Debug.Log("Trying to pickup object");
                // first walk to the target
                _selection.EnqueueTask(new Goto(_selection,
                    GetValidTargetPosition(taskTarget.transform.position),
                    GotoDistanceThreshold));
                // pickup item
                _selection.EnqueueTask(new Pickup(_selection, carryable));
            }
        }
    }
}