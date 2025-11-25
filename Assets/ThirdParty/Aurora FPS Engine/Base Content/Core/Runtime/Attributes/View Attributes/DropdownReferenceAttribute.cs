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
    public sealed class DropdownReferenceAttribute : ViewAttribute
    {
        public DropdownReferenceAttribute()
        {
            FoldoutToggle = true;
        }


        #region [Optional Options]
        public bool FoldoutToggle { get; set; }
        #endregion
    }
}