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
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public interface IPainterInitialization
    {
        /// <summary>
        /// Called once, before any other decorator calls, 
        /// when the editor becomes active or enabled.
        /// </summary>
        /// <param name="property">Serialized property reference with current decorator attribute.</param>
        /// <param name="attribute">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        void OnInitialize(SerializedProperty property, PainterAttribute attribute, GUIContent label);
    }
}