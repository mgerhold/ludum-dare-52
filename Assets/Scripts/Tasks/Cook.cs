using Unity.VisualScripting;
using UnityEngine;

namespace Tasks {
    public class Cook : TimedTask {
        private Pot _pot;
        private PlantType[] _ingredients = null;

        private IngredientDropOff _dropOff;

        public Cook(Meeple meeple, Pot pot) : base(meeple) {
            _pot = pot;
            _dropOff = _pot.ingredientDropOff;
        }

        protected override float TaskDuration() {
            return 6f;
        }

        protected override bool CanStartExecution() {
            if (!_pot.CanCook()) {
                MessagePanelManager.Instance.ShowMessage("Deliver the last dish before cooking a new one!");
                return false;
            }

            if (_dropOff.IsEmpty()) {
                MessagePanelManager.Instance.ShowMessage("You cannot cook without ingredients!");
                return false;
            }

            return true;
        }

        protected override void OnStart() {
            // only take the ingredients out of the drop off
            _ingredients = _dropOff.TakeAllIngredients();
            _pot.PlayCookingSound();
        }

        protected override void OnFinish() {
            _pot.Cook(_ingredients);
            TutorialManager.Instance.OnCookingFinished(_pot);
        }
    }
}