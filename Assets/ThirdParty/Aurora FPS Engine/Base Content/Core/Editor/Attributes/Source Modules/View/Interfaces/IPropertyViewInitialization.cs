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
    public interface IPropertyViewInitialization
    {
        /// <summary>
        /// Called once when initializing PropertyView.
        /// </summary>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="viewAttribute">ViewAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label);
    }
}