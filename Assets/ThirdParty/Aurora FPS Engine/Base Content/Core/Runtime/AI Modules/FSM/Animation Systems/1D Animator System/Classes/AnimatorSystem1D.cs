/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/FSM/Animation/1D Animator System")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AIController))]
    public class AnimatorSystem1D : MonoBehaviour
    {
        [SerializeField] 
        [Prefix("Float", Style = "Parameter")]
        [Order(-100)]
        private AnimatorParameter speedParameter = "Speed";

        // Stored required component.
        protected Animator animator;
        protected AIController controller;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<AIController>();
        }

        /// <summary>
        /// Called after all Update functions have been called. 
        /// </summary>
        protected virtual void LateUpdate()
        {
            SetSpeedParameter(controller.GetVelocity().magnitude);
        }

        /// <summary>
        /// Set animator speed parameter value.
        /// </summary>
        protected virtual void SetSpeedParameter(float value)
        {
            animator.SetFloat(speedParameter.GetNameHash(), value);
        }

        #region [Getter / Setter]
        public AnimatorParameter GetSpeedParameter()
        {
            return speedParameter;
        }

        public void SetSpeedParameter(AnimatorParameter value)
        {
            speedParameter = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        public AIController GetController()
        {
            return controller;
        }
        #endregion
    }
}