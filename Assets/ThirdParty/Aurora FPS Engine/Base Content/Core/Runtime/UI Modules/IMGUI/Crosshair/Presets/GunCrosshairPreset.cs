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

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    [ReferenceContent("Gun", "Default Presets/Gun")]
    public class GunCrosshairPreset : CrosshairPreset
    {
        [SerializeField] 
        [Indent(1)]
        private CrosshairElement upElement = new CrosshairElement(null, 3, 15);

        [SerializeField]
        [Indent(1)]
        private CrosshairElement downElement = new CrosshairElement(null, 3, 15);

        [SerializeField]
        [Indent(1)]
        private CrosshairElement leftElement = new CrosshairElement(null, 3, 15);

        [SerializeField]
        [Indent(1)]
        private CrosshairElement rightElement = new CrosshairElement(null, 3, 15);

        [SerializeField]
        [Indent(1)]
        private CrosshairElement centerPoint = new CrosshairElement(null, 2, 2);

        public override void Initialize(PlayerController controller)
        {
            base.Initialize(controller);
            if (upElement.GetTexture() == null)
                upElement.SetTexture(CreateSquadTexture(Color.white));
            if (downElement.GetTexture() == null)
                downElement.SetTexture(CreateSquadTexture(Color.white));
            if (leftElement.GetTexture() == null)
                leftElement.SetTexture(CreateSquadTexture(Color.white));
            if (rightElement.GetTexture() == null)
                rightElement.SetTexture(CreateSquadTexture(Color.white));
            if (centerPoint.GetTexture() == null)
                centerPoint.SetTexture(CreateSquadTexture(Color.white));
        }

        /// <summary>
        /// Draw crosshair elements layout.
        /// </summary>
        /// <param name="spread">Specific spread value calculated by controller state.</param>
        protected override void OnElementsGUI(float spread)
        {
            DrawElement(upElement, new Rect((Screen.width - upElement.GetWidth()) / 2, (Screen.height - spread) / 2 - upElement.GetHeight(), upElement.GetWidth(), upElement.GetHeight()));
            DrawElement(downElement, new Rect((Screen.width - downElement.GetWidth()) / 2, (Screen.height + spread) / 2, downElement.GetWidth(), downElement.GetHeight()));
            DrawElement(leftElement, new Rect((Screen.width - spread) / 2 - leftElement.GetHeight(), (Screen.height - leftElement.GetWidth()) / 2, leftElement.GetHeight(), leftElement.GetWidth()));
            DrawElement(rightElement, new Rect((Screen.width + spread) / 2, (Screen.height - rightElement.GetWidth()) / 2, rightElement.GetHeight(), rightElement.GetWidth()));
            DrawElement(centerPoint, new Rect((Screen.width - centerPoint.GetWidth()) / 2, (Screen.height - centerPoint.GetHeight()) / 2, centerPoint.GetWidth(), centerPoint.GetHeight()));
        }
    }
}