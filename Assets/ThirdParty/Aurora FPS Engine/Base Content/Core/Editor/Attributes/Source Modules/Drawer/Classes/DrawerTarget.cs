/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;

namespace AuroraFPSEditor.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DrawerTarget : Attribute
    {
        public readonly Type target;

        public DrawerTarget(Type target)
        {
            this.target = target;
            SubClasses = false;
        }

        #region [Optional Parameters]
        public bool SubClasses { get; set; }
        #endregion
    }
}