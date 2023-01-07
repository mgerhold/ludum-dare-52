using UnityEngine;
using UnityEngine.AI;

namespace Tasks {
    public class Goto : Task {
        private readonly Vector3 _target;
        private readonly float _distanceThreshold;

        public Goto(Meeple meeple, Vector3 target, float distanceThreshold) : base(meeple) {
            _target = target;
            _distanceThreshold = distanceThreshold;
        }

        public override void Execute() {
            var agent = meeple.GetComponent<NavMeshAgent>();
            Debug.Assert(agent != null, "No Navmesh Agent provided");
            agent.SetDestination(_target);
        }

        public override bool HasFinished() {
            return Vector3.Distance(meeple.transform.position, _target) <= _distanceThreshold;
        }
    }
}