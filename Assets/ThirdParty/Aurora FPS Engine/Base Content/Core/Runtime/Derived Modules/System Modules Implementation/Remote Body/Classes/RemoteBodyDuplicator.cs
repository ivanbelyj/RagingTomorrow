/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using AuroraFPSRuntime.SystemModules.ControllerModules;

namespace AuroraFPSRuntime
{
    [System.Obsolete]
    [RequireComponent(typeof(Animator))]
    public class RemoteBodyDuplicator : MonoBehaviour
    {
        // Base CloneRemoteBody properties.
        [SerializeField] private RemoteBody target;
        
        // Stored required components.
        private Animator animator;
        private FPCharacterController controller;


        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        protected virtual void Start()
        {
            controller = target.GetController();
        }
        
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            ParametersProcessing();
        }

        /// <summary>
        /// Processing remote body animator parameters.
        /// </summary>
        protected virtual void ParametersProcessing()
        {
            animator.SetFloat(target.GetSpeedParameter().GetNameHash(), target.GetDeltaVelocity().y);
            animator.SetFloat(target.GetDirectionParameter().GetNameHash(), target.GetDeltaVelocity().x);
            animator.SetBool(target.GetIsGroundedParameter().GetNameHash(), controller.IsGrounded());
            animator.SetBool(target.GetIsCrouchingParameter().GetNameHash(), controller.IsCrouched());
        }

        #region [Getter / Setter]
        public RemoteBody GetRemoteBodyTarget()
        {
            return target;
        }

        public void SetRemoteBodyTarget(RemoteBody value)
        {
            target = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        protected void SetAnimator(Animator value)
        {
            animator = value;
        }

        public FPCharacterController GetController()
        {
            return controller;
        }

        public void SetController(FPCharacterController value)
        {
            controller = value;
        }
        #endregion
    }
}