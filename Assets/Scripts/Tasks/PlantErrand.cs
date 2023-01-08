using UnityEngine;

namespace Tasks {
    public class PlantErrand : TimedTask {
        public TilledGround TargetGround { get; private set; }
        public PlantType PlantType { get; private set; }

        public PlantErrand(Meeple meeple, TilledGround targetGround, PlantType plantType) : base(meeple) {
            TargetGround = targetGround;
            PlantType = plantType;
        }

        protected override float TaskDuration() {
            return 0.5f;
        }

        protected override void OnStart() {
            // destroy the seeds
            if (meeple.CurrentItem() is not Seeds) {
                Debug.LogError("Meeple does not carry seeds");
                return;
            }
            meeple.DestroyCurrentItem();
        }

        protected override void OnFinish() {
            // place down the plant
            TargetGround.PlantSeed(PlantType);
        }
    }
}