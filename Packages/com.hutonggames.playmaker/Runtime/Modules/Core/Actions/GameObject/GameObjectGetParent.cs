using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.GameObject)]
    [ActionDescription("Get the parent GameObject from the Transform hierarchy.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Transform-parent.html")]
    public sealed class GameObjectGetParent : BaseAction
    {
        [SerializeField, OwnerDefaultValue]
        [Tooltip("The GameObject to get the parent of.")]
        private GameObjectVar _gameObject;

        [SerializeField, WriteOnly]
        [Tooltip("Store the parent GameObject, or null if there is no parent.")]
        private GameObjectRef _result;

        public override bool CanExecute()
        {
            return CheckParameters(_gameObject, _result);
        }

        public override void Execute()
        {
            var gameObject = _gameObject.Value;
            var parent = gameObject != null ? gameObject.transform.parent : null;
            _result.Value = parent != null ? parent.gameObject : null;
        }

        public override string GetSummary()
        {
            return "Get {_gameObject} parent -> {_result}";
        }
    }
}
