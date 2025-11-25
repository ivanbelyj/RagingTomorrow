/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Remote Body/Remote Body Ragdoll")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class RemoteBodyRagdoll : DynamicRagdoll
    {
        [SerializeField]
        [NotNull]
        private Transform ragdollBody;

        // Stored required properties.
        private Vector3 localPosition;
        private Quaternion localRotation;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            localPosition = ragdollBody.localPosition;
            localRotation = ragdollBody.localRotation;
            OnBlendCompleteCallback += ResetTransform;
        }

        /// <summary>
        /// Override this method to initialize animator component of the ragdoll character.
        /// Use GetComponent<Animator>() method.
        /// </summary>
        /// <param name="animator">Animator component of the ragdoll character.</param>
        protected override void CopyAnimator(out Animator animator)
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Override this method to initialize transform component of the ragdoll character.
        /// </summary>
        /// <param name="transform">Transform component of the ragdoll character.</param>
        protected override void CopyRagdollTransform(out Transform transform)
        {
            if(ragdollBody == null)
            {
                ragdollBody = this.transform;
            }
            transform = ragdollBody;
        }

        /// <summary>
        /// Calculate body direction, when character get up.
        /// </summary>
        /// <returns>Body direction when character get up.</returns>
        protected override Vector3 CalculateBodyDirection()
        {
            return transform.forward;
        }

        /// <summary>
        /// Fix character position when character blended to animator.
        /// </summary>
        public virtual void ResetTransform()
        {
            if (StateIs(RagdollState.Animated))
            {
                ragdollBody.localPosition = localPosition;
                ragdollBody.localRotation = localRotation;
            }
        }
    }
}