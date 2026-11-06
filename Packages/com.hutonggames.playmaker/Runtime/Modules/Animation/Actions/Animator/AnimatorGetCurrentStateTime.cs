using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.Animator)]
    [ActionDescription("Gets the current Animator state's normalized time and playback position in seconds on a specified layer. Normalized time reaches 1 after one loop.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Animator.GetCurrentAnimatorStateInfo.html")]
    public sealed class AnimatorGetCurrentStateTime : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;

        [Tooltip("The Animator.")]
        [SerializeField]
        private AnimatorVar _animator;

        [Tooltip("The layer index.")]
        [SerializeField]
        private IntegerVar _layerIndex;

        [ActionHeader("Results")]

        [Tooltip("Store the normalized time. The fractional part is the progress through the current loop.")]
        [SerializeField, WriteOnly, OptionalField]
        private FloatRef _normalizedTime;

        [Tooltip("Store the playback position in seconds (normalized time times the current state length), including completed loops.")]
        [SerializeField, WriteOnly, OptionalField]
        private FloatRef _time;

        public override bool CanExecute() => CheckParameters(_animator, _layerIndex);

        public override string ErrorCheck() =>
            _normalizedTime.IsAssigned || _time.IsAssigned ? null : "Assign at least one output.";

        public override void Execute()
        {
            var stateInfo = _animator.Value.GetCurrentAnimatorStateInfo(_layerIndex.Value);

            if (_normalizedTime.IsAssigned)
                _normalizedTime.Value = stateInfo.normalizedTime;

            if (_time.IsAssigned)
                _time.Value = stateInfo.normalizedTime * stateInfo.length;
        }

        public override string GetSummary() =>
            "Get {_animator} current state time " + 
            (_layerIndex.IsNotDefault() ? "({_layerIndex})" : "") +
            "{_normalizedTime:output} {_time:output}";
    }
}
