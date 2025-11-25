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
    /// <summary>
    /// Completely change how the property is drawing and how you interact with it.
    /// </summary>
    public abstract class PropertyView : IPropertyViewInitialization, IPropertyView, IPropertyViewHeight
    {
        /// <summary>
        /// Called once when initializing PropertyView.
        /// </summary>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="viewAttribute">ViewAttribute of serialized property.</param>
        /// <param name="label">Label of serialized property.</param>
        public virtual void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {

        }

        /// <summary>
        /// Called for drawing property view GUI.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public abstract void OnGUI(Rect position, SerializedProperty property, GUIContent label);

        /// <summary>
        /// Get height which needed to draw property.
        /// </summary>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}