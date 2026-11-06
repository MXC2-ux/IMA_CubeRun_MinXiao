using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.GameObject)]
    [ActionDescription("Set a GameObject's parent in the Transform hierarchy. Leave the parent empty to unparent it.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Transform.SetParent.html")]
    public sealed class GameObjectSetParent : BaseAction
    {
        [SerializeField, OwnerDefaultValue]
        [Tooltip("The GameObject to reparent.")]
        private GameObjectVar _gameObject;

        [SerializeField, CanBeNullOrEmpty]
        [Tooltip("The new parent GameObject. Leave empty to unparent.")]
        private GameObjectVar _parent;

        [SerializeField, DefaultValue(true)]
        [Tooltip("Keep the GameObject's world position, rotation, and scale.")]
        private BoolVar _worldPositionStays;

        public override bool CanExecute()
        {
            return CheckParameters(_gameObject, _worldPositionStays);
        }

        public override void Execute()
        {
            var gameObject = _gameObject.Value;
            if (gameObject == null) return;

            var parent = _parent.Value;
            gameObject.transform.SetParent(parent != null ? parent.transform : null, _worldPositionStays.Value);
        }

        public override string GetSummary()
        {
            return "Set {_gameObject} parent to {_parent}";
        }
    }
}
