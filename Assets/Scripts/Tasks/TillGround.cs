using UnityEngine;

namespace Tasks {
    public class TillGround : Task {
        public Vector3Int TargetPosition { get; }
        private bool _hasExecuted = false;

        public TillGround(Meeple meeple, Vector3Int targetPosition) : base(meeple) {
            TargetPosition = targetPosition;
        }

        public override void Execute() {
            GameObject.Instantiate(PrefabManager.Instance.tilledGroundPrefab, TargetPosition,
                Quaternion.identity);
            _hasExecuted = true;
        }

        public override bool HasFinished() {
            return _hasExecuted;
        }
    }
}