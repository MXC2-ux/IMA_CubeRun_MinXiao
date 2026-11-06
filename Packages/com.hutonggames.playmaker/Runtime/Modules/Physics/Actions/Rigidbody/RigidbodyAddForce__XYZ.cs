
using JetBrains.Annotations;
using System;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ConvertibleGroup("RigidbodyAddForce")]
	[ActionDescription("Adds a force to the Rigidbody.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody.AddForce.html")]
	public sealed class RigidbodyAddForce__XYZ : BaseAction
	{
		
		[Tooltip("The Rigidbody.")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Size of force along the world x-axis.")]
		[OptionalField]
		[SerializeField]
		private FloatVar _x;
		
		[Tooltip("Size of force along the world y-axis.")]
		[OptionalField]
		[SerializeField]
		private FloatVar _y;
		
		[Tooltip("Size of force along the world z-axis.")]
		[OptionalField]
		[SerializeField]
		private FloatVar _z;
		
		[Tooltip("Type of force to apply.")]
		[SerializeField]
		private ForceModeVar _mode;
		
		public override bool CanExecute()
		{
			return CheckParameters(_rigidbody, _mode);
		}
		
		public override void Execute()
		{
			var rigidbody = _rigidbody.Value;
			if (rigidbody == null) return;
			
			rigidbody.AddForce(_x.IsNone ? 0f : _x.Value, _y.IsNone ? 0f : _y.Value, _z.IsNone ? 0f : _z.Value, _mode.Value);
		}
		
		public override string GetSummary()
		{
			return "Add force ({_x}, {_y}, {_z}) to {_rigidbody} as {_mode}";
		}
	}
}
