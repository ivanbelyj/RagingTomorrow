/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class MovableController : PawnController, IKinematic, IRestorable
    {
        [SerializeField]
        [Foldout("Locomotion Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(-399)]
        private float walkSpeed = 8.0f;

        [SerializeField]
        [Foldout("Check Wall Settings", Style = "Header")]
        [Order(590)]
        private LayerMask wallCullingLayer = 1 << 0;

        [SerializeField]
        [Foldout("Check Wall Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(591)]
        private float wallCheckRange = 0.1f;

        [SerializeField]
        [Foldout("Check Wall Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(592)]
        private float wallCheckRadius = 0.1f;

        [SerializeField]
        [Foldout("Gravity Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(550)]
        private float gravityMultiplier = 2.25f;

        [SerializeField]
        [Foldout("Gravity Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(551)]
        private float stickGroundForce = 0.75f;

        [SerializeField]
        [Foldout("Smoothness Settings", Style = "Header")]
        [Order(126)]
        private bool smoothControlInput = true;

        [SerializeField]
        [Label("Value")]
        [Slider(1f, 100.0f)]
        [Foldout("Smoothness Settings", Style = "Header")]
        [VisibleIf("smoothControlInput")]
        [Order(1000)]
        [Indent(1)]
        private float smoothControlInputValue = 25.0f;

        [SerializeField]
        [Foldout("Smoothness Settings", Style = "Header")]
        [Order(1001)]
        private bool smoothVelocity = true;

        [SerializeField]
        [Label("Value")]
        [Slider(1f, 100.0f)]
        [Foldout("Smoothness Settings", Style = "Header")]
        [VisibleIf("smoothVelocity")]
        [Order(1002)]
        [Indent(1)]
        private float smoothVelocityValue = 20.0f;

        [SerializeField]
        [Foldout("Smoothness Settings", Style = "Header")]
        [Order(1003)]
        private bool smoothSpeed = true;

        [SerializeField]
        [Label("Value")]
        [Slider(1f, 100.0f)]
        [Foldout("Smoothness Settings", Style = "Header")]
        [VisibleIf("smoothSpeed")]
        [Order(1004)]
        [Indent(1)]
        private float smoothSpeedValue = 20.0f;

        [SerializeField]
        [Foldout("Smoothness Settings", Style = "Header")]
        [Order(1004)]
        private bool smoothDirection = true;

        [SerializeField]
        [Label("Value")]
        [Slider(1f, 100.0f)]
        [Foldout("Smoothness Settings", Style = "Header")]
        [VisibleIf("smoothDirection")]
        [Order(1005)]
        [Indent(1)]
        private float smoothDirectionValue = 35.0f;

        [SerializeField]
        [Foldout("Smoothness Settings", Style = "Header")]
        [Order(1006)]
        private bool smoothInputMagnitude = true;

        [SerializeField]
        [Label("Value")]
        [Slider(1f, 100.0f)]
        [Foldout("Smoothness Settings", Style = "Header")]
        [VisibleIf("smoothInputMagnitude")]
        [Order(1007)]
        [Indent(1)]
        private float smoothInputMagnitudeValue = 10.0f;

        // Stored required properties.
        private bool isKinematic;
        private bool hasWallObstacle;
        private float speed;
        private float smoothedSpeed;
        private float smoothedInputMagnitude;
        private Vector3 direction;
        private Vector3 movementVector;
        private Vector2 smoothedControlInput;
        private Vector3 smoothedDirection;
        private RaycastHit wallInfo;

        /// <summary>
        /// Called every frame, while the controller is enabled.
        /// </summary>
        protected override void Update()
        {
            base.Update();
            hasWallObstacle = CheckWallObstacle(out wallInfo);
            if (!isKinematic && GetUpdateOrder() == UpdateOrder.Update)
            {
                ApplyMovement();
            }
        }

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (!isKinematic && GetUpdateOrder() == UpdateOrder.FixedUpdate)
            {
                ApplyMovement();
            }
        }

        #region [Abstract Methods]
        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        /// <param name="velocity">Movement velocity.</param>
        protected abstract void Move(Vector3 velocity);

        /// <summary>
        /// Returns the motion update mode.
        /// </summary>
        protected abstract UpdateOrder GetUpdateOrder();
        #endregion

        /// <summary>
        /// Performs calculation and application of the motion vector for the controller.
        /// </summary>
        private void ApplyMovement()
        {
            if (smoothControlInput)
                SmoothingInput(ref smoothedControlInput, smoothControlInputValue);
            else
                smoothedControlInput = GetControlInput().normalized;

            if (smoothSpeed)
                SmoothingSpeed(ref smoothedSpeed, smoothSpeedValue);
            else
                smoothedSpeed = speed;

            if (smoothDirection)
                SmoothingDirection(ref smoothedDirection, smoothDirectionValue);
            else
                smoothedDirection = direction;

            if (smoothInputMagnitude)
                SmoothingInputMagnitude(ref smoothedInputMagnitude, smoothInputMagnitudeValue);
            else
                smoothedInputMagnitude = GetControlInput().normalized.magnitude;

            speed = CalculateSpeed();
            direction = CalculateDirection(smoothedControlInput);
            movementVector = CalculateMovement(smoothedDirection, smoothedSpeed, smoothedInputMagnitude);

            if (IsGrounded())
            {
                SnapToGround(ref movementVector);
            }

            ApplyGravity(ref movementVector);

            Move(movementVector);

            if (GetVelocity() != Vector3.zero)
                OnMoveCallback?.Invoke(movementVector);
        }

        /// <summary>
        /// Applying physics gravity to controller.
        /// </summary>
        /// <param name="movementVector">Controller movement vector.</param>
        protected virtual void ApplyGravity(ref Vector3 movementVector)
        {
            if (!IsGrounded())
            {
                movementVector += GetGravityDirection() * gravityMultiplier * Time.deltaTime;
            }
        }

        /// <summary>
        /// Keep snapping controller to ground, while is grounded.
        /// </summary>
        /// <param name="movementVector">Controller movement vector.</param>
        protected virtual void SnapToGround(ref Vector3 movementVector)
        {
            movementVector += GetGravityDirection().normalized * stickGroundForce;
        }

        /// <summary>
        /// Calculate movement controller speed.
        /// </summary>
        /// <returns>Movement speed.</returns>
        protected virtual float CalculateSpeed()
        {
            if (GetControlInput() == Vector2.zero)
            {
                return 0.0f;
            }
            else
            {
                return walkSpeed;
            }
        }

        /// <summary>
        /// Calculate movement direction.
        /// </summary>
        /// <param name="controlInput">Control input in Vector2D representation.</param>
        /// <returns>Movement direction</returns>
        protected virtual Vector3 CalculateDirection(Vector2 controlInput)
        {
            Vector3 verticalDirection = GetForwardDirection() * controlInput.y;
            Vector3 horizontalDirection = GetRightDirection() * controlInput.x;
            return FlattenSlopes(verticalDirection + horizontalDirection);
        }

        /// <summary>
        /// Calculate controller movement vector.
        /// </summary>
        /// <param name="direction">Current movement direction.</param>
        /// <param name="speed">Current movement speed.</param>
        /// <param name="inputMagnitude">Input magnitude.</param>
        /// <returns>Controller movement vector.</returns>
        protected virtual Vector3 CalculateMovement(Vector3 direction, float speed, float inputMagnitude)
        {
            if (IsGrounded() && !hasWallObstacle)
            {
                return inputMagnitude * speed * direction;
            }
            return movementVector;
        }

        /// <summary>
        /// Smoothing control input in Vector2D representation.
        /// <br><i>Base implementation: Linear interpolation value to normalized control input.</i></br>
        /// </summary>
        /// <param name="value">Reference value of smoothed control input.</param>
        /// <param name="smoothTime">Interpolation smooth value.</param>
        protected virtual void SmoothingInput(ref Vector2 value, float smoothTime)
        {
            value = Vector2.Lerp(value, GetControlInput().normalized, Time.deltaTime * smoothTime);
        }

        /// <summary>
        /// Smoothing movement direction Vector3.
        /// <br><i>Base implementation: Linear interpolation value to movement direction.</i></br>
        /// </summary>
        /// <param name="value">Reference value of smoothed direction.</param>
        /// <param name="smoothTime">Interpolation smooth value.</param>
        protected virtual void SmoothingSpeed(ref float value, float smoothTime)
        {
            value = Mathf.Lerp(value, speed, Time.deltaTime * smoothTime);
        }

        /// <summary>
        /// Smoothing movement direction Vector3.
        /// <br><i>Base implementation: Linear interpolation value to movement direction.</i></br>
        /// </summary>
        /// <param name="value">Reference value of smoothed direction.</param>
        /// <param name="smoothTime">Interpolation smooth value.</param>
        protected virtual void SmoothingDirection(ref Vector3 value, float smoothTime)
        {
            value = Vector3.Lerp(value, direction, Time.deltaTime * smoothTime);
        }

        /// <summary>
        /// Smoothing input magnitude value.
        /// <br><i>Base implementation: Linear interpolation value to control input magnitude.</i></br>
        /// </summary>
        /// <param name="value">Reference value of smoothed magnitude.</param>
        /// <param name="smoothTime">Interpolation smooth value.</param>
        protected virtual void SmoothingInputMagnitude(ref float value, float smoothTime)
        {
            value = Mathf.Lerp(value, GetControlInput().normalized.magnitude, Time.deltaTime * smoothTime);
        }

        /// <summary>
        /// Restore controller to default.
        /// </summary>
        public virtual void Restore()
        {
            const float ZERO_FLOAT = 0.0f;

            speed = ZERO_FLOAT;
            smoothedSpeed = ZERO_FLOAT;

            direction = Vector3.zero;
            smoothedDirection = Vector3.zero;

            smoothedControlInput = Vector3.zero;
            smoothedInputMagnitude = ZERO_FLOAT;

            movementVector = Vector3.zero;
        }

        /// <summary>
        /// Checks whether there is an obstacle in the form of a wall in move direction of the controller.
        /// </summary>
        /// <param name="origin">Transform position.</param>
        private bool CheckWallObstacle(out RaycastHit hitInfo)
        {
            CopyBounds(out Vector3 center, out float height);
            Vector3 origin = transform.TransformPoint(center);
            if (GetControlInput() != Vector2.zero && direction.sqrMagnitude > 0)
            {
                return Physics.SphereCast(origin, wallCheckRadius, direction, out hitInfo, wallCheckRange, wallCullingLayer);
            }
            hitInfo = new RaycastHit();
            return false;
        }

        /// <summary>
        /// Current controller movement speed raw.
        /// </summary>
        public float GetSpeed()
        {
            return speed;
        }

        /// <summary>
        /// Current movement direction raw.
        /// </summary>
        public Vector3 GetMovementDirection()
        {
            return direction;
        }

        /// <summary>
        /// Direction of gravity on the controller.
        /// </summary>
        public virtual Vector3 GetGravityDirection()
        {
            return Physics.gravity;
        }

        /// <summary>
        /// Normalized forward direction of controller.
        /// </summary>
        public virtual Vector3 GetForwardDirection()
        {
            return transform.forward;
        }

        /// <summary>
        /// Normalized right direction of controller.
        /// </summary>
        public virtual Vector3 GetRightDirection()
        {
            return transform.right;
        }

        /// <summary>
        /// Controller is currently on the move.
        /// </summary>
        public virtual bool IsMoving()
        {
            return !isKinematic && !hasWallObstacle && IsGrounded() && GetControlInput() != Vector2.zero;
        }

        /// <summary>
        /// Is there a wall in front of the controller at the moment.
        /// </summary>
        public bool HasWallObstacle()
        {
            return hasWallObstacle;
        }

        #region [IKinematic Implementation]
        public void IsKinematic(bool value)
        {
            if (isKinematic != value)
            {
                isKinematic = value;
                OnKinematicCallback?.Invoke(isKinematic);
            }
        }

        public bool IsKinematic()
        {
            return isKinematic;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when the controller is moving.
        /// </summary>
        public event Action<Vector3> OnMoveCallback;

        /// <summary>
        /// Called when kinematic state is changed.
        /// </summary>
        public event Action<bool> OnKinematicCallback;
        #endregion

        #region [Getter / Setter]
        public Vector3 GetMovementVector()
        {
            return movementVector;
        }
        public float GetWalkSpeed()
        {
            return walkSpeed;
        }

        public void SetWalkSpeed(float value)
        {
            walkSpeed = value;
        }

        public LayerMask GetWallCullingLayer()
        {
            return wallCullingLayer;
        }

        public void SetWallCullingLayer(LayerMask value)
        {
            wallCullingLayer = value;
        }

        public float GetWallCheckRange()
        {
            return wallCheckRange;
        }

        public void SetWallCheckRange(float value)
        {
            wallCheckRange = value;
        }

        public float GetWallCheckRadius()
        {
            return wallCheckRadius;
        }

        public void SetWallCheckRadius(float value)
        {
            wallCheckRadius = value;
        }

        public float GetGravityMultiplier()
        {
            return gravityMultiplier;
        }

        public void SetGravityMultiplier(float value)
        {
            gravityMultiplier = value;
        }

        public float GetStickGroundForce()
        {
            return stickGroundForce;
        }

        public void SetStickGroundForce(float value)
        {
            stickGroundForce = value;
        }

        public bool SmoothControlInput()
        {
            return smoothControlInput;
        }

        public void SmoothControlInput(bool value)
        {
            smoothControlInput = value;
        }

        public float GetSmoothControlInputValue()
        {
            return smoothControlInputValue;
        }

        public void SetSmoothControlInputValue(float value)
        {
            smoothControlInputValue = value;
        }

        public bool SmoothVelocity()
        {
            return smoothVelocity;
        }

        public void SmoothVelocity(bool value)
        {
            smoothVelocity = value;
        }

        public float GetSmoothVelocityValue()
        {
            return smoothVelocityValue;
        }

        public void SetSmoothVelocityValue(float value)
        {
            smoothVelocityValue = value;
        }

        public bool SmoothSpeed()
        {
            return smoothSpeed;
        }

        public void SmoothSpeed(bool value)
        {
            smoothSpeed = value;
        }

        public float GetSmoothSpeedValue()
        {
            return smoothSpeedValue;
        }

        public void SetSmoothSpeedValue(float value)
        {
            smoothSpeedValue = value;
        }

        public bool SmoothFinalDirection()
        {
            return smoothDirection;
        }

        public void SmoothFinalDirection(bool value)
        {
            smoothDirection = value;
        }

        public float GetSmoothFinalDirectionValue()
        {
            return smoothDirectionValue;
        }

        public void SetSmoothFinalDirectionValue(float value)
        {
            smoothDirectionValue = value;
        }

        public bool SmoothInputMagnitude()
        {
            return smoothInputMagnitude;
        }

        public void SmoothInputMagnitude(bool value)
        {
            smoothInputMagnitude = value;
        }

        public float GetSmoothInputMagnitudeValue()
        {
            return smoothInputMagnitudeValue;
        }

        public void SetSmoothInputMagnitudeValue(float value)
        {
            smoothInputMagnitudeValue = value;
        }

        public RaycastHit GetWallInfo()
        {
            return wallInfo;
        }
        #endregion
    }
}

