using System;
using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour {
    private const float GotoDistanceThreshold = 0.5f;
    private const float MaxNavMeshOffset = 5.0f;
    private Meeple _selection = null;
    private InputMode _mode = InputMode.Normal;
    private GameObject tilledGroundPreview = null;

    private T ScriptByRaycast<T>(out RaycastHit hit) where T : MonoBehaviour {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Debug.Assert(hit.collider.gameObject.GetComponentsInParent<T>().Length <= 1);
            return hit.collider.gameObject.GetComponentInParent<T>();
        }
        return null;
    }

    private T[] ScriptsByRaycast<T>(out RaycastHit hit) where T : MonoBehaviour {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            return hit.collider.gameObject.GetComponentsInParent<T>();
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

    private bool HasSelection() => _selection is not null;

    private InputMode DetermineInputMode() {
        if (!HasSelection()) {
            return InputMode.Normal;
        }
        var item = _selection.CurrentItem();
        if (item is null) {
            return InputMode.Normal;
        }
        if (item is Hoe) {
            return InputMode.Tilling;
        }
        if (item is Seeds) {
            return InputMode.Seeding;
        }
        if (item.GetComponent<Plant>() is not null) {
            return InputMode.IngredientTransporting;
        }
        // todo: watering can
        return InputMode.Normal;
    }

    private void TryDestroyTilledGroundPreview() {
        if (tilledGroundPreview is not null) {
            GameObject.Destroy(tilledGroundPreview);
            tilledGroundPreview = null;
        }
    }

    void Update() {
        HandleTillingPreview();

        HandleSelection();

        if (HasSelection() && Input.GetMouseButtonDown(1)) {
            switch (_mode) {
                case InputMode.Normal:
                    HandleMovementAndPickup();
                    break;
                case InputMode.Tilling:
                    if (tilledGroundPreview is null) {
                        // preview is invisible => normal movement
                        HandleMovementAndPickup();
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
                    }
                    break;
                case InputMode.Seeding:
                    bool issuedPlantErrand = false;
                    var tilledGround = ScriptByRaycast<TilledGround>(out _);
                    if (tilledGround is not null) {
                        if (tilledGround.Plant is null) {
                            if (_selection.CurrentItem() is Seeds seeds) {
                                bool canSeedHere = true;
                                foreach (var meeple in GameManager.Instance.Meeples) {
                                    foreach (var task in meeple.GetTasksOfType<PlantErrand>()) {
                                        if (task.TargetGround == tilledGround) {
                                            canSeedHere = false;
                                            break;
                                        }
                                    }
                                    if (!canSeedHere) {
                                        break;
                                    }
                                }

                                if (!canSeedHere) {
                                    // todo: show error message
                                    Debug.Log("This tilled ground is already queued to be planted");
                                } else {
                                    var plantType = seeds.plantType;
                                    Debug.Log("Adding Task to place down seeds");
                                    // 1. walk to tilled ground tile
                                    _selection.EnqueueTask(new Goto(_selection,
                                        tilledGround.transform.position,
                                        GotoDistanceThreshold));
                                    // 2. plant the plant
                                    _selection.EnqueueTask(new PlantErrand(_selection, tilledGround,
                                        plantType));
                                    issuedPlantErrand = true;
                                }
                            } else {
                                Debug.LogError("Meeple does not carry seeds");
                                return;
                            }
                        } else {
                            // todo: show error message - tilled ground already occupied
                        }
                    }
                    if (!issuedPlantErrand) {
                        HandleMovementAndPickup();
                    }
                    break;
                case InputMode.IngredientTransporting:
                    var ingredientDropOff = ScriptByRaycast<IngredientDropOff>(out _);
                    if (ingredientDropOff is not null) {
                        Debug.Log("Trying to take ingredients to a drop off");
                        // 1. walk to ingredient drop off
                        _selection.EnqueueTask(new Goto(_selection,
                            GetValidTargetPosition(ingredientDropOff.transform.position),
                            GotoDistanceThreshold));
                        // 2. drop off ingredients
                        _selection.EnqueueTask(new DropOff(_selection, ingredientDropOff));
                    } else {
                        HandleMovementAndPickup();
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

    private void HandleSelection() {
        if (Input.GetMouseButtonDown(0)) {
            var meeple = ScriptByRaycast<Meeple>(out _);
            if (meeple != null) {
                _selection = meeple;
                Debug.Log("Selected");
            } else {
                _selection = null;
                Debug.Log("Cleared selection");
            }
        }
    }

    private void HandleTillingPreview() {
        bool shouldShowTilledGroundPreview = false;
        if (_mode == InputMode.Tilling) {
            var ground = ScriptByRaycast<Soil>(out var hit);
            if (ground is not null) {
                shouldShowTilledGroundPreview = true;
                var gridPosition = new Vector3Int(Mathf.RoundToInt(hit.point.x), 0,
                    Mathf.RoundToInt(hit.point.z));
                foreach (var meeple in GameManager.Instance.Meeples) {
                    foreach (var task in meeple.GetTasksOfType<TillGround>()) {
                        if (task.TargetPosition == gridPosition) {
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
    }

    private void HandleMovementAndPickup() {
        var taskTargets = ScriptsByRaycast<TaskTarget>(out var hit);
        foreach (var taskTarget in taskTargets) {
            if (taskTarget is Ground) {
                _selection.EnqueueTask(new Goto(_selection, GetValidTargetPosition(hit.point),
                    GotoDistanceThreshold));
                Debug.Log("Moving so a point on the ground");
                break;
            }
            if (taskTarget is Carryable carryable) {
                Debug.Log("Trying to pickup object");
                // first walk to the target
                _selection.EnqueueTask(new Goto(_selection,
                    GetValidTargetPosition(taskTarget.transform.position),
                    GotoDistanceThreshold));
                // pickup item
                _selection.EnqueueTask(new Pickup(_selection, carryable));
                break;
            }
        }
    }
}