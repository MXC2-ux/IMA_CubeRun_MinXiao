using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.Collider2D)]
    [ActionDescription("Enabled 2D colliders will collide with other 2D colliders; disabled 2D colliders won't.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Collider2D-enabled.html")]
    public sealed class Collider2DSetEnabled : BaseAction
    {
        [Tooltip("The Collider2D")]
        [SerializeField]
        private Collider2DVar _collider2D;

        [Tooltip("Set Collider2D Enabled")]
        [SerializeField]
        private BoolVar _setEnabled;

        public override bool CanExecute()
        {
            return CheckParameters(_collider2D, _setEnabled);
        }

        public override void Execute()
        {
            _collider2D.Value.enabled = _setEnabled.Value;
        }

        public override string GetSummary()
        {
            return "Set {_collider2D} enabled to {_setEnabled}";
        }
    }
}
