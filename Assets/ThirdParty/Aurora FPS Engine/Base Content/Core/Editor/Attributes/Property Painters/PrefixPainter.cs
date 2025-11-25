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
    [PainterTarget(typeof(PrefixAttribute))]
    public sealed class PrefixPainter : PropertyPainter, IPropertyValidatorReceiver, IPropertyPositionModifyReceiver
    {
        public const float LabelSpace = 0.5f;

        private PrefixAttribute attribute;
        private float prefixWidth;
        private float previousLabelWidth;
        private GUIStyle style;

        /// <summary>
        /// Called once, before any other painter calls, 
        /// when the editor becomes active or enabled.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {
            attribute = painterAttribute as PrefixAttribute;
            prefixWidth = -1;
        }

        /// <summary>
        /// Called for drawing GUI elements.
        /// </summary>
        /// <param name="position">Current position which has been modified by other painters, if this property contains other painter attributes.</param>
        public void ModifyPropertyPosition(ref Rect position)
        {
            if (prefixWidth == -1)
            {
                GUIContent content = new GUIContent(attribute.label);
                if (attribute.Style == "Parameter")
                {
                    style = new GUIStyle(EditorStyles.helpBox);
                    style.alignment = TextAnchor.MiddleCenter;
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 11;
                    style.contentOffset = new Vector2(0, -0.5f);
                }
                else
                {
                    style = new GUIStyle(attribute.Style);
                }
                prefixWidth = style.CalcSize(content).x;
            }

            previousLabelWidth = EditorGUIUtility.labelWidth;

            if (attribute.beforeProperty)
            {
                EditorGUIUtility.labelWidth -= prefixWidth;
                Padding widthPadding = new Padding(0.0f, 0.0f, prefixWidth, 0.0f);
                position = widthPadding.PaddingRect(position);
            }
            else
            {
                EditorGUIUtility.labelWidth += prefixWidth + LabelSpace;
                position.width -= 0.5f;
            }
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

            Rect prefixPosition = new Rect(position.x, propertyPosition.y, prefixWidth, propertyPosition.height);
            if (attribute.beforeProperty)
            {
                prefixPosition.x -= prefixWidth;
            }
            else
            {
                prefixPosition.x += EditorGUIUtility.labelWidth - prefixWidth - LabelSpace;
                prefixPosition.x += 1;
            }

            GUI.Label(prefixPosition, attribute.label, style);

            EditorGUIUtility.labelWidth = previousLabelWidth;
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            if (property.isArray || property.hasVisibleChildren)
            {
                if(property.propertyType == SerializedPropertyType.String || ApexReflection.HasPropertyDrawer(property))
                {
                    return true;
                }

                return false;
            }
            return true;
        }
    }
}