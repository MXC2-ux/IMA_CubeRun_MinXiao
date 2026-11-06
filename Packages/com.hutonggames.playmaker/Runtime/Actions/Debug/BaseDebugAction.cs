using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for actions that display live values in the PlayMaker debug panel.
    /// </summary>
    /// <remarks>
    /// Use for debug displays that need the common panel and label lifecycle.
    /// These actions start only in debug builds with the runtime debug display enabled.
    /// </remarks>
    [Serializable]
    [ActionCategory(Category.Debug)]
    public abstract class BaseDebugAction : BaseAction, IHasDebugPanel
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;

        public override bool CanStart() =>
            Debug.isDebugBuild &&
            RuntimeSettings.Instance.DebugDisplayEnabled &&
            base.CanStart();
        
        [SerializeField] protected DebugDisplay DebugDisplay;

        protected DebugPanel Panel;
        protected Label Label;

        public override void OnStateEnter()
        {
            DebugDisplay?.Validate(OwnerGameObject);
        }

        public override void OnStart()
        {
            DebugDisplay ??= new DebugDisplay();
            Panel = new DebugPanel(DebugDisplay);
            Label = new Label();
            Panel.Add(Label);
            
            DebugDisplay.AddPanel(Panel);
        }

        public override void OnStop()
        {
            Panel?.RemoveFromHierarchy();
            Panel = null;
        }

        public void OnUpdateDebugPanel()
        {
            if (Panel == null) return;
            EnsurePanelAttached();
            Execute();
        }

        public void OnAnchorChanged()
        {
            if (DebugDisplay == null) return;
            DebugDisplay.UpdateAnchor(Panel);
        }

        private void EnsurePanelAttached()
        {
            if (Panel == null || Panel.panel != null || DebugDisplay == null) return;
            DebugDisplay.AddPanel(Panel);
        }
    }
}
