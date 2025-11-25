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
    [Flags]
    public enum EditorState
    {
        Editor,
        Play,
        Pause,
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ReadOnlyAttribute : ApexBaseAttribute
    {
        public readonly EditorState state;

        public ReadOnlyAttribute()
        {
            state = EditorState.Editor | EditorState.Play | EditorState.Pause;
        }

        public ReadOnlyAttribute(EditorState state)
        {
            this.state = state;
        }
    }
}