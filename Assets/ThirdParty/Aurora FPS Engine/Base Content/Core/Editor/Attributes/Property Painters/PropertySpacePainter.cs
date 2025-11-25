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
    [PainterTarget(typeof(PropertySpaceAttribute))]
    public sealed class PropertySpacePainter : PropertyPainter
    {
        private float space;

        /// <summary>
        /// Called once, before any other decorator calls, 
        /// when the editor becomes active or enabled.
        /// </summary>
        /// <param name="property">Serialized property reference with current decorator attribute.</param>
        /// <param name="attribute">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute attribute, GUIContent label)
        {
            PropertySpaceAttribute propertySpaceAttribute = attribute as PropertySpaceAttribute;
            space = propertySpaceAttribute.space;
        }

        /// <summary>
        /// Get the height of the decorator, which required to display it.
        /// Calculate only the size of the current decorator, not the entire property.
        /// The decorator height will be added to the total size of the property with other decorators.
        /// </summary>
        /// <param name="property">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override float GetPainterHeight(SerializedProperty property, GUIContent label)
        {
            return space;
        }
    }
}