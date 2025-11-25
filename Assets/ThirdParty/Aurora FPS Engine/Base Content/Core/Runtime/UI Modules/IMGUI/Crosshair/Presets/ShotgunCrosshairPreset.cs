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

namespace AuroraFPSRuntime.UIModules.IMGUI.Crosshair
{
    [System.Serializable]
    [ReferenceContent("Shotgun", "Default Presets/Shotgun")]
    public class ShotgunCrosshairPreset : CrosshairPreset
    {
        [SerializeField]
        [Indent(1)]
        private CrosshairElement element = new CrosshairElement(null, 0, 0);

        protected override void OnElementsGUI(float spread)
        {
            DrawElement(element, new Rect((Screen.width - (element.GetWidth() + spread)) / 2, (Screen.height - (element.GetHeight() + spread)) / 2, element.GetWidth() + spread, element.GetHeight() + spread));
        }
    }
}