/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AIBehaviourMenuAttribute : Attribute
    {
        public readonly string Name;
        public readonly string Path;

        public AIBehaviourMenuAttribute(string name, string path)
        {
            Name = name;
            Path = path;
            Hide = false;
        }

        #region [Optional Options]
        public bool Hide { get; set; }
        #endregion
    }
}

