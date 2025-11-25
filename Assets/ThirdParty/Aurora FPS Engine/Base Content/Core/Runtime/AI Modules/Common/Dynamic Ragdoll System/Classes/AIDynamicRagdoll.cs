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

namespace AuroraFPSRuntime.AIModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/Common/Dynamic Ragdoll/AI Dynamic Ragdoll")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public sealed class AIDynamicRagdoll : DynamicRagdoll
    {
        /// <summary>
        /// Override this method to return animator component of the ragdoll character.
        /// Use GetComponent<Animator>() method.
        /// </summary>
        /// <param name="animator">Animator component of the ragdoll character.</param>
        protected override void CopyAnimator(out Animator animator)
        {
            animator = GetComponent<Animator>();
        }
    }
}