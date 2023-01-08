namespace Tasks {
    public class Pickup : Task {
        private readonly Carryable _target;
        private bool _wasExecuted = false;

        public Pickup(Meeple meeple, Carryable target) : base(meeple) {
            _target = target;
        }

        public override void Execute() {
            _target.OnPickedUp();
            meeple.PickupItem(_target);
            _wasExecuted = true;
            TutorialManager.Instance.OnPickedUpItem(_target);
        }

        public override bool HasFinished() {
            return _wasExecuted;
        }
    }
}