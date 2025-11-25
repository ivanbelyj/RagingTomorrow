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
    public sealed class ValueDropdownAttribute : ViewAttribute
    {
        public readonly string ienumerable;

        public ValueDropdownAttribute(string ienumerable)
        {
            this.ienumerable = ienumerable;
        }
    }
}