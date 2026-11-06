
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Transform)]
	[ActionDescription("The implementation of this method applies a rotation of zAngle degrees around the" +
		" z axis, xAngle degrees around the x axis, and yAngle degrees around the y axis " +
		"(in that order).")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Transform.Rotate.html")]
	public sealed class TransformRotate__XYZ : BaseAction
	{
		[OwnerDefaultValue]
		[Tooltip("The Transform.")]
		[SerializeField]
		private TransformVar _transform;
		
		[Tooltip("Degrees to rotate the GameObject around the X axis." + Strings.PerSecondNote)]
		[OptionalField]
		[SerializeField]
		private FloatVar _xAngle;
		
		[Tooltip("Degrees to rotate the GameObject around the Y axis." + Strings.PerSecondNote)]
		[OptionalField]
		[SerializeField]
		private FloatVar _yAngle;
		
		[Tooltip("Degrees to rotate the GameObject around the Z axis." + Strings.PerSecondNote)]
		[OptionalField]
		[SerializeField]
		private FloatVar _zAngle;
		
		public override bool CanUsePerSecond => true;
		
		public override bool CanExecute() => CheckParameters(_transform);

		public override void Execute()
		{
			var transform = _transform.Value;
			if (transform == null) return;
			transform.Rotate(
				(_xAngle.IsNone ? 0f : _xAngle.Value) * PerSecond,
				(_yAngle.IsNone ? 0f : _yAngle.Value) * PerSecond,
				(_zAngle.IsNone ? 0f : _zAngle.Value) * PerSecond);
		}

		public override string GetSummary() => 
			"Rotate {_transform} {_xAngle} {_yAngle} {_zAngle} {PerSecond}";
	}
}
