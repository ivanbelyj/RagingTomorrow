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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Controller/Deprecated/Rigidbody Controller/First Person Controller")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [Obsolete("Use Player Controller implementation instead.")]
    public class FPRigidbodyController : Controller
    {
        private const int ROOF_BREAK_FORCE = -10;

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
        [Order(99)]
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

        // *** Grounded settings *** //
        [SerializeField]
        [Label("Radius")]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(115)]
        private float groundCheckRadius = 0.3f;

        [SerializeField]
        [Label("Ratio")]
        [VisualClamp(0.65f, 0.1f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(116)]
        private float groundCheckRatio = 0.87f;

        [SerializeField]
        [Label("Stick Force")]
        [Slider(0.01f, 1.99f)]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(117)]
        private float groundStickForce = 1.1f;

        [SerializeField]
        [Label("Culling Layer")]
        [TabGroup("Base Group", "Controller")]
        [Foldout("Grounded Settings", Style = "Header")]
        [Order(118)]
        private LayerMask groundCullingLayer = 1 << 0;

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

        private ControllerState controllerState = ControllerState.Idle;
        private CollisionFlags collisionFlags = CollisionFlags.None;

        // Controller locomotion properties.
        private LocomotionType locomotionType = LocomotionType.Movement;
        private bool cameraControlEnabled = true;

        // Stored required components.
        private new Rigidbody rigidbody;
        private CapsuleCollider capsuleCollider;

        // Stored physics materials.
#if UNITY_6000_0_OR_NEWER
        private PhysicsMaterial maxFrictionPhysics;
        private PhysicsMaterial frictionPhysics;
        private PhysicsMaterial slippyPhysics;
#else
        private PhysicMaterial maxFrictionPhysics;
        private PhysicMaterial frictionPhysics;
        private PhysicMaterial slippyPhysics;
