using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [MovedFrom(true, null, null, "InteractionControllerSuppressInteractable")]
    [ActionCategory(Category.Interactor)]
    [ConvertibleGroup("Interactable")]
    [ActionDescription("Prevents this Interactor from selecting an Interactable for the specified duration. For example, after using a lever, suppress it for 1 second so another nearby Interactable can be selected.")]
    public sealed class InteractorSuppressInteractable : BaseAction
    {
        [Tooltip("The Interactor that should temporarily ignore the chosen Interactable. Other Interactors are unaffected.")]
        [SerializeField, DisplayName("Interactor")]
        private InteractorVar _interactionController;

        [Tooltip("The Interactable to exclude from this Interactor's selection, such as a lever just used.")]
        [SerializeField]
        private InteractableVar _interactable;

        [Tooltip("How many seconds to ignore this Interactable. For example, 1 prevents reselection for 1 second. Set to 0 to remove its suppression.")]
        [SerializeField]
        private FloatVar _suppressDuration;

        public override bool CanExecute() => CheckParameters(_interactionController, _interactable, _suppressDuration);

        public override void Execute()
        {
            var controller = _interactionController.Value;
            var interactable = _interactable.Value;
            if (!controller || !interactable)
            {
                return;
            }

            controller.SuppressInteractable(interactable, _suppressDuration.Value);
        }

        public override string GetSummary() =>
            "Suppress {_interactable} on {_interactionController} for {_suppressDuration} seconds";
    }
}
