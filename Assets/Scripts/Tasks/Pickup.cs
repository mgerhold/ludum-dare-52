namespace Tasks {
    public class Pickup : Task {
        private readonly Carryable _target;
        private bool _wasExecuted = false;
        
        public Pickup(Meeple meeple, Carryable target) : base(meeple) {
            _target = target;
        }

        public override void Execute() {
            meeple.PickupItem(_target);
            _wasExecuted = true;
        }

        public override bool HasFinished() {
            return _wasExecuted;
        }
    }
}