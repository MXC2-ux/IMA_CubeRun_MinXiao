using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.Tween)]
    [ActionDescription("Tween the field of view of a Camera Component, in degrees.")]
    public class TweenCameraFov : BaseTweenAction
    {
        [Tooltip("The Camera.")]
        [SerializeField]
        private CameraVar _camera;

        [Tooltip("Tween from or to the given value.")]
        [SerializeField]
        private TweenDirection _direction;

        [Tooltip("The field of view to tween from/to, in degrees.")]
        [SerializeField]
        [DefaultValue(60f)]
        private FloatVar _fieldOfView;

        [NonSerialized] private float _fromValue;
        [NonSerialized] private float _toValue;

        public override bool CanExecute() => CheckParameters(_camera, _fieldOfView) && base.CanExecute();

        public override void OnStart()
        {
            base.OnStart();

            if (_direction == TweenDirection.To)
            {
                _fromValue = _camera.Value.fieldOfView;
                _toValue = _fieldOfView.Value;
            }
            else
            {
                _fromValue = _fieldOfView.Value;
                _toValue = _camera.Value.fieldOfView;
            }

            Distance = Mathf.Abs(_toValue - _fromValue);
        }

        public override void Execute()
        {
            base.Execute();

            _camera.Value.fieldOfView = Mathf.LerpUnclamped(_fromValue, _toValue, Easing.Evaluate(Progress));
        }

        public override string GetSummary() => "Tween {_camera} FOV {_direction} {_fieldOfView}" + base.GetSummary();
    }
}
