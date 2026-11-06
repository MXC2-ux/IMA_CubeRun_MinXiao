using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable, PublicAPI]
    [ActionCategory(Category.GameObjectData)]
    [ActionDescription("Add a Data Table Component for a DataDefinition to a GameObject. Optionally use an existing matching component.")]
    public sealed class GameObjectAddDataTable : BaseAction
    {
        [Tooltip("The GameObject to add the Data Table Component to.")]
        public GameObjectVar GameObject;
        
        [RequiredField]
        [Tooltip("The DataDefinition (schema) for the Data Table Component.")]
        public DataDefinitionVar DataDefinition;

        [Tooltip("Use an existing Data Table Component with the same DataDefinition if one is found.")]
        [DefaultValue(true)]
        public BoolVar UseExisting;

        [ActionHeader("Output")]

        [OptionalField, WriteOnly]
        [Tooltip("Store the Data Table Component that was found or added.")]
        public DataTableComponentRef DataTableComponent;

        [OptionalField, WriteOnly]
        [Tooltip("Store the DataTable from the component.")]
        public DataTableRef DataTable;

        [OptionalField, WriteOnly]
        [Tooltip("True if a new component was added. False if an existing matching component was used.")]
        public BoolRef Added;

        [OptionalField, WriteOnly]
        [Tooltip("True if a matching Data Table Component was found or added.")]
        public BoolRef Succeeded;

        public override void Execute()
        {
            if (DataTableComponent.IsAssigned) DataTableComponent.Value = null;
            if (DataTable.IsAssigned) DataTable.Value = null;
            if (Added.IsAssigned) Added.Value = false;
            if (Succeeded.IsAssigned) Succeeded.Value = false;

            var go = GameObject.Value;
            var definition = DataDefinition?.Value;
            if (go == null || definition == null) return;

            DataTableComponent component = null;
            if (UseExisting.Value)
            {
                foreach (var candidate in go.GetComponents<DataTableComponent>())
                {
                    if (candidate.DataDefinition != definition) continue;
                    component = candidate;
                    break;
                }
            }

            var added = component == null;
            if (added)
                component = go.AddComponent<DataTableComponent>();

            var table = component.Table;
            table.DataDefinition = definition;

            if (DataTableComponent.IsAssigned) DataTableComponent.Value = component;
            if (DataTable.IsAssigned) DataTable.Value = table;
            if (Added.IsAssigned) Added.Value = added;
            if (Succeeded.IsAssigned) Succeeded.Value = true;
        }

        public override string GetSummary() =>
            "Add {DataDefinition} DataTable to {GameObject} {DataTableComponent:output} {DataTable:output}";
    }
}
