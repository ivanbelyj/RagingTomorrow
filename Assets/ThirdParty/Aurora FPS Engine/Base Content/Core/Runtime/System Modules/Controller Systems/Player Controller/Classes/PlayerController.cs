/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class PlayerController : ActorController, IPlayerCamera, IPlayerState
    {
        public enum DefaultSpeed
        {
            Walk,
            Run
        }

        /// <summary>
        /// First person controller sprint directions.
        /// </summary>
        [Flags]
        public enum AccelerationDirection
        {
            /// <summary>
            /// Sprint movement disabled.
            /// </summary>
            Disabled = 0,

            /// <summary>
            ///  Sprint movement available only on forward direction.
            /// </summary>
            Forward = 1 << 0,

            /// <summary>
            /// Sprint movement available only on side directions.
            /// </summary>
            Side = 1 << 1,

            /// <summary>
            /// Sprint movement available only on backward direction.
            /// </summary>
            Backword = 1 << 2,

            /// <summary>
            /// Sprint movement available on any directions.
            /// </summary>
            All = ~0
        }

        [SerializeField]
        [Order(-999)]
        private DefaultSpeed defaultSpeed = DefaultSpeed.Run;

        [SerializeField]
        [NotNull(Format = "Add one of implementation of player camera and attach it here.")]
        [Order(-998)]
        private PlayerCamera playerCamera;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-398)]
        private float runSpeed = 7.5f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-397)]
        private float sprintSpeed = 8.5f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-396)]
        private float crouchSpeed = 3.25f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-395)]
        private float zoomSpeed = 4.0f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-394)]
        private float crouchZoomSpeed = 2.5f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Label("Backwards Speed")]
        [VisualClamp(0, 1)]
        [Suffix("%", true)]
        [Order(-393)]
        private float backwardSpeedPercent = 0.5f;

        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Label("Side Speed")]
        [VisualClamp(0, 1)]
        [Suffix("%", true)]
        [Order(-392)]
        private float sideSpeedPercent = 0.75f;

        [SerializeField]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(-299)]
        private AccelerationDirection accelerationDirection = AccelerationDirection.Forward;

        [SerializeField]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(-298)]
        private float runVelocityThreshold = 0.0f;

        [SerializeField]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(-297)]
        private float sprintVelocityThreshold = 0.0f;

        [SerializeField]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(-296)]
        private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        // Stored required properties.
        private bool isSprint;
        private bool isLightWalk;
        private ControllerState controllerState = ControllerState.Idle;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            Debug.Assert(playerCamera != null, $"<b><color=#FF0000>Attach reference of player camera to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Player Camera<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            CalculateState();
        }

        /// <summary>
        /// Calculate speed of the controller.
        /// </summary>
        protected override float CalculateSpeed()
        {
            float speed = base.CalculateSpeed();
            if(GetControlInput() != Vector2.zero)
            {
                switch (defaultSpeed)
                {
                    case DefaultSpeed.Run:
                        speed = isLightWalk ? speed : runSpeed;
                        speed = isSprint && CanAccelerate() ? sprintSpeed : speed;
                        break;
                    case DefaultSpeed.Walk:
                        speed = isSprint && CanAccelerate() ? runSpeed : GetWalkSpeed();
                        break;
                }
                speed = IsCrouched() ? crouchSpeed : speed;
                speed = playerCamera.IsZooming() ? zoomSpeed : speed;
                speed = playerCamera.IsZooming() && IsCrouched() ? crouchZoomSpeed : speed;
                speed = GetControlInput().y < 0 ? speed * backwardSpeedPercent : speed;
                speed = GetControlInput().x != 0 && GetControlInput().y == 0 ? speed * sideSpeedPercent : speed;
            }

            return speed;
        }

        /// <summary>
        /// Smoothing movement direction Vector3.
        /// </summary>
        /// <param name="value">Reference value of smoothed direction.</param>
        /// <param name="smoothTime">Interpolation smooth value.</param>
        protected override void SmoothingSpeed(ref float value, float smoothTime)
        {
            base.SmoothingSpeed(ref value, smoothTime);
            if (isSprint && CanAccelerate())
            {
                float accelerateSpeed = defaultSpeed == DefaultSpeed.Run ? sprintSpeed : runSpeed;
                float acceleratePersent = Mathf.InverseLerp(GetSpeed(), accelerateSpeed, value);
                value = accelerationCurve.Evaluate(acceleratePersent) * (accelerateSpeed - GetSpeed()) + GetSpeed();
            }
        }

        /// <summary>
        /// Called when controller become enabled.
        /// </summary>
        public override void RegisterInputActions()
        {
            base.RegisterInputActions();

            // Sprint action callback.
            InputReceiver.SprintAction.performed += OnSprintAction;
            InputReceiver.SprintAction.canceled += OnSprintAction;

            // Light walk action callback.
            InputReceiver.LightWalkAction.performed += OnLightWalkAction;
            InputReceiver.LightWalkAction.canceled += OnLightWalkAction;
        }

        /// <summary>
        /// Called when controller disabled.
        /// </summary>
        public override void RemoveInputActions()
        {
            base.RemoveInputActions();
            // Sprint action callback.
            InputReceiver.SprintAction.performed -= OnSprintAction;
            InputReceiver.SprintAction.canceled -= OnSprintAction;

            // Light walk action callback.
            InputReceiver.LightWalkAction.performed -= OnLightWalkAction;
            InputReceiver.LightWalkAction.canceled -= OnLightWalkAction;
        }


        /// <summary>
        /// Calculate controller state.
        /// </summary>
        protected virtual void CalculateState()
        {
            float currentSpeed = GetSpeed();

            if (currentSpeed != 0 && IsGrounded())
            {
                controllerState &= ~ControllerState.Idle;
                controllerState &= ~ControllerState.InAir;

                if (currentSpeed == GetWalkSpeed() ||
                    currentSpeed == GetWalkSpeed() * backwardSpeedPercent ||
                    currentSpeed == GetWalkSpeed() * sideSpeedPercent ||
                    currentSpeed == crouchSpeed ||
                    currentSpeed == crouchSpeed * backwardSpeedPercent ||
                    currentSpeed == crouchSpeed * sideSpeedPercent ||
                    currentSpeed == zoomSpeed ||
                    currentSpeed == zoomSpeed * backwardSpeedPercent ||
                    currentSpeed == zoomSpeed * sideSpeedPercent)
                    controllerState |= ControllerState.Walking;
                else
                    controllerState &= ~ControllerState.Walking;

                if (currentSpeed == runSpeed ||
                    currentSpeed == runSpeed * backwardSpeedPercent ||
                    currentSpeed == runSpeed * sideSpeedPercent)
                    controllerState |= ControllerState.Running;
                else
                    controllerState &= ~ControllerState.Running;

                if (CanAccelerate() && currentSpeed == sprintSpeed ||
                    currentSpeed == sprintSpeed * backwardSpeedPercent ||
                    currentSpeed == sprintSpeed * sideSpeedPercent)
                    controllerState |= ControllerState.Sprinting;
                else
                    controllerState &= ~ControllerState.Sprinting;
            }
            else if (IsGrounded())
            {
                controllerState = ControllerState.Idle;
            }
            else
            {
                controllerState = ControllerState.InAir;
            }

            if (IsCrouched())
                controllerState |= ControllerState.Crouched;
            else
                controllerState &= ~ControllerState.Crouched;

            if (IsJumped())
                controllerState |= ControllerState.Jumped;
            else
                controllerState &= ~ControllerState.Jumped;

            if (playerCamera.IsZooming())
                controllerState |= ControllerState.Zooming;
            else
                controllerState &= ~ControllerState.Zooming;
        }

        /// <summary>
        /// Checks whether the controller can accelerate at the current moment.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAccelerate()
        {
            float threshold = defaultSpeed == DefaultSpeed.Run ? sprintVelocityThreshold : runVelocityThreshold;
            return threshold <= GetVelocity().sqrMagnitude && !IsCrouched() && !playerCamera.IsZooming() && CheckAccelerationDirection();
        }

        /// <summary>
        /// Check that acceleration direction is correct.
        /// </summary>
        /// <returns>True, if the current direction satisfies the direction in which controller can accelerate. 
        /// Otherwise false.</returns>
        public bool CheckAccelerationDirection()
        {
            Vector2 controlInput = GetControlInput();
            if (accelerationDirection == AccelerationDirection.All)
                return true;
            else if (accelerationDirection == AccelerationDirection.Forward && (controlInput.y > 0 && controlInput.x == 0))
                return true;
            else if (accelerationDirection == AccelerationDirection.Backword && (controlInput.y < 0 && controlInput.x == 0))
                return true;
            else if (accelerationDirection == AccelerationDirection.Side && (controlInput.x != 0 && controlInput.y == 0))
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Forward | AccelerationDirection.Backword)) && (controlInput.y != 0 && controlInput.x == 0))
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Forward | AccelerationDirection.Side)) && controlInput.y > 0)
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Backword | AccelerationDirection.Side)) && controlInput.y < 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Breaks sprint
        /// </summary>
        public void BreakSprint()
        {
            isSprint = false;
        }

        #region [Input Action Wrapper]
        private void OnLightWalkAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                isLightWalk = true;
            else if (context.canceled)
                isLightWalk = false;
        }

        private void OnSprintAction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isSprint = true;
                Crouch(false);
            }
            else if (context.canceled)
            {
                isSprint = false;
            }
        }
        #endregion

        #region [Getter / Setter]
        public DefaultSpeed GetMovementType()
        {
            return defaultSpeed;
        }

        public void SetMovementType(DefaultSpeed value)
        {
            defaultSpeed = value;
        }

        public float GetRunSpeed()
        {
            return runSpeed;
        }

        public void SetRunSpeed(float value)
        {
            runSpeed = value;
        }

        public float GetSprintSpeed()
        {
            return sprintSpeed;
        }

        public void SetSprintSpeed(float value)
        {
            sprintSpeed = value;
        }

        public float GetCrouchSpeed()
        {
            return crouchSpeed;
        }

        public void SetCrouchSpeed(float value)
        {
            crouchSpeed = value;
        }

        public float GetZoomSpeed()
        {
            return zoomSpeed;
        }

        public void SetZoomSpeed(float value)
        {
            zoomSpeed = value;
        }

        public float GetCrouchZoomSpeed()
        {
            return crouchZoomSpeed;
        }

        public void SetCrouchZoomSpeed(float value)
        {
            crouchZoomSpeed = value;
        }

        public float GetBackwardSpeedPercent()
        {
            return backwardSpeedPercent;
        }

        public void SetBackwardSpeedPercent(float value)
        {
            backwardSpeedPercent = value;
        }

        public float GetSideSpeedPercent()
        {
            return sideSpeedPercent;
        }

        public void SetSideSpeedPercent(float value)
        {
            sideSpeedPercent = value;
        }

        public AccelerationDirection GetAccelerationDirection()
        {
            return accelerationDirection;
        }

        public void SetAccelerationDirection(AccelerationDirection value)
        {
            accelerationDirection = value;
        }

        public float GetRunVelocityThreshold()
        {
            return runVelocityThreshold;
        }

        public void SetRunVelocityThreshold(float value)
        {
            runVelocityThreshold = value;
        }

        public float GetSprintVelocityThreshold()
        {
            return sprintVelocityThreshold;
        }

        public void SetSprintVelocityThreshold(float value)
        {
            sprintVelocityThreshold = value;
        }

        public AnimationCurve GetAccelerationCurve()
        {
            return accelerationCurve;
        }

        public void SetAccelerationCurve(AnimationCurve value)
        {
            accelerationCurve = value;
        }

        public ControllerState GetState()
        {
            return controllerState;
        }

        public bool HasState(ControllerState state)
        {
            return (controllerState & state) != 0;
        }

        public bool CompareState(ControllerState state)
        {
            return controllerState == state;
        }

        public PlayerCamera GetPlayerCamera()
        {
            return playerCamera;
        }

        public void SetPlayerCamera(PlayerCamera value)
        {
            playerCamera = value;
        }
        #endregion
    }
}
