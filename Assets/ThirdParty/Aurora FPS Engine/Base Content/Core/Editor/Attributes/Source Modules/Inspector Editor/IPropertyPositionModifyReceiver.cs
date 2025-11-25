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
    public interface IPropertyPositionModifyReceiver
    {
        /// <summary>
        /// Called before drawing target property, for changing property Rectangle position.
        /// </summary>
        /// <param name="position">Target property position.</param>
        void ModifyPropertyPosition(ref Rect position);
    }
}