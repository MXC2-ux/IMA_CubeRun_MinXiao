using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Rigidbody)]
    [ConvertibleGroup("RigidbodySetVelocity")]
    [ActionDescription("Set the velocity of the rigidbody using individual x, y, z components in either world or local space. It represents the rate of change of Rigidbody position.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html")]
    public sealed class RigidbodySetVelocity_XYZ : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.FixedUpdate | UpdateMode.EveryFrame;
        
        [Tooltip("The Rigidbody")]
        [SerializeField]
        private RigidbodyVar _rigidbody;
        
        [Tooltip("Set X Velocity")]
        [OptionalField]
        [SerializeField]
        private FloatVar _setXVelocity;
        
        [Tooltip("Set Y Velocity")]
        [OptionalField]
        [SerializeField]
        private FloatVar _setYVelocity;
        
        [Tooltip("Set Z Velocity")]
        [OptionalField]
        [SerializeField]
        private FloatVar _setZVelocity;

        [Tooltip("Select if the velocity is in world or local space")]
        [SerializeField, DefaultValue(Space.World)]
        private SpaceVar _space;
        
        public override bool CanExecute()
        {
            return CheckParameters(_rigidbody);
        }
        
        public override void Execute()
        {
            if (_setXVelocity.IsNone && _setYVelocity.IsNone && _setZVelocity.IsNone) return;

#if UNITY_6000_0_OR_NEWER
            var velocity = _rigidbody.Value.linearVelocity;
#else
            var velocity = _rigidbody.Value.velocity;
#endif
            if (_space.Value == Space.Self)
            {
                velocity = _rigidbody.Value.transform.InverseTransformDirection(velocity);
            }

            if (!_setXVelocity.IsNone) velocity.x = _setXVelocity.Value;
            if (!_setYVelocity.IsNone) velocity.y = _setYVelocity.Value;
            if (!_setZVelocity.IsNone) velocity.z = _setZVelocity.Value;

            if (_space.Value == Space.Self)
            {
                velocity = _rigidbody.Value.transform.TransformDirection(velocity);
            }

#if UNITY_6000_0_OR_NEWER
            _rigidbody.Value.linearVelocity = velocity;
#else
            _rigidbody.Value.velocity = velocity;
#endif
        }
        
        public override string GetSummary()
        {
            return "Set {_rigidbody} velocity to ({_setXVelocity}, {_setYVelocity}, {_setZVelocity})" +
                   (_space.IsNotDefault(Space.World) ? " (local)" : string.Empty);
        }
    }
}
