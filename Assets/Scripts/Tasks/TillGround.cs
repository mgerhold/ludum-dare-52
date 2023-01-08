using UnityEngine;

namespace Tasks {
    public class TillGround : TimedTask {
        protected override float TaskDuration() {
            return 3f;
        }

        protected override void OnFinish() {
            GameObject.Instantiate(PrefabManager.Instance.tilledGroundPrefab, TargetPosition,
                Quaternion.identity);
        }

        public Vector3Int TargetPosition { get; }

        public TillGround(Meeple meeple, Vector3Int targetPosition) : base(meeple) {
            TargetPosition = targetPosition;
        }
    }
}