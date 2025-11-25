/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class HealthFunctionMenu : Attribute
    {
        public readonly string name;
        public readonly string path;

        public HealthFunctionMenu(string name, string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}