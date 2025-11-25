/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition/Procedural Transition Handler")]
    [DisallowMultipleComponent]
    [System.Obsolete("Use Group Transition component instead.")]
    public sealed class ProceduralTransitionHandler : MonoBehaviour
    {
        // Stored required components.
        private Transition[] transitions;

        /// <summary>
        /// Сalled when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            transitions = GetComponentsInChildren<Transition>();
        }

        public void FadeIn()
        {
            if(transitions != null)
            {
                for (int i = 0; i < transitions.Length; i++)
                {
                    Transition transition = transitions[i];
                    transition?.FadeIn();
                }
            }
        }

        public void FadeOut()
        {
            if(transitions != null)
            {
                for (int i = 0; i < transitions.Length; i++)
                {
                    Transition transition = transitions[i];
                    transition?.FadeOut();
                }
            }
        }
    }
}
