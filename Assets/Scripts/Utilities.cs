using UnityEngine;
using UnityEngine.AI;

public class Utilities {
    private const float MaxNavMeshOffset = 5.0f;

    public static Vector3 GetValidTargetPosition(Vector3 maybeInvalidTargetPosition) {
        var positionOnGround = maybeInvalidTargetPosition;
        positionOnGround.y = 0f;
        if (NavMesh.SamplePosition(positionOnGround, out var hit, MaxNavMeshOffset, NavMesh.AllAreas)) {
            return hit.position;
        }
        Debug.Assert(false, "unreachable");
        return default;
    }
}