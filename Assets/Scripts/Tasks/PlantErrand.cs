using UnityEngine;

namespace Tasks {
    public class PlantErrand : Task {
        public TilledGround TargetGround { get; private set; }
        public PlantType PlantType { get; private set; }

        private bool _hasExecuted = false;

        public PlantErrand(Meeple meeple, TilledGround targetGround, PlantType plantType) : base(meeple) {
            TargetGround = targetGround;
            PlantType = plantType;
        }

        public override void Execute() {
            // destroy the seeds
            if (meeple.CurrentItem() is not Seeds) {
                Debug.LogError("Meeple does not carry seeds");
                return;
            }
            meeple.DestroyCurrentItem();

            // place down the plant
            var plantObject = GameObject.Instantiate(PrefabManager.Instance.plantPrefabs[PlantType],
                TargetGround.transform.position, Quaternion.identity);
            TargetGround.Plant = plantObject.GetComponentInChildren<Plant>();
            Debug.Assert(TargetGround.Plant is not null);
            _hasExecuted = true;
        }

        public override bool HasFinished() {
            return _hasExecuted;
        }
    }
}