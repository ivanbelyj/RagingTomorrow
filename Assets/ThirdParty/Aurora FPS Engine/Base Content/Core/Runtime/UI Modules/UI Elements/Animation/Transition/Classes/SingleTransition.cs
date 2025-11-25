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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Animation/Transition/Single Transition")]
    [DisallowMultipleComponent]
    public sealed class SingleTransition : Transition
    {
        [SerializeField]
        [NotNull]
        private Graphic element;

        [SerializeField]
        [Slider(0.1f, 1.0f)]
        private float maxAlpha = 1.0f;

        protected override void OnFadeIn(float smooth)
        {
            Color color = element.color;
            color.a = maxAlpha;
            element.color = Color.Lerp(element.color, color, smooth);
        }

        protected override void OnFadeOut(float smooth)
        {
            Color color = element.color;
            color.a = 0.0f;
            element.color = Color.Lerp(element.color, color, smooth);
        }

        #region [Getter / Setter]
        public Graphic GetElement()
        {
            return element;
        }

        public void SetElement(Graphic value)
        {
            element = value;
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
