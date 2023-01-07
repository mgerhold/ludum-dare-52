using UnityEngine;

namespace Tasks {
    public class DeliverDish : Task {
        private DishLocationInfo _dishLocation;
        private bool _hasDelivered = false;

        public DeliverDish(Meeple meeple, DishLocationInfo dishLocation) : base(meeple) {
            _dishLocation = dishLocation;
        }

        public override void Execute() {
            Debug.Log("Delivering Dish");
            Debug.Assert(meeple.CurrentItem() is not null);
            Debug.Assert(meeple.CurrentItem() is Dish);
            var dish = meeple.DetachCurrentItem().GetComponent<Dish>();
            _dishLocation.counter.DeliverDish(_dishLocation, dish);
            _hasDelivered = true;
        }

        public override bool HasFinished() {
            return _hasDelivered;
        }
    }
}