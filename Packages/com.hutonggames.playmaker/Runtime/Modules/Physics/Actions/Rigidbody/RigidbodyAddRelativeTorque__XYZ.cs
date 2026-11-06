
using JetBrains.Annotations;
using System;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ConvertibleGroup("RigidbodyAddTorque")]
	[ActionDescription("Adds a torque to the rigidbody relative to its coordinate system.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody.AddRelativeTorque.html")]
	public sealed class RigidbodyAddRelativeTorque__XYZ : BaseAction
	{
		
		[Tooltip("The Rigidbody.")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Size of torque along the local x-axis.")]
		[OptionalField]
		[SerializeField]
		private FloatVar _x;
		
		[Tooltip("Size of torque along the local y-axis.")]
		[OptionalField]
		[SerializeField]
		private FloatVar _y;
		
		[Tooltip("Size of torque along the local z-axis.")]
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

			rigidbody.AddRelativeTorque(_x.IsNone ? 0f : _x.Value, _y.IsNone ? 0f : _y.Value, _z.IsNone ? 0f : _z.Value, _mode.Value);
		}
		
		public override string GetSummary()
		{
			return "Add relative torque ({_x}, {_y}, {_z}) to {_rigidbody} as {_mode}";
		}
	}
}
