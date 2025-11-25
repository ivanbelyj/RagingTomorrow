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
    public sealed class FoldoutAttribute : LayoutAttribute
    {
        public FoldoutAttribute(string name) : base(name)
        {
            Style = null;
        }

        #region [Optional Options]
        public string Style { get; set; }
        #endregion
    }
}