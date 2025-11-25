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
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class InlineButtonAttribute : PainterAttribute
    {
        public readonly string name;

        public InlineButtonAttribute(string name)
        {
            this.name = name;
            this.Label = name;
            this.Width = -1.0f;
            this.Style = "Button";
        }

        #region [Parameters]
        /// <summary>
        /// Custom label for button.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Custom width for button.
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Custom style for button.
        /// </summary>
        public string Style { get; set; }
        #endregion
    }
}