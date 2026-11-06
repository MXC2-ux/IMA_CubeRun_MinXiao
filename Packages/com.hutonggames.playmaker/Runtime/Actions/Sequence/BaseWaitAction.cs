namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for sequence actions that keep the state waiting until a condition or delay completes.
    /// </summary>
    /// <remarks>
    /// Use for waits that must block subsequent actions while checking for completion every frame.
    /// Derived actions implement the wait condition and finish when it is satisfied.
    /// </remarks>
    [System.Serializable]
    public abstract class BaseWaitAction : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame | UpdateMode.Blocking;
        public override UpdateMode RequiredUpdateModes => DefaultUpdateMode;
        public override bool CanFinish => true;
    }
}
