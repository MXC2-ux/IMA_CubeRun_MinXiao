using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace HutongGames.PlayMaker.Actions
{
    [PublicAPI, Serializable]
    [ActionCategory(Category.CharacterController)]
    [ConvertibleGroup("CharacterControllerMove")]
    [ActionDescription("Control a CharacterController while in the air (not grounded) and call Move each frame. " +
                       "<br/>This action pairs best with CharacterControllerCheckIsGrounded or CharacterControllerCheckIsGrounded (SphereCast).")]
    public class CharacterControllerMoveInAir : BaseAction
    {
        public enum AirMotionMode
        {
            TargetVelocity,
            Accelerate,
            DirectSet,
            Preserve
        }

        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;
        public override UpdateMode RequiredUpdateModes => UpdateMode.EveryFrame;
        
        [RequiredComponent(typeof(CharacterController))]
        [Tooltip("The CharacterController")]
        [SerializeField]
        private CharacterControllerVar _characterController;
        
        [OptionalField]
        [FormerlySerializedAs("_motion")]
        [Tooltip("Optional initial velocity for the in-air state. " +
                 "If set, X/Y/Z seed the airborne velocity when entering the state.")]
        [SerializeField]
        private Vector3Ref _initialVelocity;
        
        [OptionalField]
        [HideIf(nameof(HideMoveVector))]
        [Tooltip("Horizontal input used while in the air. " +
                 "Usually from the same move vector used for grounded movement. " +
                 "X and Z set the target velocity, or the acceleration direction and speed limit in Accelerate mode.")]
        [SerializeField]
        private Vector3Ref _moveVector;
        
        [OptionalField]
        [HideIf(nameof(HideSpeedMultiplier))]
        [Tooltip("Multiplies the Move Vector by a Speed factor.")]
        [SerializeField, DefaultValue(1f)]
        private FloatVar _speedMultiplier;
        
        [Tooltip("Move in local or word space.")]
        [SerializeField]
        private SpaceVar _space;

        [Tooltip("How the Move Vector affects horizontal airborne velocity.\n" +
                 "Target Velocity: Steer toward the Move Vector at the Horizontal Acceleration rate.\n" +
                 "Accelerate: Steer in the Move Vector direction up to its speed, keeping momentum when input stops.\n" +
                 "Direct Set: Replace horizontal velocity with the Move Vector each frame.\n" +
                 "Preserve: Keep the current horizontal velocity and ignore the Move Vector.")]
        [SerializeField]
        private AirMotionMode _motionMode;

        [OptionalField]
        [HideIf(nameof(HideHorizontalAcceleration))]
        [FormerlySerializedAs("_airControl")]
        [Tooltip("Horizontal acceleration used in Target Velocity and Accelerate modes. " +
                 "This is the maximum change in horizontal velocity per second, in units/s^2. " +
                 "Higher values steer faster. Lower values preserve more airborne inertia.")]
        [SerializeField, DefaultValue(10f)]
        private FloatVar _horizontalAcceleration;
        
        [Tooltip("Multiply the gravity applied to the CharacterController.")]
        [SerializeField, DefaultValue(1f)]
        private FloatVar _gravityMultiplier;
        
        [Tooltip("Extra gravity multiplier when falling. " +
                 "Note: This is on top of the gravity multiplier above. " +
                 "This can be used to make jumps less 'floaty.'")]
        [SerializeField, DefaultValue(1f)]
        private FloatVar _fallMultiplier;
        
        [OptionalField]
        [Tooltip("Indicates the direction of a collision: None, Sides, Above, and Below.")]
        [SerializeField]
        [WriteOnly]
        private CollisionFlagsRef _collisionFlags;
        
        [OptionalField]
        [Tooltip("Event to send when landing. Use this to transition back to a grounded State.")]
        [SerializeField]
        private EventRef _landedEvent;

        [NonSerialized] private Vector3 _startJumpPosition;
        [NonSerialized] private Vector3 _totalJumpMovement;
        [NonSerialized] private Vector3 _currentVelocity;
        [NonSerialized] private Transform _transform;

        private bool HideMoveVector => _motionMode == AirMotionMode.Preserve;
        private bool HideSpeedMultiplier => _motionMode == AirMotionMode.Preserve;
        private bool HideHorizontalAcceleration => _motionMode != AirMotionMode.TargetVelocity && _motionMode != AirMotionMode.Accelerate;

        public override bool CanExecute() => 
            CheckParameters(_characterController, _space, _gravityMultiplier, _fallMultiplier);

        public override void OnStart()
        {
            var controller = _characterController.Value;
            _transform = controller.transform;
            _currentVelocity = _initialVelocity.IsNone ? controller.velocity : _initialVelocity.Value;

            if (_space.Value == Space.Self && _initialVelocity.IsNone)
            {
                _currentVelocity = _transform.InverseTransformDirection(_currentVelocity);
            }
        }

        public override void Execute()
        {
            var controller = _characterController.Value;

            if (_moveVector.IsAssigned)
            {
                var inAirMove = _moveVector.Value;
                if (!_speedMultiplier.IsNone)
                {
                    inAirMove *= _speedMultiplier.Value;
                }

                switch (_motionMode)
                {
                    case AirMotionMode.TargetVelocity:
                        var horizontalAcceleration = _horizontalAcceleration.IsNone ? 10f : _horizontalAcceleration.Value;
                        _currentVelocity.x = Mathf.MoveTowards(_currentVelocity.x, inAirMove.x, horizontalAcceleration * Time.deltaTime);
                        _currentVelocity.z = Mathf.MoveTowards(_currentVelocity.z, inAirMove.z, horizontalAcceleration * Time.deltaTime);
                        break;

                    case AirMotionMode.Accelerate:
                        var input = new Vector2(inAirMove.x, inAirMove.z);
                        if (input.sqrMagnitude > 0f)
                        {
                            var inputMagnitude = input.magnitude;
                            var velocity = new Vector2(_currentVelocity.x, _currentVelocity.z);
                            var maxSpeed = Mathf.Max(velocity.magnitude, inputMagnitude);
                            var acceleration = _horizontalAcceleration.IsNone ? 10f : _horizontalAcceleration.Value;
                            velocity += input / inputMagnitude *
                                        (Mathf.Max(0f, acceleration) * Mathf.Min(1f, inputMagnitude) * Time.deltaTime);
                            velocity = Vector2.ClampMagnitude(velocity, maxSpeed);
                            _currentVelocity.x = velocity.x;
                            _currentVelocity.z = velocity.y;
                        }
                        break;

                    case AirMotionMode.DirectSet:
                        _currentVelocity.x = inAirMove.x;
                        _currentVelocity.z = inAirMove.z;
                        break;

                    case AirMotionMode.Preserve:
                        break;
                }
            }

            var gravity = Physics.gravity.y * _gravityMultiplier.Value * (_currentVelocity.y < 0 ? _fallMultiplier.Value : 1);
            _currentVelocity.y += gravity * Time.deltaTime;

            var move = _currentVelocity;

            if (_space.Value == Space.Self)
            {
                move = _transform.TransformDirection(move);
            }

            _collisionFlags.Value = controller.Move(move * Time.deltaTime);
            
            if (controller.isGrounded && controller.velocity.y < 0.1f)
            {
                controller.Move(Vector3.zero);
                
                SendEvent(_landedEvent);
            }
        }

        public override string GetSummary() => 
            "Move {_characterController} in air {_moveVector}"
            + (Mathf.Approximately(_speedMultiplier.Value,1) ? "" : " x {_speedMultiplier}")
            + (_landedEvent.IsSet ? " {_landedEvent}" : "");
    }
}
