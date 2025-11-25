/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [System.Obsolete("Use Controller from AuroraFPSRuntime.SystemModules.ControllerSystem instead.")]
    public abstract class Controller : MonoBehaviour, IController, IControllerMovement, IControllerCamera, IControllerSpeed, IControllerJump, IControllerState, IControllerInput, IControllerEnabled, IControllerGrounded, IControllerCrouch, IControllerPhysics, IControllerCallbacks
    {
        public enum LocomotionType
        {
            Movement,
            Custom
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            GetCameraControl().Initialize(this);
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            GetCameraControl().Internal_Update();
        }

        public abstract bool IsMoving();

        #region [IController Implementation]
        public abstract Vector3 GetVelocity();

        public abstract void SetLocomotionType(LocomotionType locomotionType);
        #endregion

        #region [IControllerMovement Implementation]
        public abstract Vector3 GetMovementDirection();
        #endregion

        #region [IControllerCamera Implementation]
        public abstract FPCameraControl GetCameraControl();
        #endregion

        #region [IcontrollerSpeed Implementation]
        public abstract float GetSpeed();
        public abstract float GetWalkSpeed();
        public abstract float GetRunSpeed();
        public abstract float GetSprintSpeed();
        public abstract float GetBackwardSpeedPerсent();
        public abstract float GetSideSpeedPerсent();
        public abstract void SetWalkSpeed(float value);
        public abstract void SetRunSpeed(float value);
        public abstract void SetSprintSpeed(float value);
        public abstract void SetBackwardSpeedPerсent(float value);
        public abstract void SetSideSpeedPerсent(float value);
        #endregion

        #region [IControllerJump Implementation]
        public abstract void MakeJump(float force);

        public abstract void MakeJump(float force, Vector3 direction);

        public abstract void MakeJump(float force, Vector3 direction, bool saveVelocity);

        public abstract bool IsJumped();
        #endregion

        #region [ControllerState Implementation]
        public abstract ControllerState GetState();
        public abstract bool CompareState(ControllerState value);
        public abstract bool HasState(ControllerState value);
        #endregion

        #region [IControllerInput Implementation]
        public abstract Vector2 GetMovementInput();
        public abstract float GetHorizontalInput();
        public abstract float GetVerticalInput();
        #endregion

        #region [IControllerEnabled Implementation]
        public abstract bool IsEnabled();
        public abstract void SetEnabled(bool enabled);
        #endregion

        #region [IControllerGrounded Implementation]
        public abstract bool IsGrounded();
        #endregion

        #region [IControllerCrouch Implementation]
        public abstract void Crouch(bool value);

        public abstract bool IsCrouched();

        public abstract float GetCrouchSpeed();
        #endregion

        #region [IControllerPhysics Implementation]
        public abstract bool AirControl();

        public abstract void AirControl(bool value);

        public abstract float GetGravityMultiplier();
        #endregion

        #region [IControllerCallbacks Implementation]
        public abstract event Action<Vector3> OnMoveCallback;
        public abstract event Action OnGroundedCallback;
        public abstract event Action OnJumpCallback;
        public abstract event Action OnBecomeAirCallback;
        public abstract event Action OnEnableCallback;
        public abstract event Action OnDisableCallback;
        #endregion
    }
}