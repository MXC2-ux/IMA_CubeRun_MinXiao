
using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global


namespace HutongGames.PlayMaker.Actions
{
	
	/// <summary>
	/// Base for actions that read or change a named Animator parameter.
	/// </summary>
	/// <remarks>
	/// This base validates the Animator and parameter name and caches the parameter hash.
	/// Derived Execute methods call base.Execute before using <see cref="ParameterID"/>.
	/// </remarks>
	[System.Serializable]
	public abstract class BaseAnimatorParameterAction : BaseAction
	{
		[DisplayOrder(-1000)]
		[Tooltip("The Animator.")]
		[SerializeField]
		protected AnimatorVar _animator;
		
		[DisplayOrder(-999)]
		[Tooltip("The parameter name.")]
		[AnimatorParameter]
		[SerializeField]
		protected StringVar _name;

		private string _cachedName;
		protected int ParameterID;
		
		public override bool CanExecute() => CheckParameters(_animator, _name);

		public override void Execute()
		{
			UpdateCachedId();
		}

		private void UpdateCachedId()
		{
			if (_cachedName == _name.Value) return;
			ParameterID = Animator.StringToHash(_name.Value);
			_cachedName = _name.Value;
		}
	}
}
