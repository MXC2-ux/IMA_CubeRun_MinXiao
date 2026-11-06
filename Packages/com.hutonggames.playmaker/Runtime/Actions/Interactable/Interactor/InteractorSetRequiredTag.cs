using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [MovedFrom(true, null, null, "InteractionControllerSetRequiredTag")]
    [ActionCategory(Category.Interactor)]
    [ConvertibleGroup("Interactable")]
    [ActionDescription("Sets the Interactor's required tag. Interactables are only considered if their target GameObject has this tag. Leave it empty to allow any tag.")]
    public sealed class InteractorSetRequiredTag : BaseAction
    {
        [Tooltip("The Interactor whose nearby Interactables you want to filter by tag.")]
        [SerializeField, DisplayName("Interactor")]
        private InteractorVar _interactionController;

        [TagValue, CanBeNullOrEmpty]
        [Tooltip("Only consider Interactables whose target GameObject has this tag. For example, 'Usable' ignores targets with other tags. Leave empty to accept any tag.")]
        [SerializeField]
        private StringVar _requiredTag;

        public override bool CanExecute() => CheckParameters(_interactionController, _requiredTag);

        public override void Execute()
        {
            var controller = _interactionController.Value;
            if (!controller)
            {
                return;
            }

            controller.RequiredTag = _requiredTag.Value;
        }

        public override string GetSummary() =>
            "Set {_interactionController} Required Tag to {_requiredTag}";
    }
}
