/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.AIModules.Conditions
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ConditionMenuAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public ConditionMenuAttribute(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }

        #region [Optional]
        public string Description { get; set; }
        #endregion
    }
}

