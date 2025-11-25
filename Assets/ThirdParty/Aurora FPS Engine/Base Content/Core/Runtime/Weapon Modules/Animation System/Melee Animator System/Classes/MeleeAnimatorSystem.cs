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
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Animation/Melee Animator System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class MeleeAnimatorSystem : EquippableObjectAnimationSystem
    {
        [System.Serializable]
        public class StateEvent : CallbackEvent<AnimatorState> { }

        [SerializeField]
        [Foldout("Default Parameters", Style = "Header")]
        private AnimatorParameter speedParameter = "Speed";

        [SerializeField]
        [Foldout("Default Parameters", Style = "Header")]
        private AnimatorParameter isGroundedParameter = "IsGrounded";

        [SerializeField]
        [Foldout("Default Parameters", Style = "Header")]
        private AnimatorParameter isCrouchingParameter = "IsCrouching";

        [SerializeField]
        [Foldout("Default Parameters", Style = "Header")]
        private AnimatorParameter isSprintingParameter = "IsSprinting";

        [SerializeField]
        [Foldout("Default Parameters", Style = "Header")]
        private AnimatorParameter pushParameter = "Push";

        [SerializeField]
        [ReorderableList(DisplayHeader = false, DisplaySeparator = true)]
        [Foldout("Custom State Events")]
        private StateEvent[] stateEvents;

        // Stored required properties.
        private Animator animator;
        private PlayerController controller;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            animator = GetComponent<Animator>();
            controller = GetComponentInParent<PlayerController>();
            RegisterStateEvents();
        }

        private void RegisterStateEvents()
        {
            for (int i = 0; i < stateEvents.Length; i++)
            {
                stateEvents[i].RegisterCallback(PlayAnimationState);
            }
        }

        private void PlayAnimationState(AnimatorState state)
        {
            animator.CrossFadeInFixedTime(state);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            animator.SetFloat(speedParameter, controller.GetSpeed());
            animator.SetBool(isGroundedParameter, controller.IsGrounded());
            animator.SetBool(isCrouchingParameter, controller.IsCrouched());
            animator.SetBool(isSprintingParameter, controller.HasState(SystemModules.ControllerSystems.ControllerState.Sprinting));
        }

        /// <summary>
        /// Empty implementation.
        /// Weapon pull animation playing automatically as entry state in animator.
        /// </summary>
        public override void PlayPullAnimation() { }

        public override void PlayPushAnimation()
        {
            animator.SetTrigger(pushParameter);
        }
    }
}