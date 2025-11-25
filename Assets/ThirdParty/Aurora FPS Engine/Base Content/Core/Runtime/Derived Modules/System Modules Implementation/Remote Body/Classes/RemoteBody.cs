/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    [System.Obsolete]
    [RequireComponent(typeof(Animator))]
    public class RemoteBody : MonoBehaviour
    {
        // Remote body properties.
        [Header("Body Properties")]
        [SerializeField] private FPCharacterController controller;

        [Header("Animation Properties")]
        [SerializeField] private AnimatorParameter speedParameter = "Speed";
        [SerializeField] private AnimatorParameter directionParameter = "Direction";
        [SerializeField] private AnimatorParameter isGroundedParameter = "IsGrounded";
        [SerializeField] private AnimatorParameter isCrouchingParameter = "IsCrouching";
        [SerializeField] private float velocitySmooth = 0.9f;

        // Stored required components.
        private Animator animator;

        // Stored required properties.
        private Vector3 deltaVelocity;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Called only once during the lifetime of the script instance and after all objects are initialized
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            VelocityPrcessing();
            ParametersProcessing();
        }

        protected virtual void VelocityPrcessing()
        {
            if (controller.IsEnabled())
            {
                Vector3 worldDeltaPosition = controller.GetVelocity();
                float dx = Vector3.Dot(transform.right, worldDeltaPosition);
                float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
                Vector3 targetDeltaVelocity = new Vector2(dx, dy);
                deltaVelocity = Vector3.Lerp(deltaVelocity, targetDeltaVelocity, velocitySmooth * Time.deltaTime);
            }
            else
            {
                deltaVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Processing remote body animator parameters.
        /// </summary>
        protected virtual void ParametersProcessing()
        {
            animator.SetFloat(speedParameter.GetNameHash(), deltaVelocity.y);
            animator.SetFloat(directionParameter.GetNameHash(), deltaVelocity.x);
            animator.SetBool(isGroundedParameter.GetNameHash(), controller.IsGrounded());
            animator.SetBool(isCrouchingParameter.GetNameHash(), controller.IsCrouched());
        }

        #region [Getter / Setter]
        public FPCharacterController GetController()
        {
            return controller;
        }

        public void SetController(FPCharacterController value)
        {
            controller = value;
        }

        public Vector3 GetDeltaVelocity()
        {
            return deltaVelocity;
        }

        public void SetDeltaVelocity(Vector3 value)
        {
            deltaVelocity = value;
        }

        public AnimatorParameter GetSpeedParameter()
        {
            return speedParameter;
        }

        public void SetSpeedParameter(AnimatorParameter value)
        {
            speedParameter = value;
        }

        public AnimatorParameter GetDirectionParameter()
        {
            return directionParameter;
        }

        public void SetDirectionParameter(AnimatorParameter value)
        {
            directionParameter = value;
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

        public float GetVelocitySmooth()
        {
            return velocitySmooth;
        }

        public void SetVelocitySmooth(float value)
        {
            velocitySmooth = value;
        }

        public Animator GetAnimator()
        {
            return animator;
        }

        protected void SetAnimator(Animator value)
        {
            animator = value;
        }
        #endregion
    }
}