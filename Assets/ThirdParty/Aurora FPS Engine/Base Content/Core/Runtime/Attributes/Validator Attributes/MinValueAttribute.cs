/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MinValueAttribute : ValidatorAttribute
    {
        public readonly float value;
        public readonly string property;

        public MinValueAttribute(float value)
        {
            this.value = value;
        }

        public MinValueAttribute(string property)
        {
            this.property = property;
        }
    }
}