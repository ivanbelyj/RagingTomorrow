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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ReferenceContent : ApexBaseAttribute
    {
        public readonly string name;
        public readonly string path;

        public ReferenceContent(string name, string path)
        {
            this.name = name;
            this.path = path;

            Hided = false;
        }

        #region [Getter / Setter]
        public bool Hided { get; set; }
        #endregion
    }
}
