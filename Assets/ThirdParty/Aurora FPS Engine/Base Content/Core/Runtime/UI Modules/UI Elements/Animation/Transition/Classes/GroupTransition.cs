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
using UnityEngine.UI;

namespace AuroraFPSRuntime.UIModules.UIElements.Animation
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition/Group Transition")]
    [DisallowMultipleComponent]
    public sealed class GroupTransition : Transition
    {
        [SerializeField]
        [NotNull]
        private CanvasGroup group;

        [SerializeField]
        [Slider(0.1f, 1.0f)]
        private float maxAlpha = 1.0f;

        [SerializeField]
        private bool handleInteraction = true;

        /// <summary>
        /// Called on the frame when a script is enabled,
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (handleInteraction)
            {
                SetInteract(group.alpha != 0);
                OnFadeInCompleteCallback += () => SetInteract(true);
                OnFadeOutCompleteCallback += () => SetInteract(false);
            }
        }

        /// <summary>
        /// Implement this method to make fade in logic.
        /// </summary>
        /// <param name="smooth">Smooth interpolation value.</param>
        protected override void OnFadeIn(float smooth)
        {
            group.alpha = Mathf.Lerp(group.alpha, maxAlpha, smooth);
        }

        /// <summary>
        /// Implement this method to make fade out logic.
        /// </summary>
        /// <param name="smooth">Smooth interpolation value.</param>
        protected override void OnFadeOut(float smooth)
        {
            if (handleInteraction)
            {
                SetInteract(false);
            }
            group.alpha = Mathf.Lerp(group.alpha, 0.0f, smooth);

        }

        private void SetInteract(bool value)
        {
            group.interactable = value;
            group.blocksRaycasts = value;
        }

        #region [Getter / Setter]
        public CanvasGroup GetGroup()
        {
            return group;
        }

        public void SetGroup(CanvasGroup value)
        {
            group = value;
        }

        public float GetMaxAlpha()
        {
            return maxAlpha;
        }

        public void SetMaxAlpha(float value)
        {
            maxAlpha = value;
        }
        #endregion
    }
}