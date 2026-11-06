using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [MovedFrom(true, null, null, "InteractionControllerSetSearchRadius")]
    [ActionCategory(Category.Interactor)]
    [ConvertibleGroup("Interactable")]
    [ActionDescription("Sets how far the Interactor searches from its interaction origin. Interactables are considered when their colliders overlap the area within this radius.")]
    public sealed class InteractorSetSearchRadius : BaseAction
    {
        [Tooltip("The Interactor whose nearby search range you want to change.")]
        [SerializeField, DisplayName("Interactor")]
        private InteractorVar _interactionController;

        [Tooltip("Radius in world units around the interaction origin. Increase it to detect colliders farther away, such as a distant doorway. Set to 0 to disable the search.")]
        [SerializeField]
        private FloatVar _searchRadius;

        public override bool CanExecute() => CheckParameters(_interactionController, _searchRadius);

        public override void Execute()
        {
            var controller = _interactionController.Value;
            if (!controller)
            {
                return;
            }

            controller.SearchRadius = _searchRadius.Value;
        }

        public override string GetSummary() =>
            "Set {_interactionController} Search Radius to {_searchRadius}";
    }
}
