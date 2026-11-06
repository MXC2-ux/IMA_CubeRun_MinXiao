
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Transform)]
	[ActionDescription("Set the local space position of the Transform in X and Y.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Transform-position.html")]
	public sealed class TransformSetLocalPosition__XY : BaseAction
	{
		[OwnerDefaultValue]
		[Tooltip("The Transform")]
		[SerializeField]
		private TransformVar _transform;
		
		[Tooltip("Set X Position")]
		[OptionalField]
		[SerializeField]
		private FloatVar _setXPosition;
		
		[Tooltip("Set Y Position")]
		[OptionalField]
		[SerializeField]
		private FloatVar _setYPosition;
		
		public override bool CanExecute()
		{
			return CheckParameters(_transform);
		}
		
		public override void Execute()
		{
			var transform = _transform.Value;
			if (transform == null) return;
			
			if (_setXPosition.IsNone && _setYPosition.IsNone) return;

			var position = transform.localPosition;
			if (!_setXPosition.IsNone) position.x = _setXPosition.Value;
			if (!_setYPosition.IsNone) position.y = _setYPosition.Value;
			
			transform.localPosition = position;
		}
		
		public override string GetSummary()
		{
			return "Set {_transform} Local Position to X {_setXPosition} Y {_setYPosition}";
		}
	}
}
