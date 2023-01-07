using System.Collections;
using System.Collections.Generic;
using Tasks;
using UnityEngine;
using UnityEngine.AI;

public class InputManager : MonoBehaviour {
    private Meeple _selection = null;
    private const float GotoDistanceThreshold = 0.5f;
    private const float MaxNavMeshOffset = 1.0f;

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

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var meeple = ScriptByRaycast<Meeple>(out _);
            if (meeple != null) {
                _selection = meeple;
                Debug.Log("Selected");
            } else {
                _selection = null;
                Debug.Log("Cleared selection");
            }
        } else if (_selection != null && Input.GetMouseButtonDown(1)) {
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
    }
}