#endif

        // Stored required properties.
        private RaycastHit groundHitInfo;
        private Vector2 inputVector;
        private Vector2 smoothInputVector;
        private Vector3 localUpVelocity;
        private Vector3 finalMoveDirection;
        private Vector3 smoothFinalMoveDirection;
        private Vector3 moveVector;
        private float speed;
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
        private float storedJumpIndulgenceTime;
        private bool isGrounded;
        private bool actualGrounded;
        private bool isJumped;
        private bool isGroundedJumped;
        private bool isCrouched;
        private bool isLightWalk;
        private bool isSprint;
        private bool previouslyGrounded;
        private CoroutineObject<bool> crouchCoroutineObject;
        private CoroutineObject breakGroundedJump;
        private int storedJumpCount;
        private Vector3 controllerBottomPoint;
        private Vector3 storedJumpDirection;
        private float storedJumpForce;
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
            rigidbody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();

            cameraControl.Initialize(this);
            crouchCoroutineObject = new CoroutineObject<bool>(this);
            breakGroundedJump = new CoroutineObject(this);
            CreatePhysicMaterial();
            ExcludeCullingLayers(LNC.Player);
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            capsuleCollider.center = new Vector3(0.0f, capsuleCollider.height / 2f, 0.0f);

            defaultControllerCenter = capsuleCollider.center;
            defaultControllerHeight = capsuleCollider.height;

            crouchControllerHeight = defaultControllerHeight * crouchHeightPercent;
            crouchControllerCenter = (crouchControllerHeight / 2f) * Vector3.up;

            crouchStandHeightDifference = defaultControllerHeight - crouchControllerHeight;

            defaultCameraHeight = cameraControl.GetHinge().localPosition.y;
            crouchCameraHeight = defaultCameraHeight - crouchStandHeightDifference;

            isCrouched = false;
            isGrounded = true;
            previouslyGrounded = true;

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
            CheckGrounded();

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
            }

            GroundedCallbackHandler();
        }

        private void FixedUpdate()
        {
            if (locomotionType == LocomotionType.Movement)
            {
                ApplyMovement(moveVector);
            }
            CheckWallObstacle();
            CheckRoofObstacle();
            ResetPerformedActionInFixedUpdate();
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
                speed = 0.0f;
            }
            else
            {
                switch (movementType)
                {
                    case MovementType.Run:
                        speed = isLightWalk ? walkSpeed : runSpeed;
                        speed = isSprint && CanAccelerate() ? sprintSpeed : speed;
                        break;
                    case MovementType.Walk:
                        speed = isSprint && CanAccelerate() ? runSpeed : walkSpeed;
                        break;
                }
                speed = isCrouched ? crouchSpeed : speed;
                speed = cameraControl.IsZooming() ? zoomSpeed : speed;
                speed = cameraControl.IsZooming() && isCrouched ? crouchZoomSpeed : speed;
                speed = inputVector.y < 0 ? speed * backwardSpeedPercent : speed;
                speed = inputVector.x != 0 && inputVector.y == 0 ? speed * sideSpeedPercent : speed;
            }
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
                if (isGrounded && !hasWallObstacle)
                {
                    moveVector = finalVector;
                }
                else if (!isGrounded && airControl)
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
            if (speed != 0 && isGrounded)
            {
                controllerState &= ~ControllerState.Idle;
                controllerState &= ~ControllerState.InAir;

                if (speed == walkSpeed ||
                    speed == walkSpeed * backwardSpeedPercent ||
                    speed == walkSpeed * sideSpeedPercent ||
                    speed == crouchSpeed ||
                    speed == crouchSpeed * backwardSpeedPercent ||
                    speed == crouchSpeed * sideSpeedPercent || 
                    speed == zoomSpeed ||
                    speed == zoomSpeed * backwardSpeedPercent ||
                    speed == zoomSpeed * sideSpeedPercent)
                    controllerState |= ControllerState.Walking;
                else
                    controllerState &= ~ControllerState.Walking;

                if (speed == runSpeed ||
                    speed == runSpeed * backwardSpeedPercent ||
                    speed == runSpeed * sideSpeedPercent)
                    controllerState |= ControllerState.Running;
                else
                    controllerState &= ~ControllerState.Running;

                if (CanAccelerate() && speed == sprintSpeed ||
                    speed == sprintSpeed * backwardSpeedPercent ||
                    speed == sprintSpeed * sideSpeedPercent)
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
            smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, speed, Time.deltaTime * smoothVelocitySpeed);
            if (isSprint && CanAccelerate())
            {
                float accelerateSpeed = movementType == MovementType.Run ? sprintSpeed : runSpeed;
                float acceleratePersent = Mathf.InverseLerp(speed, accelerateSpeed, smoothCurrentSpeed);
                finalSmoothCurrentSpeed = accelerationCurve.Evaluate(acceleratePersent) * (accelerateSpeed - speed) + speed;
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
        /// Checks whether there is an obstacle in the form of a wall in move direction of the controller.
        /// </summary>
        protected virtual void CheckWallObstacle()
        {
            Vector3 origin = transform.TransformPoint(capsuleCollider.center);
            hasWallObstacle = Physics.Raycast(origin, finalMoveDirection, wallCheckRange + capsuleCollider.radius, wallCullingLayer, QueryTriggerInteraction.Ignore);
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
            Vector3 origin = transform.TransformPoint(capsuleCollider.center);
            origin += transform.up * capsuleCollider.center.y;
            hasRoofObstacle = Physics.CheckSphere(origin, roofCheckRadius, roofCullingLayer, QueryTriggerInteraction.Ignore);
        }

        public bool HasRoofObstacle()
        {
            return hasRoofObstacle;
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
                isGroundedJumped = true;
                breakGroundedJump.Start(BreakGroundedJump, true);
                OnJumpCallback?.Invoke();
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
            if (!isGrounded)
            {
                moveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
                if (hasRoofObstacle)
                {
                    moveVector.y = ROOF_BREAK_FORCE;
                }
            }
            else if(isGrounded && !isGroundedJumped)
            {
                moveVector -= transform.up * groundStickForce;
            }
            JumpHandler();
        }

        /// <summary>
        /// Applying movement vector to controller.
        /// </summary>
        protected virtual void ApplyMovement(Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            rigidbody.linearVelocity = velocity;
            if (rigidbody.linearVelocity != Vector3.zero)
                OnMoveCallback?.Invoke(rigidbody.linearVelocity);
#else
            rigidbody.velocity = velocity;
            if (rigidbody.velocity != Vector3.zero)
                OnMoveCallback?.Invoke(rigidbody.velocity);
#endif
        }

        protected virtual void ResetPerformedActionInFixedUpdate()
        {
            isJumped = false;
        }

        /// <summary>
        /// Manually check of controller grounded.
        /// </summary>
        protected virtual void CheckGrounded()
        {
            Vector3 origin = transform.TransformPoint(capsuleCollider.center);
            float range = capsuleCollider.center.y - (capsuleCollider.radius * groundCheckRatio);
            actualGrounded = Physics.SphereCast(origin, groundCheckRadius, -transform.up, out groundHitInfo, range, groundCullingLayer);
            isGrounded = actualGrounded;
            if (isGroundedJumped && isGrounded)
            {
                isGrounded = false;
            }
            else if (isGroundedJumped && !isGrounded)
            {
                isGroundedJumped = false;
            }

            // change the physics material to very slip when not grounded or maxFriction when is
            if (isGrounded && inputVector == Vector2.zero)
                capsuleCollider.material = maxFrictionPhysics;
            else if (isGrounded && inputVector != Vector2.zero)
                capsuleCollider.material = frictionPhysics;
            else
                capsuleCollider.material = slippyPhysics;
        }

        public override bool IsMoving()
        {
            return isGrounded && inputVector != Vector2.zero && !hasWallObstacle;
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

                capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, desiredHeight, smoothTime);
                capsuleCollider.center = Vector3.Lerp(capsuleCollider.center, desiredCenter, smoothTime);

                cameraPosition.y = Mathf.Lerp(cameraPosition.y, cameraDesiredHeight, smoothTime);
                cameraHinge.localPosition = cameraPosition;
                yield return null;
            }
        }

        private IEnumerator BreakGroundedJump()
        {
            yield return new WaitForSeconds(1.0f);
            if(isGroundedJumped && actualGrounded)
            {
                isGroundedJumped = false;
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

        private void CreatePhysicMaterial()
        {
            // Slides the character through walls and edges
#if UNITY_6000_0_OR_NEWER
            frictionPhysics = new PhysicsMaterial();
#else
            frictionPhysics = new PhysicMaterial();
#endif
            frictionPhysics.name = "FrictionPhysics";
            frictionPhysics.staticFriction = .25f;
            frictionPhysics.dynamicFriction = .25f;
            frictionPhysics.bounciness = 0.0f;
#if UNITY_6000_0_OR_NEWER
            frictionPhysics.frictionCombine = PhysicsMaterialCombine.Multiply;
            frictionPhysics.bounceCombine = PhysicsMaterialCombine.Minimum;
#else
            frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;
            frictionPhysics.bounceCombine = PhysicMaterialCombine.Minimum;
#endif

            // Prevents the collider from slipping on ramps
#if UNITY_6000_0_OR_NEWER
            maxFrictionPhysics = new PhysicsMaterial();
#else
            maxFrictionPhysics = new PhysicMaterial();
#endif
            maxFrictionPhysics.name = "MaxFrictionPhysics";
            maxFrictionPhysics.staticFriction = 1f;
            maxFrictionPhysics.dynamicFriction = 1f;
            maxFrictionPhysics.bounciness = 0.0f;
#if UNITY_6000_0_OR_NEWER
            maxFrictionPhysics.frictionCombine = PhysicsMaterialCombine.Maximum;
            maxFrictionPhysics.bounceCombine = PhysicsMaterialCombine.Minimum;
#else
            maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;
            maxFrictionPhysics.bounceCombine = PhysicMaterialCombine.Minimum;
#endif

            // Air physics 
#if UNITY_6000_0_OR_NEWER
            slippyPhysics = new PhysicsMaterial();
#else
            slippyPhysics = new PhysicMaterial();
#endif
            slippyPhysics.name = "SlippyPhysics";
            slippyPhysics.staticFriction = 0f;
            slippyPhysics.dynamicFriction = 0f;
            slippyPhysics.bounciness = 0.0f;
#if UNITY_6000_0_OR_NEWER
            slippyPhysics.frictionCombine = PhysicsMaterialCombine.Minimum;
            slippyPhysics.bounceCombine = PhysicsMaterialCombine.Minimum;
#else
            slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;
            slippyPhysics.bounceCombine = PhysicMaterialCombine.Minimum;
#endif
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
#if UNITY_6000_0_OR_NEWER
            return rigidbody.linearVelocity;
#else
            return rigidbody.velocity;
#endif
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
            return speed;
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
        /// Called when controller lost ground.
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

        public LayerMask GetGroundCullingLayer()
        {
            return groundCullingLayer;
        }

        public void SetGroundCullingLayer(LayerMask value)
        {
            groundCullingLayer = value;
        }

        public float GetGroundCheckRadius()
        {
            return groundCheckRatio;
        }

        public void SetGroundCheckRadius(float value)
        {
            groundCheckRatio = value;
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
            rigidbody.isKinematic = value != LocomotionType.Movement;
        }

        public bool CameraControlEnabled()
        {
            return cameraControlEnabled;
        }

        public void CameraControlEnabled(bool value)
        {
            cameraControlEnabled = value;
        }
        #endregion
    }
}
