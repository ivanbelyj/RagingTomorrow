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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DefaultStringAttribute : ApexBaseAttribute
    {
        public readonly string Property;
        public readonly string DefaultValue;

        public DefaultStringAttribute(string property, string defaultValue)
        {
            Property = property;
            DefaultValue = defaultValue;
        }
    }
}