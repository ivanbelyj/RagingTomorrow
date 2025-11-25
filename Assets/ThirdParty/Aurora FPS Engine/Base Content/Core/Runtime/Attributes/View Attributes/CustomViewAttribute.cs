/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.Attributes
{
    public sealed class CustomViewAttribute : ViewAttribute
    {
        public string ViewInitialization { get; set; }
        public string ViewGUI { get; set; }
        public string ViewHeight { get; set; }
    }
}