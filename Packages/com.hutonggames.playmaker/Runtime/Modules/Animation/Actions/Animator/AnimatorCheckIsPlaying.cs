
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Animator)]
	[ActionDescription("Checks if an animation state is playing.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Animator.Play.html")]
	public sealed class AnimatorCheckIsPlaying : BaseTrueFalseAction
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

		protected override bool Test() => AnimatorUtils.IsPlaying(_animator.Value, _layer.Value, _stateName.Value);

		protected override string TrueSummary => "{_animator} {_stateName} is playing";
		protected override string FalseSummary => "{_animator} {_stateName} is not playing";

#if UNITY_EDITOR
		public override bool HasDebugInfo => true;

		public override string GetDebugInfo()
		{
			var animator = _animator?.Value;
			var layer = _layer?.Value ?? -1;
			if (!animator || !animator.isInitialized || layer < 0 || layer >= animator.layerCount)
				return "Animator state unavailable";

			var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
			var currentState = GetCurrentStateName(animator, layer, stateInfo);

			var clips = animator.GetCurrentAnimatorClipInfo(layer);
			var currentClip = clips.Length > 0 && clips[0].clip ? clips[0].clip.name : "None";
			return $"Current State: {currentState}\nCurrent Clip: {currentClip}";
		}

		private string GetCurrentStateName(Animator animator, int layer, AnimatorStateInfo stateInfo)
		{
			var controller = animator.runtimeAnimatorController;
			while (controller is AnimatorOverrideController overrideController)
				controller = overrideController.runtimeAnimatorController;

			if (controller is AnimatorController animatorController)
			{
				var layers = animatorController.layers;
				if (layer < layers.Length)
				{
					var controllerLayer = layers[layer];
					var syncedLayer = controllerLayer.syncedLayerIndex;
					var hasSyncedLayer = syncedLayer >= 0 && syncedLayer < layers.Length;
					var stateMachine = hasSyncedLayer
						? layers[syncedLayer].stateMachine
						: controllerLayer.stateMachine;

					var stateName = FindStateName(stateMachine, controllerLayer.name, stateInfo.fullPathHash);
					if (stateName == null && hasSyncedLayer)
						stateName = FindStateName(stateMachine, layers[syncedLayer].name, stateInfo.fullPathHash);
					if (stateName != null)
						return stateName;
				}
			}

			var configuredName = _stateName?.Value;
			return AnimatorUtils.MatchesStateName(stateInfo, configuredName)
				? configuredName
				: "Unavailable";
		}

		private static string FindStateName(AnimatorStateMachine stateMachine, string path, int stateHash)
		{
			if (!stateMachine) return null;

			foreach (var child in stateMachine.states)
			{
				if (!child.state) continue;
				var fullPath = path + "." + child.state.name;
				if (Animator.StringToHash(fullPath) == stateHash)
					return fullPath;
			}

			foreach (var child in stateMachine.stateMachines)
			{
				if (!child.stateMachine) continue;
				var stateName = FindStateName(child.stateMachine, path + "." + child.stateMachine.name, stateHash);
				if (stateName != null)
					return stateName;
			}

			return null;
		}
#endif
	}
}
