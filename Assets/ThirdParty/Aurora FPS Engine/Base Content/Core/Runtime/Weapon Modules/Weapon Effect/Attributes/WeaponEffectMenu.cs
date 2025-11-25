/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.WeaponModules
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class WeaponEffectMenu : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public WeaponEffectMenu(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}