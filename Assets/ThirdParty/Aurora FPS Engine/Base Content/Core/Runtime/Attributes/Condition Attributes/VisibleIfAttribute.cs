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
    public sealed class VisibleIfAttribute : ConditionAttribute
    {
        public VisibleIfAttribute(string propertyName) : base(propertyName)
        {
        }

        public VisibleIfAttribute(string propertyName, bool condition) : base(propertyName, condition)
        {
        }

        public VisibleIfAttribute(string enumProperty, string enumValue) : base(enumProperty, enumValue)
        {
        }

        public VisibleIfAttribute(string firstProperty, string condition, string secondProperty) : base(firstProperty, condition, secondProperty)
        {
        }
    }
}