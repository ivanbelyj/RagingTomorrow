/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public abstract class ApexField
    {
        public abstract void DrawFieldLayout();

        public abstract void DrawField(Rect position);

        public abstract float GetFieldHeight();

        public virtual bool IsVisible()
        {
            return true;
        }
    }
}