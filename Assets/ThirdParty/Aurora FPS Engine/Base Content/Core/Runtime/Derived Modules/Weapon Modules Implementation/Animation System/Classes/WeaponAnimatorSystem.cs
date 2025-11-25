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
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using UnityEngine;
using AuroraFPSRuntime.SystemModules;

#region [Unity Editor Section]
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Animation/Weapon Animator System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class WeaponAnimatorSystem : EquippableObjectAnimationSystem
    {
        [System.Serializable]
        public class ProceduralState : CallbackEvent<AnimatorState> { }

        #region [Animator State Properties]
        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState selectState = "Select";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState fireState = "Fire";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState dryFireState = "Dry Fire";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState fireZoomState = "Zoom Fire";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState dryFireZoomState = "Zoom Dry Fire";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState zoomInState = "Base Layer.Zoom.Zoom Locomotion.Idle";

        [SerializeField]
        [Foldout("Animator States", Style = "Header")]
        private AnimatorState zoomOutState = "Base Layer.Default Locomotion.Idle";
        #endregion

        #region [Animator Parameter Properties]
        [SerializeField]
        [Label("Speed")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Float", Style = "Parameter")]
        private AnimatorParameter speedParameter = "Speed";

        [SerializeField]
        [Label("IsGrounded")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Bool", Style = "Parameter")]
        private AnimatorParameter isGroundedParameter = "IsGrounded";

        [SerializeField]
        [Label("IsCrouching")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Bool", Style = "Parameter")]
        private AnimatorParameter isCrouchingParameter = "IsCrouching";

        [SerializeField]
        [Label("IsSprinting")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Bool", Style = "Parameter")]
        private AnimatorParameter isSprintingParameter = "IsSprinting";

        [SerializeField]
        [Label("IsZooming")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Bool", Style = "Parameter")]
        private AnimatorParameter isZoomingParameter = "IsZooming";

        [SerializeField]
        [Label("Hide")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Trigger", Style = "Parameter")]
        private AnimatorParameter hideParameter = "Hide";

        [SerializeField]
        [Label("IsJumped")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Trigger", Style = "Parameter")]
        private AnimatorParameter isJumpedParameter = "IsJumped";

        [SerializeField]
        [Label("IsLanded")]
        [InlineButton("OnSetParameterCallback", Label = "@align_vertically_center", Style = "IconButton")]
        [Foldout("Animator Parameters", Style = "Header")]
        [Prefix("Trigger", Style = "Parameter")]
        private AnimatorParameter isLandedParameter = "IsLanded";
        #endregion

        [SerializeField]
        [ReorderableList(DisplayHeader = false, DisplaySeparator = true)]
        [Foldout("Procedural State Events", Style = "Header")]
        private ProceduralState[] proceduralStates;

        // Stored required components.
        private Animator animator;
        private PlayerCamera playerCamera;
        private PlayerController playerController;
        private WeaponShootingSystem shootingSystem;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            playerController = GetComponentInParent<PlayerController>();
            Debug.Assert(playerController != null, $"<b><color=#FF0000>The Weapon Animator System can be used as a child object on which there is a player controller component.\nAdd component of the player controller to {transform.root.name}<i>(gameobject)</i> -> Player Controller<i>(component)</i>.</color></b>");

            playerCamera = GetComponentInParent<PlayerCamera>();
            shootingSystem = GetComponent<WeaponShootingSystem>();
        }

        /// <summary>
        /// Called on the frame when a script is enabled 
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            RegisterProceduralStates();
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            playerController.OnBecomeAirCallback += OnJumpAction;
            playerController.OnGroundedCallback += OnLandAction;

            if (playerCamera != null)
            {
                playerCamera.OnStartZoomCallback += OnZoomStartAction;
                playerCamera.OnStopZoomCallback += OnZoomEndAction;
            }

            if (shootingSystem != null)
            {
                if (playerCamera != null)
                {
                    shootingSystem.OnShootCallback += OnFireWithZoomAction;
                    shootingSystem.OnEmptyCallback += OnDryFireWithZoomAction;
                }
                else
                {
                    shootingSystem.OnShootCallback += OnFireAction;
                    shootingSystem.OnEmptyCallback += OnDryFireAction;
                }
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            playerController.OnBecomeAirCallback -= OnJumpAction;
            playerController.OnGroundedCallback -= OnLandAction;

            if (playerCamera != null)
            {
                playerCamera.OnStartZoomCallback -= OnZoomStartAction;
                playerCamera.OnStopZoomCallback -= OnZoomEndAction;
            }

            if (shootingSystem != null)
            {
                if(playerCamera != null)
                {
                    shootingSystem.OnShootCallback -= OnFireWithZoomAction;
                    shootingSystem.OnEmptyCallback -= OnDryFireWithZoomAction;
                }
                else
                {
                    shootingSystem.OnShootCallback -= OnFireAction;
                    shootingSystem.OnEmptyCallback -= OnDryFireAction;
                }
            }
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            UpdateAnimatorParameters();
        }

        /// <summary>
        /// Play pull weapon in inventory animation clip.
        /// </summary>
        public override void PlayPullAnimation()
        {
            animator.CrossFadeInFixedTime(selectState);
        }

        /// <summary>
        /// Play push weapon in inventory animation clip.
        /// </summary>
        public override void PlayPushAnimation()
        {
            animator.SetTrigger(hideParameter);
        }

        /// <summary>
        /// Set speed animator parameter.
        /// </summary>
        /// <param name="value">Current controller speed value.</param>
        public virtual void SetSpeedParameter(float value)
        {
            animator.SetFloat(speedParameter, value);
        }

        /// <summary>
        /// Set is grounded animator parameter.
        /// </summary>
        /// <param name="value">Current controller grounded value.</param>
        public virtual void SetGroundedParameter(bool value)
        {
            animator.SetBool(isGroundedParameter, value);
        }

        /// <summary>
        /// Set is crouched animator parameter.
        /// </summary>
        /// <param name="value">Current controller crouched value.</param>
        public virtual void SetCrouchedParameter(bool value)
        {
            animator.SetBool(isCrouchingParameter, value);
        }

        /// <summary>
        /// Set is sprinting animator parameter.
        /// </summary>
        /// <param name="value">Current controller sprinting value.</param>
        public virtual void SetSprintingParameter(bool value)
        {
            animator.SetBool(isSprintingParameter, value);
        }

        /// <summary>
        /// Set is zooming animator parameter.
        /// </summary>
        /// <param name="value">Current controller zomming value.</param>
        public virtual void SetZoomingParameter(bool value)
        {
            animator.SetBool(isZoomingParameter, value);
        }

        /// <summary>
        /// Update all specified animator parameter.
        /// </summary>
        protected virtual void UpdateAnimatorParameters()
        {
            SetSpeedParameter(playerController.GetSpeed());
            SetGroundedParameter(playerController.IsGrounded());
            SetCrouchedParameter(playerController.IsCrouched());
            SetSprintingParameter(playerController.HasState(ControllerState.Sprinting));
            SetZoomingParameter(playerCamera.IsZooming());
        }

        /// <summary>
        /// Register procedural state events.
        /// </summary>
        private void RegisterProceduralStates()
        {
            for (int i = 0; i < proceduralStates.Length; i++)
            {
                proceduralStates[i].RegisterCallback(CrossFadeAnimation);
            }
        }

        #region [C# Callbacks]
        private void OnFireAction()
        {
            CrossFadeAnimation(fireState);
        }

        private void OnDryFireAction()
        {
            CrossFadeAnimation(dryFireState);
        }

        private void OnFireWithZoomAction()
        {
            if (playerCamera.IsZooming())
            {
                CrossFadeAnimation(fireZoomState);
            }
            else
            {
                CrossFadeAnimation(fireState);
            }
        }

        private void OnDryFireWithZoomAction()
        {
            if (playerCamera.IsZooming())
            {
                CrossFadeAnimation(dryFireZoomState);
            }
            else
            {
                CrossFadeAnimation(dryFireState);
            }
        }

        private void OnJumpAction()
        {
            if (!playerCamera.IsZooming())
            {
                animator.SetTrigger(isJumpedParameter);
            }
        }

        private void OnLandAction()
        {
            if (!playerCamera.IsZooming())
            {
                animator.SetTrigger(isLandedParameter);
            }
        }

        private void OnZoomStartAction()
        {
            animator.CrossFadeInFixedTime(zoomInState.GetNameHash(), playerCamera.GetZoomFOVSettings().GetDuration(), zoomInState.GetLayer());
        }

        private void OnZoomEndAction()
        {
            animator.CrossFadeInFixedTime(zoomOutState.GetNameHash(), playerCamera.GetDefaultFOVSettings().GetDuration(), zoomOutState.GetLayer());
        }
        #endregion

        #region [Event Callback Wrapper]
        private void CrossFadeAnimation(AnimatorState state)
        {
            animator.CrossFadeInFixedTime(state);
        }
        #endregion

        #region [Unity Editor Section]
#if UNITY_EDITOR
        private void OnSetParameterCallback(SerializedProperty property)
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            GenericMenu genericMenu = new GenericMenu();
            for (int i = 0; i < animator.parameterCount; i++)
            {
                AnimatorControllerParameter parameter = animator.parameters[i];
                genericMenu.AddItem(new GUIContent(parameter.name), false, () =>
                {
                    property.FindPropertyRelative("name").stringValue = parameter.name;
                    property.FindPropertyRelative("nameHash").intValue = parameter.nameHash;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            genericMenu.ShowAsContext();
        }
#endif
        #endregion

        #region [Getter / Setter]
        public ProceduralState[] GetProduralStates()
        {
            return proceduralStates;
        }

        public void SetProduralStates(ProceduralState[] value)
        {
            proceduralStates = value;
        }

        public AnimatorParameter GetSpeedParameter()
        {
            return speedParameter;
        }

        public void SetSpeedParameter(AnimatorParameter value)
        {
            speedParameter = value;
        }

        public AnimatorParameter GetIsGroundedParameter()
        {
            return isGroundedParameter;
        }

        public void SetIsGroundedParameter(AnimatorParameter value)
        {
            isGroundedParameter = value;
        }

        public AnimatorParameter GetIsCrouchingParameter()
        {
            return isCrouchingParameter;
        }

        public void SetIsCrouchingParameter(AnimatorParameter value)
        {
            isCrouchingParameter = value;
        }

        public AnimatorParameter GetIsSprintingParameter()
        {
            return isSprintingParameter;
        }

        public void SetIsSprintingParameter(AnimatorParameter value)
        {
            isSprintingParameter = value;
        }

        public AnimatorParameter GetHideParameter()
        {
            return hideParameter;
        }

        public void SetHideParameter(AnimatorParameter value)
        {
            hideParameter = value;
        }
        #endregion
    }
}
