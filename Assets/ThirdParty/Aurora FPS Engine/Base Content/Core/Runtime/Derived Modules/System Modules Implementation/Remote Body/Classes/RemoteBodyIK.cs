/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.ControllerModules;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [System.Obsolete]
    [RequireComponent(typeof(Animator))]
    public class RemoteBodyIK : CharacterIKSystem
    {
        // Base remote body IK properties.
        [Header("Remote Body Properties")]
        [SerializeReference] private FPCharacterController controller;

        // Stored required properties.
        private Animator animator;
    
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            controller = transform.GetComponentInParent<FPCharacterController>();
            animator = GetComponent<Animator>();
        }

        #region [CharacterIKSystem Implementation]
        public override Animator GetAnimator()
        {
            return animator;
        }

        public override CapsuleCollider GetCapsuleCollider()
        {
            return null;
        }

        public override Vector3 GetVelocity()
        {
            return controller.GetVelocity();
        }

        #endregion

        #region [Getter / Setter]


        protected void SetAnimator(Animator value)
        {
            animator = value;
        }


        public FPCharacterController GetController()
        {
            return controller;
        }

        protected void SetController(FPCharacterController value)
        {
            controller = value;
        }
        #endregion
    }
}