using UnityEngine;

namespace Tasks {
    public class DropOff : Task {
        private readonly IngredientDropOff _dropOffTarget = null;
        private bool _hasTriedToDropOff = false;

        public DropOff(Meeple meeple, IngredientDropOff dropOffTarget) : base(meeple) {
            _dropOffTarget = dropOffTarget;
        }

        public override void Execute() {
            _hasTriedToDropOff = true;
            if (!_dropOffTarget.DropOff(meeple.CurrentItem())) {
                Debug.LogWarning("Unable to drop off item");
                return;
            }
            Debug.Log("Dropped off item");
            meeple.DetachCurrentItem();
        }

        public override bool HasFinished() {
            return _hasTriedToDropOff;
        }
    }
}