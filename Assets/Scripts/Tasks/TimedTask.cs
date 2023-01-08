using UnityEngine;

namespace Tasks {
    public abstract class TimedTask : Task {
        private bool _hasStarted = false;
        private float _startTime = 0f;
        private bool _hasFinished = false;
        private ProgressBar _progressBar = null;

        protected TimedTask(Meeple meeple) : base(meeple) {
            _progressBar = meeple.GetComponentInChildren<ProgressBar>();
        }

        protected abstract float TaskDuration();

        protected virtual void OnExecution() { }

        protected abstract void OnFinish();

        protected float GetProgress() {
            return Mathf.Clamp((Time.time - _startTime) / TaskDuration(), 0f, 1f);
        }

        private void UpdateProgressBar() {
            if (_progressBar is null) {
                Debug.LogError("no progress bar found");
                return;
            }
            _progressBar.Value = GetProgress();
        }

        private void SetProgressBarVisibility(bool visible) {
            if (_progressBar is null) {
                Debug.LogError("no progress bar found");
                return;
            }
            _progressBar.Visible = visible;
        }

        protected virtual bool CanStartExecution() {
            return true;
        }

        protected virtual void OnFailedStart() { }

        protected virtual void OnStart() { }

        public sealed override void Execute() {
            if (HasFinished()) {
                return;
            }
            if (!_hasStarted) {
                if (CanStartExecution()) {
                    _hasStarted = true;
                    _startTime = Time.time;
                    SetProgressBarVisibility(true);
                    OnStart();
                } else {
                    _hasFinished = true;
                    OnFailedStart();
                    return;
                }
            }
            UpdateProgressBar();
            OnExecution();
            if (Time.time >= _startTime + TaskDuration()) {
                _hasFinished = true;
                OnFinish();
                SetProgressBarVisibility(false);
            }
        }

        public sealed override bool HasFinished() {
            return _hasFinished;
        }
    }
}