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
    [ViewTarget(typeof(ToggleLeftAttribute))]
    public sealed class ToggleLeftView : PropertyView, IPropertyValidatorReceiver
    {
        /// <summary>
        /// Called every inspector update to draw property.
        /// </summary>
        /// <param name="position">Position of the serialized property.</param>
        /// <param name="property">Serialized property with ViewAttribute.</param>
        /// <param name="label">Label of serialized property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Boolean;
        }
    }
}