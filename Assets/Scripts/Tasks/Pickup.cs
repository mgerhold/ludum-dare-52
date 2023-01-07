namespace Tasks {
    public class Pickup : Task {
        private Carryable _target;
        public Pickup(Meeple meeple, Carryable target) : base(meeple) {
            _target = target;
        }

        public override void Execute() {
            throw new System.NotImplementedException();
        }

        public override bool HasFinished() {
            throw new System.NotImplementedException();
        }
    }
}