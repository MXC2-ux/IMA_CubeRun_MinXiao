using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable, PublicAPI]
    [DisplayName("Reset Loop")]
    [ActionCategory(Category.Loop)]
    [ConvertibleGroup("LoopControl")]
    [ActionDescription("When the reset flag is true, restarts the current loop at its first item " +
                       "and skips the rest of this iteration. Sets the flag back to false.")]
    public class ResetLoop : BaseAction
    {
        public override bool SkipsLoop => true;

        [Tooltip("Reset the loop when this bool variable is true. The action sets it back to false.")]
        [SerializeField]
        private BoolRef _resetFlag;

        public override bool CanExecute() => CheckParameters(_resetFlag);

        public override void Execute()
        {
            if (_resetFlag.Value && ResetCurrentLoop())
            {
                _resetFlag.Value = false;
            }
        }

        public override string GetSummary() => "Reset loop if {_resetFlag} is true";
    }
}
