using System;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for actions that listen for a proxy event on a target GameObject.
    /// </summary>
    /// <remarks>
    /// Use for collision, trigger, or similar Unity callbacks represented by a
    /// <see cref="BaseEvent"/>. This base registers the proxy when the action starts and
    /// unregisters it when the action stops; derived actions handle the received event.
    /// </remarks>
    /// <typeparam name="T">The proxy event type to register.</typeparam>
    [Serializable]
    public abstract class GameObjectProxyEventAction<T> : GameObjectAction where T : BaseEvent, new()
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.Update | UpdateMode.OnEventUpdate;
        public override UpdateMode AllowedUpdateModes => UpdateMode.OnEventUpdate;

        [NonSerialized]
        private T _event;
        
        public override void OnStart()
        {
            if (GameObject.HasValue())
            {
                _event ??= new T();
                _event.RegisterCallback(GameObject.Value, OwnerComponent);
            }
        }
        
        public override void OnStop()
        { 
            _event?.UnregisterCallback(OwnerComponent);
        }
        
    }
}
