/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [AddComponentMenu(null)]
    [RequireComponent(typeof(Animator))]
    public abstract class InverseKinematic : MonoBehaviour, IInverseKinematic
    {
        protected Animator animator;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Callback for setting up animation IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (IsActive())
            {
                OnCalculateIK(layerIndex);
            }
        }

        /// <summary>
        /// Callback for calculation animation IK.
        /// </summary>
        /// <param name="layerIndex">Index of the layer on which the IK solver is called.</param>
        protected abstract void OnCalculateIK(int layerIndex);

        #region [IInverseKinematic Implementation]
        /// <summary>
        /// Return true if IK is processing, otherwise false.
        /// </summary>
        public virtual bool IsActive()
        {
            return enabled && animator != null;
        }
        #endregion

        #region [Getter / Setter]
        public Animator GetAnimator()
        {
            return animator;
        }
        #endregion
    }
}