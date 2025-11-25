/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


#region [Unity Editor Section]
#if UNITY_EDITOR
#endif
#endregion

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Controller/Deprecated/Character Controller/First Person Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    [Obsolete("Use Player Controller implementation instead.")]
    public class FPCharacterController : Controller
    {
        #region [Enums]
        public enum MovementType
        {
            Walk,
            Run
        }
        #endregion

        #region [Controller Editable Properties]
        // *** Locomotion settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-100)]
        private MovementType movementType = MovementType.Run;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-99)]
        private float walkSpeed = 5.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-98)]
        private float runSpeed = 7.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-97)]
        private float sprintSpeed = 8.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-96)]
        private float crouchSpeed = 3.25f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-95)]
        private float zoomSpeed = 4.0f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Order(-94)]
        private float crouchZoomSpeed = 2.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Label("Backwards Speed")]
        [VisualClamp(0, 1)]
        [Suffix("%", true)]
        [Order(-93)]
        private float backwardSpeedPercent = 0.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Locomotion Settings", Style = "Header")]
        [Label("Side Speed")]
        [VisualClamp(0, 1)]
        [Suffix("%", true)]
        [Order(-92)]
        private float sideSpeedPercent = 0.75f;

        // *** Jump Settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Jump Settings", Style = "Header")]
        [Label("Force")]
        [Order(100)]
        private float jumpForce = 8.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Jump Settings", Style = "Header")]
        [Label("Indulgence Time")]
        [Range(0.0f, 1.0f)]
        [Suffix("sec", true)]
        [Order(101)]
        private float jumpIndulgenceTime = 0.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Jump Settings", Style = "Header")]
        [Order(102)]
        private bool jumpInAir = false;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Jump Settings", Style = "Header")]
        [Label("Count")]
        [VisibleIf("jumpInAir", true)]
        [MinValue(2)]
        [Indent(1)]
        [Order(103)]
        private int jumpCount = 2;

        // *** Air settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Air Settings", Style = "Header")]
        [Order(104)]
        private bool airControl = false;

        [SerializeField]
        [Range(0.01f, 100.0f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Air Settings", Style = "Header")]
        [Label("Control")]
        [VisualClamp(0.01f, 20.0f)]
        [VisibleIf("airControl", true)]
        [Indent(1)]
        [Order(105)]
        private float airControlPersent = 1.0f;

        // *** Acceleration settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(106)]
        private AccelerationDirection accelerationDirection = AccelerationDirection.Forward;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(107)]
        private float runVelocityThreshold = 0.0f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(108)]
        private float sprintVelocityThreshold = 0.0f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Acceleration Settings", Style = "Header")]
        [Order(109)]
        private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        // *** Crouch settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(110)]
        private InputHandleType crouchActionType = InputHandleType.Hold;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(111)]
        private float crouchHeightPercent = 0.6f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(112)]
        private float crouchDuration = 0.5f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Crouch Settings", Style = "Header")]
        [Order(113)]
        private AnimationCurve crouchCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        // *** Gravity settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Gravity Settings", Style = "Header")]
        [Order(114)]
        private float gravityMultiplier = 2.0f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Gravity Settings", Style = "Header")]
        [Order(115)]
        private float stickToGroundForce = 0.75f;

        // *** Grounded settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(116)]
        private LayerMask groundCullingLayer = 1 << 0;

        [SerializeField]
        [Range(0f, 1f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(117)]
        private float groundCheckRange = 0.1f;

        [SerializeField]
        [Range(0.01f, 1f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(118)]
        private float groundCheckRadius = 0.1f;

        // *** Roof settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Roof Settings", Style = "Header")]
        [Order(119)]
        private LayerMask roofCullingLayer = 1 << 0;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Roof Settings", Style = "Header")]
        [Order(120)]
        private float roofCheckRadius = 0.1f;

        // *** Check wall settings *** //
        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Check Wall Settings", Style = "Header")]
        [Order(121)]
        private LayerMask wallCullingLayer = 1 << 0;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Check Wall Settings", Style = "Header")]
        [Order(122)]
        private float wallCheckRange = 0.1f;

        [SerializeField]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Check Wall Settings", Style = "Header")]
        [Order(123)]
        private float wallCheckRadius = 0.1f;

        // *** Advanced settings *** //
        [SerializeField]
        [Range(1f, 100.0f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(124)]
        private float smoothInputSpeed = 10.0f;

        [SerializeField]
        [Range(1f, 100.0f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(125)]
        private float smoothVelocitySpeed = 10.0f;

        [SerializeField]
        [Range(1f, 100.0f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(126)]
        private float smoothFinalDirectionSpeed = 20.0f;

        [SerializeField]
        [Range(1f, 100.0f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(127)]
        private float smoothInputMagnitudeSpeed = 5.0f;
        #endregion

        #region [Camera Editable Properties]
        [SerializeField]
        [TabGroup("Base Group", "Camera")]
        [HideExpandButton]
        private FPCameraControl cameraControl;
        #endregion

        #region [Readonly Properties]
        [SerializeField]
        [ReadOnly]
        [Group("Readonly")]
        private ControllerState controllerState = ControllerState.Idle;

        [SerializeField]
        [ReadOnly]
        [Group("Readonly")]
        private CollisionFlags collisionFlags = CollisionFlags.None;
        #endregion

        // Controller locomotion properties.
        private LocomotionType locomotionType = LocomotionType.Movement;
        private bool cameraControlEnabled = true;

        // Stored required components.
        private CharacterController characterController;

        // Stored required properties.
        private RaycastHit groundHitInfo;
        private Vector2 inputVector;
        private Vector2 smoothInputVector;
        private Vector3 finalMoveDirection;
        private Vector3 smoothFinalMoveDirection;
        private Vector3 moveVector;
        private float currentSpeed;
        private float smoothCurrentSpeed;
        private float finalSmoothCurrentSpeed;
        private float defaultControllerHeight;
        private float crouchControllerHeight;
        private Vector3 defaultControllerCenter;
        private Vector3 crouchControllerCenter;
        private float defaultCameraHeight;
        private float crouchCameraHeight;
        private float crouchStandHeightDifference;
        private float inAirTime;
        private float inputVectorMagnitude;
        private float smoothInputVectorMagnitude;
        private float finalGroundedCheckRange;
        private float storedJumpIndulgenceTime;
        private bool isGrounded;
        private bool isJumped;
        private bool isCrouched;
        private bool isLightWalk;
        private bool isSprint;
        private bool previouslyGrounded;
        private CoroutineObject<bool> crouchCoroutineObject;
        private CoroutineObject<float> cameraLandingEffectCoroutineObject;
        private int storedJumpCount;
        private float storedJumpForce;
        private Vector3 storedJumpDirection;
        private bool storedSaveVelocity;
        private Vector3 movementDirection;
        private int becomeInAirState;
        private bool hasWallObstacle;
        private bool hasRoofObstacle;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            cameraControl.Initialize(this);

            characterController.center = new Vector3(0.0f, characterController.height / 2f + characterController.skinWidth, 0.0f);

            defaultControllerCenter = characterController.center;
            defaultControllerHeight = characterController.height;

            crouchControllerHeight = defaultControllerHeight * crouchHeightPercent;
            crouchControllerCenter = (crouchControllerHeight / 2f + characterController.skinWidth) * Vector3.up;

            crouchStandHeightDifference = defaultControllerHeight - crouchControllerHeight;

            defaultCameraHeight = cameraControl.GetHinge().localPosition.y;
            crouchCameraHeight = defaultCameraHeight - crouchStandHeightDifference;

            crouchCoroutineObject = new CoroutineObject<bool>(this);

            finalGroundedCheckRange = groundCheckRange + characterController.center.y;

            isCrouched = false;
            isGrounded = true;
            previouslyGrounded = true;

            ExcludeCullingLayers(LNC.Player);

            OnGroundedCallback += () => storedJumpCount = 0;
            OnGroundedCallback += () => becomeInAirState = 0;
            OnBecomeAirCallback += () => storedJumpIndulgenceTime = Time.time;
            OnBecomeAirCallback += () =>
            {
                if (becomeInAirState == 0)
                    becomeInAirState = 2;
            };
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            RegisterInputActions();
            cameraControl.OnEnable_Internal();
            OnEnableCallback?.Invoke();
        }

        /// <summary>
        /// Called every frame, while the MonoBehaviour is enabled.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            ReadInput();

            if(locomotionType == LocomotionType.Movement)
            {
                SmoothingInputProcessing();
                SmoothingSpeedProcessing();
                SmoothingDirectionProcessing();
                SmoothingInputMagnitudeProcessing();

                CalculateDirection();
                CalculateSpeed();
                CalculateMoveVector();
                CalculateControllerState();

                ApplyGravity();
                ApplyMovement(moveVector);
            }

            GroundedCallbackHandler();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            CheckWallObstacle();
            CheckRoofObstacle();
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            RemoveInputActions();
            cameraControl.OnDisable_Internal();
            OnDisableCallback?.Invoke();
        }

        /// <summary>
        /// Read input and save in Vector2D representation.
        /// </summary>
        protected virtual void ReadInput()
        {
            inputVector.x = InputReceiver.MovementHorizontalAction.ReadValue<float>();
            inputVector.y = InputReceiver.MovementVerticalAction.ReadValue<float>();
        }

        /// <summary>
        /// Calculate speed of the controller.
        /// </summary>
        protected virtual void CalculateSpeed()
        {
            if (inputVector == Vector2.zero)
            {
                currentSpeed = 0.0f;
                return;
            }

            switch (movementType)
            {
                case MovementType.Run:
                    currentSpeed = isLightWalk ? walkSpeed : runSpeed;
                    currentSpeed = isSprint && CanAccelerate() ? sprintSpeed : currentSpeed;
                    break;
                case MovementType.Walk:
                    currentSpeed = isSprint && CanAccelerate() ? runSpeed : walkSpeed;
                    break;
            }
            currentSpeed = isCrouched ? crouchSpeed : currentSpeed;
            currentSpeed = cameraControl.IsZooming() ? zoomSpeed : currentSpeed;
            currentSpeed = cameraControl.IsZooming() && isCrouched ? crouchZoomSpeed : currentSpeed;
            currentSpeed = inputVector.y < 0 ? currentSpeed * backwardSpeedPercent : currentSpeed;
            currentSpeed = inputVector.x != 0 && inputVector.y == 0 ? currentSpeed * sideSpeedPercent : currentSpeed;
        }

        /// <summary>
        /// Calculate controller direction.
        /// </summary>
        protected virtual void CalculateDirection()
        {
            Vector3 verticalDirection = transform.forward * smoothInputVector.y;
            Vector3 horizontalDirection = transform.right * smoothInputVector.x;

            Vector3 desiredDirection = verticalDirection + horizontalDirection;
            finalMoveDirection = FlattenVectorOnSlopes(desiredDirection);
            movementDirection = finalMoveDirection;
        }

        /// <summary>
        /// Calculate final character controller move vector.
        /// </summary>
        protected virtual void CalculateMoveVector()
        {
            Vector3 finalVector = smoothFinalMoveDirection * finalSmoothCurrentSpeed * smoothInputVectorMagnitude;

            if (characterController.isGrounded && !hasWallObstacle)
            {
                moveVector.x = finalVector.x;
                moveVector.z = finalVector.z;
                moveVector.y += finalVector.y;
            }
            else if (!characterController.isGrounded && airControl)
            {
                float y = moveVector.y;
                float smooth = inputVector != Vector2.zero ? airControlPersent : 0.01f;
                moveVector = Vector3.Lerp(moveVector, finalVector, smooth * Time.deltaTime);
                moveVector.y = y;
            }
        }

        /// <summary>
        /// Calculate actual controller state.
        /// </summary>
        protected virtual void CalculateControllerState()
        {
            if (currentSpeed != 0 && isGrounded)
            {
                controllerState &= ~ControllerState.Idle;
                controllerState &= ~ControllerState.InAir;

                if (currentSpeed == walkSpeed ||
                    currentSpeed == walkSpeed * backwardSpeedPercent ||
                    currentSpeed == walkSpeed * sideSpeedPercent ||
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
            else if (isGrounded)
            {
                controllerState = ControllerState.Idle;
            }
            else
            {
                controllerState = ControllerState.InAir;
            }

            if (isCrouched)
                controllerState |= ControllerState.Crouched;
            else
                controllerState &= ~ControllerState.Crouched;

            if (isJumped)
                controllerState |= ControllerState.Jumped;
            else
                controllerState &= ~ControllerState.Jumped;

            if (cameraControl.IsZooming())
                controllerState |= ControllerState.Zooming;
            else
                controllerState &= ~ControllerState.Zooming;
        }

        /// <summary>
        /// Smoothing input vector.
        /// </summary>
        protected virtual void SmoothingInputProcessing()
        {
            inputVector = inputVector.normalized;
            smoothInputVector = Vector2.Lerp(smoothInputVector, inputVector, Time.deltaTime * smoothInputSpeed);
        }

        /// <summary>
        /// Smoothing controller speed value.
        /// </summary>
        protected virtual void SmoothingSpeedProcessing()
        {
            smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, currentSpeed, Time.deltaTime * smoothVelocitySpeed);
            if (isSprint && CanAccelerate())
            {
                float accelerateSpeed = movementType == MovementType.Run ? sprintSpeed : runSpeed;
                float acceleratePersent = Mathf.InverseLerp(currentSpeed, accelerateSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = accelerationCurve.Evaluate(acceleratePersent) * (accelerateSpeed - currentSpeed) + currentSpeed;
            }
            else
            {
                finalSmoothCurrentSpeed = smoothCurrentSpeed;
            }
        }

        /// <summary>
        /// Smoothing controller direction vector.
        /// </summary>
        protected virtual void SmoothingDirectionProcessing()
        {
            smoothFinalMoveDirection = Vector3.Lerp(smoothFinalMoveDirection, finalMoveDirection, Time.deltaTime * smoothFinalDirectionSpeed);
        }

        /// <summary>
        /// Smoothing input magnitude value. 
        /// </summary>
        protected virtual void SmoothingInputMagnitudeProcessing()
        {
            inputVectorMagnitude = inputVector.magnitude;
            smoothInputVectorMagnitude = Mathf.Lerp(smoothInputVectorMagnitude, inputVectorMagnitude, Time.deltaTime * smoothInputMagnitudeSpeed);
        }

        /// <summary>
        /// OnControllerColliderHit is called when the controller hits a collider while performing a Move.
        /// </summary>
        /// <param name="hit">Detailed information about the collision and how to deal with it.</param>
        protected virtual void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (collisionFlags == CollisionFlags.Below || body == null || body.isKinematic)
                return;

            body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        /// <summary>
        /// Checks whether there is an obstacle in the form of a wall in front of the controller.
        /// </summary>
        protected virtual void CheckWallObstacle()
        {
            Vector3 origin = transform.TransformPoint(characterController.center);
            hasWallObstacle = Physics.Raycast(origin, finalMoveDirection, wallCheckRange + characterController.radius, wallCullingLayer, QueryTriggerInteraction.Ignore);
        }

        public bool HasWallObstacle()
        {
            return hasWallObstacle;
        }

        /// <summary>
        /// Checks whether there is an obstacle in the form of a ceiling above the controller.
        /// </summary>
        /// <returns></returns>
        protected virtual void CheckRoofObstacle()
        {
            Vector3 origin = transform.position + (Vector3.up * defaultControllerHeight);

            hasRoofObstacle = Physics.CheckSphere(origin, roofCheckRadius, roofCullingLayer, QueryTriggerInteraction.Ignore);
        }

        public bool HasRoofObstacle()
        {
            return hasRoofObstacle;
        }

        public override bool IsMoving()
        {
            return isGrounded && inputVector != Vector2.zero && !hasWallObstacle;
        }

        /// <summary>
        /// Checks whether the controller can accelerate at the current moment.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanAccelerate()
        {
            float threshold = movementType == MovementType.Run ? sprintVelocityThreshold : runVelocityThreshold;
            return threshold <= GetVelocity().sqrMagnitude && !isCrouched && !cameraControl.IsZooming() && CheckAccelerationDirection();
        }

        /// <summary>
        /// Check that acceleration direction is correct.
        /// </summary>
        /// <returns>True, if the current direction satisfies the direction in which controller can accelerate. 
        /// Otherwise false.</returns>
        public bool CheckAccelerationDirection()
        {
            if (accelerationDirection == AccelerationDirection.All)
                return true;
            else if (accelerationDirection == AccelerationDirection.Forward && (inputVector.y > 0 && inputVector.x == 0))
                return true;
            else if (accelerationDirection == AccelerationDirection.Backword && (inputVector.y < 0 && inputVector.x == 0))
                return true;
            else if (accelerationDirection == AccelerationDirection.Side && (inputVector.x != 0 && inputVector.y == 0))
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Forward | AccelerationDirection.Backword)) && (inputVector.y != 0 && inputVector.x == 0))
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Forward | AccelerationDirection.Side)) && inputVector.y > 0)
                return true;
            else if ((accelerationDirection == (AccelerationDirection.Backword | AccelerationDirection.Side)) && inputVector.y < 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Flatten direction vector on slopes.
        /// </summary>
        /// <param name="vectorToFlatten">Desired direction vector.</param>
        /// <returns>Flatten vector.</returns>
        public Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlatten)
        {
            return Vector3.ProjectOnPlane(vectorToFlatten, groundHitInfo.normal);
        }

        /// <summary>
        /// Switch controller crouching state.
        /// </summary>
        /// <param name="crouch">New crouching state for controller</param>
        public override void Crouch(bool crouch)
        {
            crouchCoroutineObject.Start(CrouchProcessing, crouch, true);
        }

        public void JumpHandler()
        {
            if (isJumped)
            {
                if (!storedSaveVelocity)
                {
                    moveVector = Vector3.zero;
                }
                moveVector.x += storedJumpDirection.x * storedJumpForce;
                moveVector.y = storedJumpDirection.y * storedJumpForce;
                moveVector.z += storedJumpDirection.z * storedJumpForce;
                OnJumpCallback?.Invoke();
                isJumped = false;
            }
        }

        public override void MakeJump(float force)
        {
            MakeJump(force, transform.up);
        }

        public override void MakeJump(float force, Vector3 direction)
        {
            MakeJump(force, direction, true);
        }

        public override void MakeJump(float force, Vector3 direction, bool saveVelocity)
        {
            storedJumpForce = force;
            storedJumpDirection = direction;
            storedSaveVelocity = saveVelocity;
            isJumped = true;
            if (becomeInAirState == 0)
                becomeInAirState = 1;
        }

        /// <summary>
        /// Applying physics gravity to controller.
        /// </summary>
        protected virtual void ApplyGravity()
        {
            if (characterController.isGrounded)
            {
                moveVector.y = -stickToGroundForce;
            }
            else if (hasRoofObstacle)
            {
                moveVector.y = -10;
            }
            else
            {
                moveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
            }
            JumpHandler();
        }

        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        protected virtual void ApplyMovement(Vector3 velocity)
        {
            collisionFlags = characterController.Move(velocity * Time.deltaTime);
            if (characterController.velocity != Vector3.zero)
                OnMoveCallback?.Invoke(characterController.velocity);
        }

        /// <summary>
        /// Manually check of controller grounded.
        /// </summary>
        protected virtual void CheckGrounded()
        {
            Vector3 origin = transform.position + characterController.center;
            isGrounded = Physics.SphereCast(origin, groundCheckRadius, -transform.up, out groundHitInfo, finalGroundedCheckRange, groundCullingLayer);
        }

        /// <summary>
        /// Exclude specific layers from all culling layers, which used in controller.
        /// </summary>
        /// <param name="excludeLayers">Layers to exclude.</param>
        protected virtual void ExcludeCullingLayers(params string[] excludeLayers)
        {
            if (excludeLayers != null)
            {
                for (int i = 0; i < excludeLayers.Length; i++)
                {
                    int layer = LayerMask.NameToLayer(excludeLayers[i]);
                    groundCullingLayer = groundCullingLayer.ExcludeLayer(layer);
                    roofCullingLayer = roofCullingLayer.ExcludeLayer(layer);
                    wallCullingLayer = wallCullingLayer.ExcludeLayer(layer);
                }
            }
        }

        /// <summary>
        /// Crouch coroutine processing.
        /// </summary>
        protected virtual IEnumerator CrouchProcessing(bool crouch)
        {
            isCrouched = crouch;
            float time = 0.0f;
            float speed = 1.0f / (crouchDuration > 0 ? crouchDuration : 0.01f);

            float desiredHeight = isCrouched ? crouchControllerHeight : defaultControllerHeight;
            Vector3 desiredCenter = isCrouched ? crouchControllerCenter : defaultControllerCenter;

            Transform cameraHinge = cameraControl.GetHinge();
            Vector3 cameraPosition = cameraHinge.localPosition;
            float cameraDesiredHeight = isCrouched ? crouchCameraHeight : defaultCameraHeight;

            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;
                float smoothTime = crouchCurve.Evaluate(time);

                characterController.height = Mathf.Lerp(characterController.height, desiredHeight, smoothTime);
                characterController.center = Vector3.Lerp(characterController.center, desiredCenter, smoothTime);

                cameraPosition.y = Mathf.Lerp(cameraPosition.y, cameraDesiredHeight, smoothTime);
                cameraHinge.localPosition = cameraPosition;
                yield return null;
            }
        }

        /// <summary>
        /// OnGroundedCallback handler.
        /// </summary>
        protected void GroundedCallbackHandler()
        {
            if (!previouslyGrounded && isGrounded)
            {
                OnGroundedCallback?.Invoke();
            }
            else if (previouslyGrounded && !isGrounded)
            {
                OnBecomeAirCallback?.Invoke();
            }
            previouslyGrounded = isGrounded;
        }

        protected virtual void RegisterInputActions()
        {
            // Jump action callback.
            InputReceiver.JumpAction.performed += OnJumpAction;
            InputReceiver.JumpAction.canceled += OnJumpAction;

            // Crouch action callback.
            InputReceiver.CrouchAction.performed += OnCrouchAction;
            InputReceiver.CrouchAction.canceled += OnCrouchAction;

            // Sprint action callback.
            InputReceiver.SprintAction.performed += OnSprintAction;
            InputReceiver.SprintAction.canceled += OnSprintAction;

            // Light walk action callback.
            InputReceiver.LightWalkAction.performed += OnLightWalkAction;
            InputReceiver.LightWalkAction.canceled += OnLightWalkAction;
        }

        protected virtual void RemoveInputActions()
        {
            // Jump action callback.
            InputReceiver.JumpAction.performed -= OnJumpAction;
            InputReceiver.JumpAction.canceled -= OnJumpAction;

            // Crouch action callback.
            InputReceiver.CrouchAction.performed -= OnCrouchAction;
            InputReceiver.CrouchAction.canceled -= OnCrouchAction;

            // Sprint action callback.
            InputReceiver.SprintAction.performed -= OnSprintAction;
            InputReceiver.SprintAction.canceled -= OnSprintAction;

            // Light walk action callback.
            InputReceiver.LightWalkAction.performed -= OnLightWalkAction;
            InputReceiver.LightWalkAction.canceled -= OnLightWalkAction;
        }

        #region [Input Action Wrapper]
        private void OnJumpAction(InputAction.CallbackContext context)
        {
            if (context.performed && 
                !isCrouched && 
                ((jumpInAir && storedJumpCount < jumpCount) || (!jumpInAir && isGrounded) || (becomeInAirState == 2 && jumpIndulgenceTime > 0 && Time.time - storedJumpIndulgenceTime <= jumpIndulgenceTime)))
            {
                MakeJump(jumpForce);
                storedJumpCount++;
            }
        }

        private void OnCrouchAction(InputAction.CallbackContext context)
        {
            switch (crouchActionType)
            {
                case InputHandleType.Trigger:
                    if (context.performed)
                        Crouch(!isCrouched);
                    break;
                case InputHandleType.Hold:
                    if (context.performed)
                        Crouch(true);
                    else if (context.canceled)
                        Crouch(false);
                    break;
            }
        }

        private void OnSprintAction(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                isSprint = true;
                Crouch(false);
            }
            else if (context.canceled)
            {
                isSprint = false;
            }
        }

        private void OnLightWalkAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                isLightWalk = true;
            else if (context.canceled)
                isLightWalk = false;
        }
        #endregion

        #region [IController Implementation]
        public override Vector3 GetVelocity()
        {
            return characterController.velocity;
        }
        #endregion

        #region [IControllerMovement Implementation]
        public override Vector3 GetMovementDirection()
        {
            return movementDirection;
        }
        #endregion

        #region [IcontrollerSpeed Implementation]
        public override float GetSpeed()
        {
            return currentSpeed;
        }

        public override float GetWalkSpeed()
        {
            return walkSpeed;
        }

        public override float GetRunSpeed()
        {
            return runSpeed;
        }

        public override float GetSprintSpeed()
        {
            return sprintSpeed;
        }

        public override float GetBackwardSpeedPerсent()
        {
            return backwardSpeedPercent;
        }

        public override float GetSideSpeedPerсent()
        {
            return sideSpeedPercent;
        }

        public override void SetWalkSpeed(float value)
        {
            walkSpeed = value;
        }

        public override void SetRunSpeed(float value)
        {
            runSpeed = value;
        }

        public override void SetSprintSpeed(float value)
        {
            sprintSpeed = value;
        }

        public override void SetBackwardSpeedPerсent(float value)
        {
            backwardSpeedPercent = value;
        }

        public override void SetSideSpeedPerсent(float value)
        {
            sideSpeedPercent = value;
        }
        #endregion

        #region [IControllerJump Implementation]
        public override bool IsJumped()
        {
            return isJumped;
        }
        #endregion

        #region [ControllerState Implementation]
        public override ControllerState GetState()
        {
            return controllerState;
        }

        public override bool CompareState(ControllerState value)
        {
            return controllerState == value;
        }

        public override bool HasState(ControllerState value)
        {
            return (controllerState & value) != 0;
        }
        #endregion

        #region [IControllerInput Implementation]
        public override Vector2 GetMovementInput()
        {
            return inputVector;
        }

        public override float GetHorizontalInput()
        {
            return inputVector.x;
        }

        public override float GetVerticalInput()
        {
            return inputVector.y;
        }
        #endregion

        #region [IControllerEnabled Implementation]
        public override bool IsEnabled()
        {
            return enabled;
        }

        public override void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            characterController.enabled = enabled;
        }
        #endregion

        #region [IControllerGrounded Implementation]
        public override bool IsGrounded()
        {
            return isGrounded;
        }
        #endregion

        #region [IControllerCrouch Implementation]
        public override bool IsCrouched()
        {
            return isCrouched;
        }

        public override float GetCrouchSpeed()
        {
            return crouchSpeed;
        }
        #endregion

        #region [IControllerCallbacks Implementation]
        /// <summary>
        /// Called when controller moved.
        /// <param name="Vector3">Velocity of the controller.</param>
        /// </summary>
        public override event Action<Vector3> OnMoveCallback;

        /// <summary>
        /// Called when controller being grounded.
        /// </summary>
        public override event Action OnGroundedCallback;

        /// <summary>
        /// Called when controller become air.
        /// </summary>
        public override event Action OnBecomeAirCallback;

        /// <summary>
        /// Called when controller jumped.
        /// </summary>
        public override event Action OnJumpCallback;

        /// <summary>
        /// Called when controller being enabled.
        /// </summary>
        public override event Action OnEnableCallback;

        /// <summary>
        /// Called when controller being disabled.
        /// </summary>
        public override event Action OnDisableCallback;
        #endregion

        #region [Getter / Setter]
        public override FPCameraControl GetCameraControl()
        {
            return cameraControl;
        }

        public void SetCameraControl(FPCameraControl value)
        {
            cameraControl = value;
        }

        public MovementType GetMovementType()
        {
            return movementType;
        }

        public void SetMovementType(MovementType value)
        {
            movementType = value;
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

        public float GetJumpForce()
        {
            return jumpForce;
        }

        public void SetJumpForce(float value)
        {
            jumpForce = value;
        }

        public int GetJumpCount()
        {
            return jumpCount;
        }

        public void SetJumpCount(int value)
        {
            jumpCount = value;
        }

        public bool JumpInAir()
        {
            return jumpInAir;
        }

        public void JumpInAir(bool value)
        {
            jumpInAir = value;
        }

        public override bool AirControl()
        {
            return airControl;
        }

        public override void AirControl(bool value)
        {
            airControl = value;
        }

        public float GetInAirControlPersent()
        {
            return airControlPersent;
        }

        public void SetInAirControlPersent(float value)
        {
            airControlPersent = value;
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

        public InputHandleType GetCrouchHandleType()
        {
            return crouchActionType;
        }

        public void SetCrouchHandleType(InputHandleType value)
        {
            crouchActionType = value;
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

        public override float GetGravityMultiplier()
        {
            return gravityMultiplier;
        }

        public void SetGravityMultiplier(float value)
        {
            gravityMultiplier = value;
        }

        public float GetStickToGroundForce()
        {
            return stickToGroundForce;
        }

        public void SetStickToGroundForce(float value)
        {
            stickToGroundForce = value;
        }

        public LayerMask GetGroundCullingLayer()
        {
            return groundCullingLayer;
        }

        public void SetGroundCullingLayer(LayerMask value)
        {
            groundCullingLayer = value;
        }

        public float GetGroundCheckRange()
        {
            return groundCheckRange;
        }

        public void SetGroundCheckRange(float value)
        {
            groundCheckRange = value;
        }

        public float GetGroundCheckRadius()
        {
            return groundCheckRadius;
        }

        public void SetGroundCheckRadius(float value)
        {
            groundCheckRadius = value;
        }

        public LayerMask GetRoofCullingLayer()
        {
            return roofCullingLayer;
        }

        public void SetRoofCullingLayer(LayerMask value)
        {
            roofCullingLayer = value;
        }

        public float GetRoofCheckRadius()
        {
            return roofCheckRadius;
        }

        public void SetRoofCheckRadius(float value)
        {
            roofCheckRadius = value;
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

        public float GetSmoothInputSpeed()
        {
            return smoothInputSpeed;
        }

        public void SetSmoothInputSpeed(float value)
        {
            smoothInputSpeed = value;
        }

        public float GetSmoothVelocitySpeed()
        {
            return smoothVelocitySpeed;
        }

        public void SetSmoothVelocitySpeed(float value)
        {
            smoothVelocitySpeed = value;
        }

        public float GetSmoothFinalDirectionSpeed()
        {
            return smoothFinalDirectionSpeed;
        }

        public void SetSmoothFinalDirectionSpeed(float value)
        {
            smoothFinalDirectionSpeed = value;
        }

        public float GetSmoothInputMagnitudeSpeed()
        {
            return smoothInputMagnitudeSpeed;
        }

        public void SetSmoothInputMagnitudeSpeed(float value)
        {
            smoothInputMagnitudeSpeed = value;
        }

        public ControllerState GetControllerState()
        {
            return controllerState;
        }

        protected void SetControllerState(ControllerState value)
        {
            controllerState = value;
        }

        public CollisionFlags GetCollisionFlags()
        {
            return collisionFlags;
        }

        public LocomotionType GetLocomotionType()
        {
            return locomotionType;
        }

        public override void SetLocomotionType(LocomotionType value)
        {
            locomotionType = value;
            moveVector = Vector3.zero;
            characterController.enabled = locomotionType == LocomotionType.Movement;
        }

        public bool CameraControlEnabled()
        {
            return cameraControlEnabled;
        }

        public void CameraControlEnabled(bool value)
        {
            cameraControlEnabled = value;
        }

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        protected void SetCharacterController(CharacterController value)
        {
            characterController = value;
        }
        #endregion
    }
}
