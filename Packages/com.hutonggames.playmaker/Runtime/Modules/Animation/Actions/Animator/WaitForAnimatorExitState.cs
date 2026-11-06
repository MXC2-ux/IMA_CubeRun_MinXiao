using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.Animator)]
    [ActionDescription("Wait until the Animator's current state no longer matches the specified state.")]
    public sealed class WaitForAnimatorExitState : BaseWaitAction
    {
        [Tooltip("The Animator.")]
        [SerializeField]
        private AnimatorVar _animator;

        [Tooltip("The state name or full path, e.g. Idle_Loop or Base Layer.Idle_Loop.")]
        [SerializeField]
        private StringVar _stateName;

        [Tooltip("The layer index.")]
        [SerializeField]
        private IntegerVar _layer;

        public override bool CanExecute() => CheckParameters(_animator, _stateName, _layer);

        public override void Execute()
        {
            var stateInfo = _animator.Value.GetCurrentAnimatorStateInfo(_layer.Value);
            if (!AnimatorUtils.MatchesStateName(stateInfo, _stateName.Value))
                Finish();
        }

        public override string GetSummary()
        {
            return "Wait for {_animator} to exit {_stateName} " 
                   + (_layer.IsNotDefault() ? "on layer {_layer}" : "");
        }
    }
}
