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
    public class MaxValueAttribute : ValidatorAttribute
    {
        public readonly float value;
        public readonly string property;

        public MaxValueAttribute(float value)
        {
            this.value = value;
        }

        public MaxValueAttribute(string property)
        {
            this.property = property;
            this.value = 100;
        }
    }
}