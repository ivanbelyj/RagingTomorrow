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
    [ReferenceContent("Gun", "Default Presets/Gun")]
    public class GunCrosshairPreset : CrosshairPreset
    {
        [SerializeField]
        [NotNull]
        private RectTransform upElement;

        [SerializeField]
        [NotNull]
        private RectTransform downElement;

        [SerializeField]
        [NotNull]
        private RectTransform leftElement;

        [SerializeField]
        [NotNull]
        private RectTransform rightElement;

        private Vector2 originalPositionUpElement;
        private Vector2 originalPositionDownElement;
        private Vector2 originalPositionLeftElement;
        private Vector2 originalPositionRightElement;

        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            originalPositionUpElement = upElement.anchoredPosition;
            originalPositionDownElement = downElement.anchoredPosition;
            originalPositionLeftElement = leftElement.anchoredPosition;
            originalPositionRightElement = rightElement.anchoredPosition;
        }

        /// <summary>
        /// Draw crosshair elements layout.
        /// </summary>
        /// <param name="spread">Specific spread value calculated by controller state.</param>
        protected override void OnElementsUI(float spread)
        {
            upElement.anchoredPosition = new Vector2(originalPositionUpElement.x, originalPositionUpElement.y + spread);
            downElement.anchoredPosition = new Vector2(originalPositionDownElement.x, originalPositionDownElement.y - spread);
            leftElement.anchoredPosition = new Vector2(originalPositionLeftElement.x - spread, originalPositionLeftElement.y);
            rightElement.anchoredPosition = new Vector2(originalPositionRightElement.x + spread, originalPositionRightElement.y);
        }

        public override void SetVisibility(bool value)
        {
            upElement.gameObject.SetActive(value);
            downElement.gameObject.SetActive(value);
            leftElement.gameObject.SetActive(value);
            rightElement.gameObject.SetActive(value);
        }
    }
}