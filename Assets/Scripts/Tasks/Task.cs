namespace Tasks {
    public abstract class Task {
        protected readonly Meeple meeple;

        protected Task(Meeple meeple) {
            this.meeple = meeple;
        }
        
        public abstract void Execute();
        public abstract bool HasFinished();
    }
}