using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace HutongGames.PlayMaker.Actions
{
	[Serializable]
	[ActionCategory(Category.String)]
	[ActionDescription("Joins individual string values or String variables using the specified separator.")]
	[HelpURL("https://learn.microsoft.com/en-us/dotnet/api/system.string.join")]

	public sealed class BuildString : BaseAction
	{
		[Tooltip("Strings to join. Each item can be a value or a String variable.")]
		[SerializeField]
		private StringVar[] _strings;
		
		[Tooltip("Separator to insert between strings.")]
		[SerializeField, OptionalField]
		private StringVar _separator;

		[Tooltip("Add Separator to end of built string.")]
		[SerializeField]
		private BoolVar _addToEnd;
		
		[Tooltip("Resolve {VariableName} and {VariableName.Property} tokens in the built string.")]
		[DefaultValue(false)]
		public BoolVar _useVariableTokens;
		
		[Tooltip("Store the result in String variable.")]
		[SerializeField]
		[WriteOnly]
		private StringRef _result;
		
		public override bool CanExecute()
		{
			return CheckParameters(_strings, _result);
		}

		public override void Execute()
		{
			var builder = new System.Text.StringBuilder();

			for (int i = 0; i < _strings.Length; i++)
			{
				builder.Append(_strings[i].Value);

				if (i < _strings.Length - 1 || _addToEnd.Value)
				{
					builder.Append(_separator.Value);
				}
			}

			var value = builder.ToString();
			_result.Value = _useVariableTokens is { Value: true }
				? DebugLogTextFormatter.Format(value, Fsm?.Variables)
				: value;
		}
		
		public override string GetSummary()
		{
			return "Join {_strings} with {_separator} -> {_result}";
		}
	}
}
