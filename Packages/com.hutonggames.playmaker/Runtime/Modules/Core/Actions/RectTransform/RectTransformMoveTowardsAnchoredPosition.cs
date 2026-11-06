using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.RectTransform)]
    [ActionDescription("Moves a RectTransform towards an anchored position with optional smoothing and max speed. " +
                       "The position and speed are in anchored-position units.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/RectTransform-anchoredPosition.html")]
    public sealed class RectTransformMoveTowardsAnchoredPosition : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;
        public override UpdateMode RequiredUpdateModes => UpdateMode.EveryFrame;
        public override bool CanFinish => true;

        [Tooltip("The RectTransform to move.")]
        [SerializeField] private RectTransformVar _rectTransform;

        [Tooltip("The anchored position to move towards.")]
        [SerializeField] private Vector2Var _position;

        [VarSlider(0f, 1f)]
        [Tooltip("Smooth time in seconds. 0 = no smoothing (instant if Max Speed is 0).")]
        [SerializeField] private FloatVar _smoothTime;

        [VarSlider(0, 20)]
        [Tooltip("The maximum speed in anchored-position units per second. 0 = uncapped.")]
        [SerializeField, DefaultValue(1f)] private FloatVar _maxSpeed;

        [Tooltip("Finish within this distance of the target. -1 = never finish.")]
        [SerializeField, DefaultValue(0.01f)] private FloatVar _finishDistance;

        [OptionalField]
        [Tooltip("Event to send when the move has finished.")]
        [SerializeField] private EventRef _finishedEvent;

        [NonSerialized] private float _distanceToTarget;

        private readonly SmoothMoveToHelper _smoother = new SmoothMoveToHelper();

        public override bool CanExecute() => CheckParameters(_rectTransform, _position, _maxSpeed);

        public override void OnStart() => _smoother.Reset();

        public override void Execute()
        {
            var rectTransform = _rectTransform.Value;
            var current = rectTransform.anchoredPosition;
            var target = _position.Value;
            var next = _smoother.Update(
                MoveAxis.XY,
                new Vector3(current.x, current.y, 0f),
                new Vector3(target.x, target.y, 0f),
                _smoothTime.Value,
                _maxSpeed.Value);

            rectTransform.anchoredPosition = new Vector2(next.x, next.y);

            var finishDistance = _finishDistance.Value;
            if (finishDistance < 0f) return;

            _distanceToTarget = Vector2.Distance(rectTransform.anchoredPosition, target);
            if (_distanceToTarget <= finishDistance)
            {
                SendEvent(_finishedEvent);
                Finish();
            }
        }

        public override string GetSummary() =>
            "Move {_rectTransform} towards anchored position {_position}" +
            (_smoothTime.IsNotDefault() ? " in {_smoothTime}s" : "") +
            (_maxSpeed.IsVariable || _maxSpeed.Value > 0f ? " at {_maxSpeed}/s" : "") +
            (_finishDistance.Value >= 0f ? " until <= {_finishDistance}" : "") +
            (_finishedEvent.IsSet ? " {_finishedEvent}" : "");

#if UNITY_EDITOR
        public override bool HasDebugInfo => true;
        public override string GetDebugInfo() => $"Distance: {_distanceToTarget:0.##}";
#endif
    }
}
