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
    public sealed class ActiveIfAttribute : ConditionAttribute
    {
        public ActiveIfAttribute(string propertyName) : base(propertyName)
        {
        }

        public ActiveIfAttribute(string propertyName, bool condition) : base(propertyName, condition)
        {
        }

        public ActiveIfAttribute(string firstProperty, string condition, string secondProperty) : base(firstProperty, condition, secondProperty)
        {
        }
    }
}