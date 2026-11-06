using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.RectTransform)]
    [ActionDescription("Moves a RectTransform's pivot towards a target RectTransform's pivot with optional smoothing and max speed. " +
                       "Converts the target position between UI layouts each frame.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/RectTransform-anchoredPosition.html")]
    public sealed class RectTransformMoveTowardsTarget : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;
        public override UpdateMode RequiredUpdateModes => UpdateMode.EveryFrame;
        public override bool CanFinish => true;

        [Tooltip("The RectTransform to move.")]
        [SerializeField] private RectTransformVar _rectTransform;

        [Tooltip("The target RectTransform to follow.")]
        [SerializeField] private RectTransformVar _target;

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

        public override bool CanStart() => CheckParameters(_rectTransform, _target, _maxSpeed);
        public override bool CanExecute() => CheckParameters(_rectTransform, _maxSpeed);

        public override void OnStart() => _smoother.Reset();

        public override void Execute()
        {
            var target = _target.Value;
            if (target == null)
            {
                Finish();
                return;
            }

            var rectTransform = _rectTransform.Value;
            var parent = rectTransform.parent;
            if (parent == null)
            {
                LogError("The moving RectTransform needs a parent to convert the target position.");
                Finish();
                return;
            }

            Vector2 targetLocalPosition;
            var movingCanvas = rectTransform.GetComponentInParent<Canvas>();
            var targetCanvas = target.GetComponentInParent<Canvas>();
            var movingRootCanvas = movingCanvas != null ? movingCanvas.rootCanvas : null;
            var targetRootCanvas = targetCanvas != null ? targetCanvas.rootCanvas : null;

            if (movingRootCanvas != null && targetRootCanvas != null &&
                movingRootCanvas != targetRootCanvas &&
                (movingRootCanvas.renderMode != RenderMode.WorldSpace || targetRootCanvas.renderMode != RenderMode.WorldSpace))
            {
                var parentRectTransform = parent as RectTransform;
                var targetCamera = GetCanvasCamera(targetRootCanvas);
                var movingCamera = GetCanvasCamera(movingRootCanvas);
                if (parentRectTransform == null ||
                    (targetRootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && targetCamera == null) ||
                    (movingRootCanvas.renderMode != RenderMode.ScreenSpaceOverlay && movingCamera == null) ||
                    !RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        parentRectTransform,
                        RectTransformUtility.WorldToScreenPoint(targetCamera, target.position),
                        movingCamera,
                        out targetLocalPosition))
                {
                    LogError("Cannot convert the target position between these canvases. Check their cameras and the moving RectTransform's parent.");
                    Finish();
                    return;
                }
            }
            else
            {
                var targetLocal = parent.InverseTransformPoint(target.position);
                targetLocalPosition = new Vector2(targetLocal.x, targetLocal.y);
            }

            var current = rectTransform.anchoredPosition;
            var currentLocal = rectTransform.localPosition;
            var targetPosition = current + targetLocalPosition - new Vector2(currentLocal.x, currentLocal.y);
            var next = _smoother.Update(
                MoveAxis.XY,
                new Vector3(current.x, current.y, 0f),
                new Vector3(targetPosition.x, targetPosition.y, 0f),
                _smoothTime.Value,
                _maxSpeed.Value);

            rectTransform.anchoredPosition = new Vector2(next.x, next.y);

            var finishDistance = _finishDistance.Value;
            if (finishDistance < 0f) return;

            _distanceToTarget = Vector2.Distance(rectTransform.anchoredPosition, targetPosition);
            if (_distanceToTarget <= finishDistance)
            {
                SendEvent(_finishedEvent);
                Finish();
            }
        }

        public override string GetSummary() =>
            "Move {_rectTransform} towards {_target} pivot" +
            (_smoothTime.IsNotDefault() ? " in {_smoothTime}s" : "") +
            (_maxSpeed.IsVariable || _maxSpeed.Value > 0f ? " at {_maxSpeed}/s" : "") +
            (_finishDistance.Value >= 0f ? " until <= {_finishDistance}" : "") +
            (_finishedEvent.IsSet ? " {_finishedEvent}" : "");

        private static Camera GetCanvasCamera(Canvas canvas) =>
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera != null ? canvas.worldCamera : Camera.main;

#if UNITY_EDITOR
        public override bool HasDebugInfo => true;
        public override string GetDebugInfo() => $"Distance: {_distanceToTarget:0.##}";
#endif
    }
}
