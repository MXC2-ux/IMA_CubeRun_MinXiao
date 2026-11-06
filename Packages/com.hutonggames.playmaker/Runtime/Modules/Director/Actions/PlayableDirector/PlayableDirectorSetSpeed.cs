using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.PlayableDirector)]
    [ActionDescription("Sets the playback speed of a PlayableDirector's current graph. Use after the director has created its graph, such as after Play. 1 is normal speed.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/Playables.PlayableExtensions.SetSpeed.html")]
    public sealed class PlayableDirectorSetSpeed : BaseAction
    {
        [Tooltip("The PlayableDirector.")]
        [SerializeField]
        private Playables.PlayableDirectorVar _playableDirector;

        [Tooltip("Playback speed multiplier. 1 is normal speed.")]
        [SerializeField, DefaultValue(1f)]
        private FloatVar _setSpeed;

        public override bool CanExecute()
        {
            return CheckParameters(_playableDirector, _setSpeed);
        }

        public override void Execute()
        {
            var graph = _playableDirector.Value.playableGraph;
            var rootCount = graph.IsValid() ? graph.GetRootPlayableCount() : 0;
            if (rootCount == 0)
            {
                LogWarning("PlayableDirector has no playable graph. Play the director before setting its speed.");
                return;
            }

            for (var i = 0; i < rootCount; i++)
            {
                graph.GetRootPlayable(i).SetSpeed(_setSpeed.Value);
            }
        }

        public override string GetSummary()
        {
            return "Set {_playableDirector} speed to {_setSpeed}";
        }
    }
}
