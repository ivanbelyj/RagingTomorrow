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
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public abstract class ConditionAttribute : ApexBaseAttribute
    {
        public readonly string FirstProperty;
        public readonly string SecondProperty;
        public readonly bool BoolCondition;
        public readonly string NumericCondition;

        public ConditionAttribute(string propertyName)
        {
            FirstProperty = propertyName;
            BoolCondition = true;
        }

        public ConditionAttribute(string propertyName, bool condition)
        {
            FirstProperty = propertyName;
            BoolCondition = condition;
        }

        public ConditionAttribute(string firstProperty, string condition, string secondProperty)
        {
            FirstProperty = firstProperty;
            NumericCondition = condition;
            SecondProperty = secondProperty;
        }

        public ConditionAttribute(string enumProperty, string enumValue)
        {
            FirstProperty = enumProperty;
            SecondProperty = enumValue;
        }
    }
}