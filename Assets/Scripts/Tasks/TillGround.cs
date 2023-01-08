using UnityEngine;

namespace Tasks {
    public class TillGround : TimedTask {
        protected override float TaskDuration() {
            return 3f;
        }

        protected override void OnStart() {
            meeple.PlayTillingSound();
        }

        protected override void OnFinish() {
            GameObject.Instantiate(PrefabManager.Instance.tilledGroundPrefab, TargetPosition,
                Quaternion.identity);
            TutorialManager.Instance.OnTillingCompleted();
        }

        public Vector3Int TargetPosition { get; }

        public TillGround(Meeple meeple, Vector3Int targetPosition) : base(meeple) {
            TargetPosition = targetPosition;
        }
    }
}