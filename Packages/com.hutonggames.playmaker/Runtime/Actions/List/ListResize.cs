using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.List)]
    [ActionDescription("Resize a list by removing items from the end or adding default values.")]
    public class ListResize : BaseAction
    {
        [BaseType(typeof(IList))]
        [Tooltip("The list variable to resize.")]
        [SerializeReference] public IListVariableRef List;

        [Tooltip("The new number of items in the list.")]
        public IntegerVar NewSize;

        public override bool CanExecute() => CheckParameters(List, NewSize);

        public override void Execute()
        {
            var newSize = NewSize.Value;
            if (newSize < 0)
                throw new ArgumentOutOfRangeException(nameof(NewSize), newSize, "List size cannot be negative.");

            var list = List.ListVariable;
            var count = list.Count;
            if (newSize < count)
            {
                list.RemoveRange(newSize, count - newSize);
            }
            else if (newSize > count)
            {
                var defaultValue = list.ElementType.IsValueType ? Activator.CreateInstance(list.ElementType) : null;
                for (var i = count; i < newSize; i++)
                    list.AddItem(defaultValue);
            }
        }

        public override string GetSummary() => "Resize {List} to {NewSize} items";
    }
}
