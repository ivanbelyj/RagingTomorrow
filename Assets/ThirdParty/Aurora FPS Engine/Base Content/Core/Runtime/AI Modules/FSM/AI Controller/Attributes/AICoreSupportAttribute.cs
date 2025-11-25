/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.AIModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AICoreSupportAttribute : Attribute
    {
        public readonly Type target;

        public AICoreSupportAttribute(Type target)
        {
            this.target = target;
        }
    }
}