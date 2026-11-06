
using HutongGames.PlayMaker.Internal;
using HutongGames.PlayMaker.UGUIEvents;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions.EventSystems
{
    /// <summary>
    /// Base for UI EventSystem actions that forward a proxy event and its event data.
    /// </summary>
    /// <remarks>Use for simple UI callbacks with no extra pointer or axis filtering.</remarks>
    /// <typeparam name="T">The input proxy component that raises the event.</typeparam>
    [System.Serializable]
    public abstract class BaseEventAction<T> : BaseAction where T : BaseInputProxyComponent
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.OnEventUpdate;
        
        [OptionalField]
        [Tooltip("Event to send.")] 
        [SerializeField] 
        protected EventRef _sendEvent;
        
        [WriteOnly, OptionalField]
        [Tooltip("Store the EventData in a variable.")]
        [SerializeField]
        protected BaseEventDataRef _eventData;

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
            _eventData.Value = eventData;
        }
        
        public override string GetSummary()
        {
            var summary = Nicify.NicifyName(GetType().Name.TrimEnd("Event"));
            
            if (_sendEvent.Event != null)   
            {
                summary += " {_sendEvent}";
            }
            
            if (_eventData.IsAssigned)
            {
                summary += " -> {_eventData}";
            }

            return summary;
        }
    }
}
