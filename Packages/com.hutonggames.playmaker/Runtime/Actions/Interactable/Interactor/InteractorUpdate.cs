using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [MovedFrom(true, null, null, "InteractionControllerActivationMode")]
    public enum InteractorActivationMode
    {
        None,
        Manual,
        MouseButtonDown,
        MouseButton
    }

    [Serializable]
    [PublicAPI]
    [MovedFrom(true, null, null, "InteractionControllerUpdate")]
    [ActionCategory(Category.Interactor)]
    [ConvertibleGroup("Interactable")]
    [ActionDescription("Updates an Interactor, evaluating all nearby Interactable components.")]
    public sealed class InteractorUpdate : BaseAction
    {
        private Interactor _controller;

        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;

        [Tooltip("The Interactor that searches for nearby Interactables and handles activation.")]
        [SerializeField, DisplayName("Interactor")]
        private InteractorVar _interactionController;

        [Tooltip("Choose how explicit interactions receive input: Manual uses Interact Pressed, and the mouse modes read the selected mouse button. None supplies no activation press.")]
        [SerializeField, DefaultValue(InteractorActivationMode.None)]
        private InteractorActivationMode _activationMode;

        [OptionalField, HideIf(nameof(HideInteractPressed))]
        [Tooltip("When using Manual activation, set this to true while the player presses the interaction control, such as a Use key.")]
        [SerializeField, DefaultValue(false)]
        private BoolRef _interactPressed;

        [HideIf(nameof(HideMouseButton))]
        [Tooltip("Mouse button used for activation: 0 = left, 1 = right, 2 = middle. Mouse Button Down reacts on the press frame; Mouse Button stays active while held.")]
        [SerializeField, DefaultValue(0)]
        private IntegerVar _mouseButton;

        [OptionalField, HideIf(nameof(HideInteractionId))]
        [Tooltip("Filters explicit interaction candidates by Activation ID. An ID must match; an Interactable with no ID accepts any value. For example, use 'AltUse' for an Interactable marked 'AltUse'.")]
        [SerializeField]
        private StringVar _interactActivationId;

        [ActionHeader("Outputs")]

        [OptionalField]
        [Tooltip("True if an Interactable passes the current activation checks. Explicit interactions normally require an activation press and, when an ID is set, a matching ID.")]
        [SerializeField, WriteOnly]
        private BoolRef _canInteract;

        [OptionalField]
        [Tooltip("The Interactable currently selected by the Interactor. Selection does not mean it has been activated.")]
        [SerializeField, WriteOnly]
        private InteractableRef _interactable;

        [OptionalField]
        [Tooltip("The selected Interactable's target GameObject, or its own GameObject if no target is assigned.")]
        [SerializeField, WriteOnly]
        private GameObjectRef _targetGameObject;

        [OptionalField]
        [Tooltip("The selected Interactable's interaction name, such as 'Open'. Defaults to 'Generic' when no name is set.")]
        [SerializeField, WriteOnly]
        private StringRef _interaction;

        [OptionalField]
        [Tooltip("The Activation ID configured on the selected Interactable, such as 'Use' or 'AltUse'. This is separate from the input Activation ID above.")]
        [SerializeField, WriteOnly]
        private StringRef _activationId;

        [OptionalField]
        [Tooltip("The selected Interactable's approach direction in world space, based on its configured direction axis.")]
        [SerializeField, WriteOnly]
        private Vector3Ref _normal;

        [OptionalField]
        [Tooltip("Measured distance from the Interactor's interaction origin to the selected Interactable. The Interactable controls whether this uses its transform or a collider.")]
        [SerializeField, WriteOnly]
        private FloatRef _distance;

        [OptionalField]
        [Tooltip("Local FSM event sent when an Interactable becomes newly activated. It is not sent again each update while the same interaction remains active.")]
        [SerializeField]
        private EventRef _interactEvent;

        public override bool CanExecute() => CheckParameters(_interactionController);

        public override void OnStop()
        {
            if (_controller)
            {
                _controller.StopHoverTracking();
            }
        }

        public override void Execute()
        {
            _controller = _interactionController.Value;
            if (!_controller)
            {
                ClearOutputs();
                return;
            }

            _controller.BeginDeferredSystemEvents();
            try
            {
                _controller.EnsurePassiveStateUpdated();
                _controller.TryActivate(ResolveInteractPressed(),
                                        _interactActivationId != null && _interactActivationId.IsAssigned ? _interactActivationId.Value : string.Empty);

                if (_canInteract is { IsAssigned: true })
                {
                    _canInteract.Value = _controller.CanInteract;
                }

                if (_controller.CurrentSelectionInteractable)
                {
                    if (_interactable is { IsAssigned: true }) _interactable.Value = _controller.CurrentSelectionInteractable;
                    if (_targetGameObject is { IsAssigned: true }) _targetGameObject.Value = _controller.CurrentSelectionTarget;
                    if (_interaction is { IsAssigned: true }) _interaction.Value = _controller.CurrentInteraction;
                    if (_activationId is { IsAssigned: true }) _activationId.Value = _controller.CurrentActivationId;
                    if (_normal is { IsAssigned: true }) _normal.Value = _controller.CurrentNormal;
                    if (_distance is { IsAssigned: true }) _distance.Value = _controller.CurrentSelectionDistance;
                }
                else
                {
                    ClearSelectionOutputs();
                }

                if (_controller.DidActivateThisUpdate)
                {
                    _controller.FlushPendingSystemEvents();
                    SendEvent(_interactEvent);
                }
            }
            finally
            {
                _controller.FlushPendingSystemEvents();
            }
        }

        public override string GetSummary()
        {
            return "Update {_interactionController} interactions" +
                   _activationMode switch
                   {
                       InteractorActivationMode.Manual => " with {_interactPressed}",
                       InteractorActivationMode.MouseButtonDown => " with mouse {_mouseButton:mouseButton} button down",
                       InteractorActivationMode.MouseButton => " with mouse{_mouseButton:mouseButton} button",
                       _ => string.Empty
                   } +
                   " {_interactable:output} {_targetGameObject:output} {_interaction:output} {_interactEvent}";
        }

        private bool HideInteractPressed => _activationMode != InteractorActivationMode.Manual;
        private bool HideMouseButton => _activationMode is InteractorActivationMode.Manual or InteractorActivationMode.None;
        private bool HideInteractionId => _activationMode == InteractorActivationMode.None;

        private bool ResolveInteractPressed()
        {
            return _activationMode switch
            {
                InteractorActivationMode.Manual => _interactPressed is { Value: true },
                InteractorActivationMode.MouseButtonDown => InputShim.GetMouseButtonDown(_mouseButton.Value),
                InteractorActivationMode.MouseButton => InputShim.GetMouseButton(_mouseButton.Value),
                _ => false
            };
        }

        private void ClearOutputs()
        {
            if (_canInteract is { IsAssigned: true }) _canInteract.Value = false;
            ClearSelectionOutputs();
        }

        private void ClearSelectionOutputs()
        {
            if (_interactable is { IsAssigned: true }) _interactable.Value = null;
            if (_targetGameObject is { IsAssigned: true }) _targetGameObject.Value = null;
            if (_interaction is { IsAssigned: true }) _interaction.Value = string.Empty;
            if (_activationId is { IsAssigned: true }) _activationId.Value = string.Empty;
            if (_normal is { IsAssigned: true }) _normal.Value = Vector3.zero;
            if (_distance is { IsAssigned: true }) _distance.Value = 0f;
        }

#if UNITY_EDITOR
        public override bool HasDebugInfo => true;

        public override string GetDebugInfo()
        {
            return _controller ? _controller.GetDebugInfo() : string.Empty;
        }
#endif
    }
}
