using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    /// <summary>
    /// Base for actions that interpolate a value over time using tween action blocks.
    /// </summary>
    /// <remarks>
    /// Use when a derived action supplies the value-specific interpolation and applies the resulting
    /// <see cref="BaseAction.Progress"/>. This base runs the update block, exposes easing and loop settings
    /// to tween blocks, and finishes after a non-looping tween reaches its end.
    /// </remarks>
    [Serializable]
    public abstract class BaseTweenAction : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;  
        public override UpdateMode RequiredUpdateModes => UpdateMode.EveryFrame;
        
        public override bool CanFinish => true;

        /// <summary>
        /// An estimate of the "length" of the tween.
        /// Used to convert Speed to Duration.
        /// </summary>
        /// <remarks>
        /// For many tween types its obvious how to measure the distance.
        /// E.g., for a position tween its the distance between start and end positions.
        /// However, some types require a creative interpretation of "distance".
        /// E.g., a scale tween needs to convert start/end scales into a distance.
        /// </remarks>
        public float Distance { get; protected set; } = -1;
        
        /// <summary>
        /// All tweens have a LoopMode,
        /// but we make it an optional ActionBlock to reduce clutter.
        /// </summary>
        public LoopMode LoopMode => Loop?.LoopMode ?? LoopMode.None;
        
        /// <summary>
        /// The number of times the tween has looped.
        /// </summary>
        public int LoopCount { get; set; }
        
        [DisplayOrder(100)]
        [SerializeReference]
        [Tooltip("Easing to use for the Tween.")]
        public TweenEasingBlock Easing;
        
        [DisplayName("Update")]
        [DisplayOrder(101)]
        [DefaultValue(typeof(TimeBasedTweenUpdateBlock))]
        [SerializeReference]
        [Tooltip("How to update the Tween (time based, speed based etc.)")]
        public TweenUpdateBlock TweenUpdate;
        

        [DisplayOrder(102)]
        [SerializeReference, OptionalField]
        [Tooltip("Loop settings.")]
        public TweenLoopBlock Loop;
        
        /*
        [HideLabel]
        [OptionalField]
        [DisplayOrder(103)]
        [SerializeReference]
        [Tooltip("Get information about the tween.")]
        public List<TweenInfoBlock> GetInfo;*/
        
        [DisplayOrder(104)]
        [SerializeReference, OptionalField]
        [Tooltip("Tween events.")]
        public TweenEventsBlock Events;

        public override void OnStart()
        {
            LoopCount = 0;
        }
        
        public override bool CanExecute() => Easing.IsValid && TweenUpdate.IsValid;

        public override void Execute()
        {
            TweenUpdate.Execute();
            
            Progress = TweenUpdate.GetProgress();
        }

        protected override bool IsFinished()
        {
            if (Progress >= 1 && !IsLooping())
            {
                // Subclasses apply Progress after base.Execute; complete only after their write.
                SendEvent(Events?.FinishedEvent);
                return true;
            }

            return false;
        }

        private bool IsLooping()
        {
            if (Loop == null) return false;
            return Loop.LoopMode != LoopMode.None;
        }

        /// <summary>
        /// Append this to tween specific GetSummary
        /// </summary>
        public override string GetSummary() =>
            " {TweenUpdate} ({Easing}" 
            + (Loop != null ? ", {Loop})" : ")")
            + (Events != null ? " {Events}" : "");
    }
}
