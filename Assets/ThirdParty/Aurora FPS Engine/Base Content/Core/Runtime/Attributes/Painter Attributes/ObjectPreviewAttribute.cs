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
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ObjectPreviewAttribute : PainterAttribute
    {
        public ObjectPreviewAttribute()
        {
            Height = 150.0f;
            Expandable = false;
        }

        #region [Parameters]
        /// <summary>
        /// Height of the preview window.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Hided in expandable foldout.
        /// </summary>
        public bool Expandable { get; set; }
        #endregion

    }
}