
using HutongGames.PlayMaker.Internal;
using HutongGames.PlayMaker.UGUIEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions.EventSystems
{
    /// <summary>
    /// Base for UI EventSystem actions that receive axis or navigation events.
    /// </summary>
    /// <remarks>Use for proxy callbacks that include a move direction and event data.</remarks>
    /// <typeparam name="T">The input proxy component that raises the event.</typeparam>
    [System.Serializable]
    public abstract class BaseAxisEventAction<T> : BaseAction where T : BaseInputProxyComponent
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.OnEventUpdate;
        
        [OptionalField]
        [Tooltip("Event to send.")] 
        [SerializeField] 
        protected EventRef _sendEvent;
        
        [WriteOnly, OptionalField]
        [Tooltip("Store the AxisEventData in a variable.")]
        [SerializeField]
        protected BaseEventDataRef _axisEventData;

        private T _proxyComponent;
        
        public override void OnStart()
        {
            _proxyComponent ??= OwnerGameObject.GetOrAddComponent<T>();
            _proxyComponent.Updated += HandleEvent;
        }
        
        public override void OnStop()
        {
            _proxyComponent.Updated -= HandleEvent;
        }
        
        private void HandleEvent(BaseEventData eventData)
        {
            SendEvent(_sendEvent);
            _axisEventData.Value = (AxisEventData) eventData;
        }
        
        public override string GetSummary()
        {
            var summary = Nicify.NicifyName(GetType().Name.TrimEnd("Event"));
            
            if (_sendEvent.Event != null)   
            {
                summary += " {_sendEvent}";
            }
            
            if (_axisEventData.IsAssigned)
            {
                summary += " -> {_axisEventData}";
            }

            return summary;
        }
    }
}
