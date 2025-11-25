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
    public sealed class BottomButtonAttribute : PainterAttribute
    {
        public readonly string name;

        public BottomButtonAttribute(string name)
        {
            this.name = name;
            this.Label = name;
            this.Group = null;
            this.Height = 20.0f;
            this.Style = "Button";
        }

        #region [Parameters]
        /// <summary>
        /// Custom name for button.
        /// Use the @ prefix to indicate, that a texture will be used instead of the name.
        /// Arguments: @{Default Unity Icon Name}, @{Path to texture}
        /// Example: @_Popup, @Assets/...
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Horizontal group name.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Custom button height.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Custom button style.
        /// </summary>
        public string Style { get; set; }
        #endregion
    }
}