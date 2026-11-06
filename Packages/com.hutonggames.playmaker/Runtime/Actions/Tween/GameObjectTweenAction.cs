using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for tweens that operate on a GameObject target.
    /// </summary>
    /// <remarks>
    /// Use when a tween needs the target's transform and should require an assigned GameObject
    /// before executing. Value-specific tween behavior belongs in the derived action.
    /// </remarks>
    [Serializable, PublicAPI]
    public abstract class GameObjectTweenAction : BaseTweenAction
    {
        public override Transform TargetTransform => GameObject.Value != null ? GameObject.Value.transform : null;
        
        [OwnerDefaultValue]
        [DisplayOrder(-100)]
        [ActionTarget]
        [Tooltip("The GameObject to tween.")]
        public GameObjectVar GameObject;

        public override bool CanExecute() => base.CanExecute() && GameObject.HasValue();
    }
}
