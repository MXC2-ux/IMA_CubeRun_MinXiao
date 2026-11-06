using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for tweens that move a value from or to a target, optionally relative to its current value.
    /// </summary>
    /// <remarks>
    /// Use when the action can obtain a current and target value through <see cref="GetCurrentValue"/>
    /// and <see cref="GetTargetValue"/>. Startup assigns the from/to endpoints according to the direction;
    /// the derived action interpolates those endpoints and applies the result.
    /// </remarks>
    /// <typeparam name="T">The value type being tweened.</typeparam>
    /// <typeparam name="TVar">The PlayMaker variable wrapper for the target value.</typeparam>
    [Serializable]
    public abstract class BaseDirectionTweenAction<T, TVar> : BaseTweenAction
    {
        [Tooltip("Tween from or to the given value.")]
        [SerializeField]
        protected TweenDirection _direction;
        
        [Tooltip("The value to tween from/to.")]
        [SerializeField]
        [FormerlySerializedAs("_position")]
        [FormerlySerializedAs("_rotation")]
        [FormerlySerializedAs("_scale")]
        protected TVar _value;
        
        [Tooltip("Defines an offset from the current value.")]
        [SerializeField]
        protected BoolVar _relative;
        
        [NonSerialized] protected T FromValue;
        [NonSerialized] protected T ToValue;
        
        public override bool CanExecute() => CheckParameters(_value, _relative) && base.CanExecute();
        
        public override void OnStart()
        {
            base.OnStart();

            if (_direction == TweenDirection.To)
            {
                FromValue = GetCurrentValue();
                ToValue = GetTargetValue();
            }
            else
            {
                FromValue = GetTargetValue();
                ToValue = GetCurrentValue();           
            }
        }

        protected abstract T GetCurrentValue();
        
        protected abstract T GetTargetValue();
    }
}
