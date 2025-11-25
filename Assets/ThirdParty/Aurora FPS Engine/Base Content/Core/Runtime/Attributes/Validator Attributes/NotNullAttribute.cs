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
    public enum MessageBoxSize
    {
        Inline,
        Small,
        Medium,
        Big
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class NotNullAttribute : ValidatorAttribute
    {
        public const string NameArgument = "{name}";
        public const string DefaultMessageFormat = "{name} cannot be null!";

        public NotNullAttribute()
        {
            Format = DefaultMessageFormat;
            Size = MessageBoxSize.Small;
        }

        #region [Parameters]
        /// <summary>
        /// Custom message format. 
        /// Arguments: {name}
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Size of message box.
        /// </summary>
        public MessageBoxSize Size { get; set; }
        #endregion
    }
}