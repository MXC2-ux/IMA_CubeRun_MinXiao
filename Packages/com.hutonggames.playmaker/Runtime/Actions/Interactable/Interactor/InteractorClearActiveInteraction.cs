using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [MovedFrom(true, null, null, "InteractionControllerClearActiveInteraction")]
    [ActionCategory(Category.Interactor)]
    [ConvertibleGroup("Interactable")]
    [ActionDescription("Forgets the last activated Interactable so it can be activated again on the next valid input. The current selection and hover are unchanged.")]
    public sealed class InteractorClearActiveInteraction : BaseAction
    {
        [Tooltip("The Interactor that should forget its last activated Interactable, allowing it to activate again on valid input.")]
        [SerializeField, DisplayName("Interactor")]
        private InteractorVar _interactionController;

        public override bool CanExecute() => CheckParameters(_interactionController);

        public override void Execute()
        {
            var controller = _interactionController.Value;
            if (!controller)
            {
                return;
            }

            controller.ClearActiveInteraction();
        }

        public override string GetSummary() =>
            "Clear active interaction on {_interactionController}";
    }
}
