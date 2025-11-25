/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov, Deryabin Vladimir
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules.ControllerSystems
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class ActorController : MovableController, IControllerCrouch, IPlayerJump, IInputListener
    {
        public enum AirFlag
        {
            None,
            Jumped,
            Fall
        }

        [SerializeField]
        [Label("Force")]
        [Foldout("Jump Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(-299)]
        private float jumpForce = 8.5f;

        [SerializeField]
        [Label("Indulgence Time")]
        [Slider(0.0f, 1.0f)]
        [Foldout("Jump Settings", Style = "Header")]
        [Suffix("sec", true)]
        [Order(-298)]
        private float jumpIndulgenceTime = 0.5f;

        [SerializeField]
        [Foldout("Jump Settings", Style = "Header")]
        [Order(-297)]
        private bool jumpInAir = false;

        [SerializeField]
        [Label("Count")]
        [Foldout("Jump Settings", Style = "Header")]
        [VisibleIf("jumpInAir", true)]
        [MinValue(2)]
        [Indent(1)]
        [Order(-296)]
        private int jumpCount = 2;

        // *** Air settings *** //
        [SerializeField]
        [Foldout("Air Settings", Style = "Header")]
        [Order(-199)]
        private bool airControl = false;

        [SerializeField]
        [Label("Control")]
        [Foldout("Air Settings", Style = "Header")]
        [Slider(0.01f, 20)]
        [VisibleIf("airControl", true)]
        [Indent(1)]
        [Order(-199)]
        private float airControlPercent = 1.0f;

        // *** Crouch settings *** //
        [SerializeField]
        [Label("Hold Interaction")]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(-99)]
        private bool crouchHoldInteraction = false;

        [SerializeField]
        [Foldout("Crouch Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(-98)]
        private float crouchHeightPercent = 0.6f;

        [SerializeField]
        [Foldout("Crouch Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(-97)]
        private float crouchDuration = 0.5f;

        [SerializeField]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(-96)]
        private AnimationCurve crouchCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        // *** Roof settings *** //
        [SerializeField]
        [Foldout("Roof Settings", Style = "Header")]
        [Order(498)]
        private LayerMask roofCullingLayer = 1 << 0;

        [SerializeField]
        [Foldout("Roof Settings", Style = "Header")]
        [MinValue(0.01f)]
        [Order(499)]
        private float roofCheckRadius = 0.1f;

        // Stored required properties.
        private bool hasRoofObstacle;
        private bool isJumped;
        private bool isCrouched;
        private int storedJumpCount;
        private float storedJumpForce;
        private bool storedSaveVelocity;
        private float storedJumpIndulgenceTime;
        private float crouchControllerHeight;
        private float defaultControllerHeight;
        private Vector3 currentControllerCenter;
        private Vector3 defaultControllerCenter;
        private Vector3 crouchControllerCenter;
        private Vector3 storedJumpDirection;
        private AirFlag airFlag = AirFlag.None;
        private CoroutineObject<bool> crouchCoroutineObject;

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            RegisterInputActions();
        }

        /// <summary>
        /// Called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            CopyBounds(out defaultControllerCenter, out defaultControllerHeight);
            currentControllerCenter = defaultControllerCenter;
            crouchControllerHeight = defaultControllerHeight * crouchHeightPercent;
            crouchControllerCenter = (crouchControllerHeight / 2f) * Vector3.up;

            crouchCoroutineObject = new CoroutineObject<bool>(this);

            OnKinematicCallback += KinematicAction;
            OnGroundedCallback += GroundedAction;
            OnBecomeAirCallback += BecomeInAirAction;
        }

        /// <summary>
        /// Called every fixed frame-rate frame.
        /// <br>0.02 seconds (50 calls per second) is the default time between calls.</br>
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            hasRoofObstacle = CheckRoofObstacle(transform.TransformPoint(currentControllerCenter));
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveInputActions();
        }

        /// <summary>
        /// Calculate final character controller move vector.
        /// </summary>
        protected override Vector3 CalculateMovement(Vector3 direction, float speed, float inputMagnitude)
        {
            if (!IsGrounded() && airControl && GetControlInput() != Vector2.zero)
            {
                float y = GetMovementVector().y;
                Vector3 movementVector = inputMagnitude * speed * direction;
                Vector3 nextVector = Vector3.Lerp(GetMovementVector(), movementVector, airControlPercent * Time.deltaTime);
                nextVector.y = y;
                return nextVector;
            }
            else
            {
                return base.CalculateMovement(direction, speed, inputMagnitude);
            }
        }

        /// <summary>
        /// Applying physics gravity to controller, while is not grounded.
        /// </summary>
        /// <param name="movementVector">Controller movement vector.</param>
        protected override void ApplyGravity(ref Vector3 movementVector)
        {
            base.ApplyGravity(ref movementVector);
            if (isJumped)
            {
                OnJump(ref movementVector, storedJumpForce, storedJumpDirection, storedSaveVelocity);
                OnJumpCallback?.Invoke();
                isJumped = false;
            }

            if (!IsGrounded() && hasRoofObstacle)
            {
                movementVector.y = GetGravityDirection().y;
            }
        }

        /// <summary>
        /// Restore controller to default.
        /// </summary>
        public override void Restore()
        {
            base.Restore();

            GroundedAction();

            crouchCoroutineObject.Stop();
            isCrouched = false;
            isJumped = false;
            EditBounds(defaultControllerCenter, defaultControllerHeight);
        }

        /// <summary>
        /// Checks whether there is an obstacle in the form of a ceiling above the controller.
        /// </summary>
        /// <param name="origin">Center of controller</param>
        protected virtual bool CheckRoofObstacle(Vector3 origin)
        {
            origin += transform.up * currentControllerCenter.y;
            return Physics.CheckSphere(origin, roofCheckRadius, roofCullingLayer, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Switch controller crouching state.
        /// </summary>
        /// <param name="crouch">New crouching state for controller</param>
        public void Crouch(bool crouch)
        {
            crouchCoroutineObject.Start(CrouchProcessing, crouch, true);
        }

        /// <summary>
        /// Crouch coroutine processing.
        /// </summary>
        protected virtual IEnumerator CrouchProcessing(bool crouch)
        {
            isCrouched = crouch;
            float time = 0.0f;
            float speed = 1.0f / (crouchDuration > 0 ? crouchDuration : 0.01f);

            CopyBounds(out Vector3 center, out float height);
            float desiredHeight = isCrouched ? crouchControllerHeight : defaultControllerHeight;
            Vector3 desiredCenter = isCrouched ? crouchControllerCenter : defaultControllerCenter;

            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;
                float smoothTime = crouchCurve.Evaluate(time);

                height = Mathf.Lerp(height, desiredHeight, smoothTime);
                center = Vector3.Lerp(center, desiredCenter, smoothTime);

                EditBounds(center, height);
                currentControllerCenter = center;

                OnCrouchingCallback?.Invoke(crouch, smoothTime);

                yield return null;
            }
        }

        /// <summary>
        /// Make jump.
        /// </summary>
        /// <param name="force">Jump force.</param>
        /// <param name="direction">Jump direction.</param>
        /// <param name="saveVelocity">Save controller velocity which collected before jump.</param>
        public void Jump(float force, Vector3 direction, bool saveVelocity)
        {
            storedJumpForce = force;
            storedJumpDirection = direction;
            storedSaveVelocity = saveVelocity;
            airFlag = AirFlag.Jumped;
            isJumped = true;
        }

        /// <summary>
        /// Make jump.
        /// </summary>
        /// <param name="force">Jump force.</param>
        /// <param name="direction">Jump direction.</param>
        public void Jump(float force, Vector3 direction)
        {
            Jump(force, direction, true);
        }

        /// <summary>
        /// Make jump.
        /// </summary>
        /// <param name="force">Jump force.</param>
        public void Jump(float force)
        {
            Jump(force, transform.up);
        }

        /// <summary>
        /// Called when executed jumping, after applied gravity.
        /// </summary>
        /// <param name="movementVector">Reference controller movement vector.</param>
        /// <param name="force">Jump force.</param>
        /// <param name="direction">Jump direction.</param>
        /// <param name="saveVelocity">Save controller velocity which collected before jump.</param>
        protected virtual void OnJump(ref Vector3 movementVector, float force, Vector3 direction, bool saveVelocity)
        {
            if (!saveVelocity)
            {
                movementVector = Vector3.zero;
            }
            movementVector.x += direction.x * force;
            movementVector.y = direction.y * force;
            movementVector.z += direction.z * force;
        }

        /// <summary>
        /// Called when controller become enabled.
        /// </summary>
        public virtual void RegisterInputActions()
        {
            // Jump action callback.
            InputReceiver.JumpAction.performed += OnJumpAction;
            InputReceiver.JumpAction.canceled += OnJumpAction;

            // Crouch action callback.
            InputReceiver.CrouchAction.performed += OnCrouchAction;
            InputReceiver.CrouchAction.canceled += OnCrouchAction;
        }

        /// <summary>
        /// Called when controller disabled.
        /// </summary>
        public virtual void RemoveInputActions()
        {
            // Jump action callback.
            InputReceiver.JumpAction.performed -= OnJumpAction;
            InputReceiver.JumpAction.canceled -= OnJumpAction;

            // Crouch action callback.
            InputReceiver.CrouchAction.performed -= OnCrouchAction;
            InputReceiver.CrouchAction.canceled -= OnCrouchAction;
        }

        /// <summary>
        /// Controller is crouched.
        /// </summary>
        public bool IsCrouched()
        {
            return isCrouched;
        }

        /// <summary>
        /// Controller is jumped.
        /// </summary>
        public bool IsJumped()
        {
            return isJumped;
        }

        /// <summary>
        /// Is there an obstacle on the roof under the controller.
        /// </summary>
        public bool HasRoofObstacle()
        {
            return hasRoofObstacle;
        }

        #region [Input Action Wrapper]
        private void OnCrouchAction(InputAction.CallbackContext context)
        {
            switch (crouchHoldInteraction)
            {
                case false:
                    if (context.performed)
                        Crouch(!isCrouched);
                    break;
                case true:
                    if (context.performed)
                        Crouch(true);
                    else if (context.canceled)
                        Crouch(false);
                    break;
            }
        }

        private void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.performed
                && !isCrouched
                && ((jumpInAir && storedJumpCount < jumpCount) ||
                (!jumpInAir && IsGrounded()) ||
                (airFlag == AirFlag.Fall &&
                jumpIndulgenceTime > 0 &&
                Time.time - storedJumpIndulgenceTime <= jumpIndulgenceTime)))
            {
                Jump(jumpForce);
                storedJumpCount++;
            }
        }

        private void KinematicAction(bool isKinematic)
        {
            if (isKinematic)
                RemoveInputActions();
            else if (enabled)
                RegisterInputActions();
        }

        private void GroundedAction()
        {
            airFlag = AirFlag.None;
            storedJumpCount = 0;
        }

        private void BecomeInAirAction()
        {
            if (airFlag == AirFlag.None)
                airFlag = AirFlag.Fall;
            storedJumpIndulgenceTime = Time.time;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when controller jumped.
        /// </summary>
        public event Action OnJumpCallback;

        public event Action<bool, float> OnCrouchingCallback;
        #endregion

        #region [Getter / Setter]
        public float GetJumpForce()
        {
            return jumpForce;
        }

        public void SetJumpForce(float value)
        {
            jumpForce = value;
        }

        public float GetJumpIndulgenceTime()
        {
            return jumpIndulgenceTime;
        }

        public void SetJumpIndulgenceTime(float value)
        {
            jumpIndulgenceTime = value;
        }

        public bool JumpInAir()
        {
            return jumpInAir;
        }

        public void JumpInAir(bool value)
        {
            jumpInAir = value;
        }

        public int GetJumpCount()
        {
            return jumpCount;
        }

        public void SetJumpCount(int value)
        {
            jumpCount = value;
        }

        public bool AirControl()
        {
            return airControl;
        }

        public void AirControl(bool value)
        {
            airControl = value;
        }

        public float GetAirControlPercent()
        {
            return airControlPercent;
        }

        public void SetAirControlPercent(float value)
        {
            airControlPercent = value;
        }

        public bool CrouchHoldInteraction()
        {
            return crouchHoldInteraction;
        }

        public void CrouchHoldInteraction(bool value)
        {
            crouchHoldInteraction = value;
        }

        public float GetCrouchHeightPercent()
        {
            return crouchHeightPercent;
        }

        public void SetCrouchHeightPercent(float value)
        {
            crouchHeightPercent = value;
        }

        public float GetCrouchDuration()
        {
            return crouchDuration;
        }

        public void SetCrouchDuration(float value)
        {
            crouchDuration = value;
        }

        public AnimationCurve GetCrouchCurve()
        {
            return crouchCurve;
        }

        public void SetCrouchCurve(AnimationCurve value)
        {
            crouchCurve = value;
        }

        public float GetRoofCheckRadius()
        {
            return roofCheckRadius;
        }

        public void SetRoofCheckRadius(float value)
        {
            roofCheckRadius = value;
        }

        public LayerMask GetRoofCullingLayer()
        {
            return roofCullingLayer;
        }

        public void SetRoofCullingLayer(LayerMask value)
        {
            roofCullingLayer = value;
        }

        public AirFlag GetAirFlag()
        {
            return airFlag;
        }
        #endregion
    }
}
