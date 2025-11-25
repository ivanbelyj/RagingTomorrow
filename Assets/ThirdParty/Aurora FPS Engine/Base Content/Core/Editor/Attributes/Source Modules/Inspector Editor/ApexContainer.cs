/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public class ApexContainer : ApexProperty
    {
        private Padding padding;

        public ApexContainer(SerializedProperty target, Padding padding) : base(target)
        {
            this.padding = padding;
        }

        #region [Getter / Setter]
        public Padding GetPadding()
        {
            return padding;
        }

        public void SetPadding(Padding value)
        {
            padding = value;
        }
        #endregion
    }
}