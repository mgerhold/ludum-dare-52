using UnityEngine;

namespace Tasks {
    public class Cook : Task {
        private Pot _pot;
        private bool _hasTriedToCook = false;

        public Cook(Meeple meeple, Pot pot) : base(meeple) {
            _pot = pot;
        }

        public override void Execute() {
            _hasTriedToCook = true;
            if (!_pot.CanCook()) {
                // todo: show error message
                Debug.LogError("spawn location is blocked");
                return;
            }
            var dropOff = _pot.ingredientDropOff;
            if (dropOff.IsEmpty()) {
                // todo: show error message
                Debug.LogError("cannot cook without ingredients");
                return;
            }
            var ingredients = dropOff.TakeAllIngredients();
            _pot.Cook(ingredients);
        }

        public override bool HasFinished() {
            return _hasTriedToCook;
        }
    }
}