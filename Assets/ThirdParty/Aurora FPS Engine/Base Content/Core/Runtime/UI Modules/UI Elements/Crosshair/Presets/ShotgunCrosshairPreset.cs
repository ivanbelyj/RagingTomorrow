/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;

namespace AuroraFPSRuntime.UIModules.UIElements.Crosshair
{
    [System.Serializable]
    [ReferenceContent("Shotgun", "Default Presets/Shotgun")]
    public class ShotgunCrosshairPreset : CrosshairPreset
    {
        [SerializeField]
        [NotNull]
        private RectTransform element;

        private Vector2 originalSizeDelta;

        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            originalSizeDelta = element.sizeDelta;
        }

        protected override void OnElementsUI(float spread)
        {
            element.sizeDelta = new Vector2(originalSizeDelta.x + spread, originalSizeDelta.y + spread);
        }

        public override void SetVisibility(bool value)
        {
            element.gameObject.SetActive(value);
        }
    }
}