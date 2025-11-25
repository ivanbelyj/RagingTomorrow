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
    [PainterTarget(typeof(HorizontalLineAttribute))]
    public sealed class HorizontalLinePainter : PropertyPainter
    {
        private HorizontalLineAttribute attribute;
        private Color color;
        private float height;

        /// <summary>
        /// Called once, when initializing PropertyPainter.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {
            attribute = painterAttribute as HorizontalLineAttribute;
            height = attribute.width + attribute.space;
            color = new Color(attribute.r, attribute.g, attribute.b, attribute.a);
        }

        /// <summary>
        /// Called for drawing GUI elements.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the painter GUI.</param>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect propertyPosition = GetPropertyPosition();
            Rect linePosition = new Rect(propertyPosition.x, position.y + attribute.space, propertyPosition.width, attribute.width);
            EditorGUI.DrawRect(linePosition, color);
        }

        /// <summary>
        /// Get the height of the painter, which required to display it.
        /// Calculate only the size of the current painter, not the entire property.
        /// The painter height will be added to the total size of the property with other painters.
        /// </summary>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override float GetPainterHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }
    }
}