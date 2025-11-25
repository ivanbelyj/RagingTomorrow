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
    [PainterTarget(typeof(SuffixAttribute))]
    public sealed class SuffixPainter : PropertyPainter, IPropertyPositionModifyReceiver, IPropertyValidatorReceiver
    {
        public const float SuffixLableHorizontalOffset = 1.0f;

        private SuffixAttribute attribute;
        private GUIStyle style;
        private float suffixWidth = -1;

        /// <summary>
        /// Called once, before any other painter calls, 
        /// when the editor becomes active or enabled.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {
            attribute = painterAttribute as SuffixAttribute;
        }

        /// <summary>
        /// Called for drawing GUI elements.
        /// </summary>
        /// <param name="position">Current position which has been modified by other painters, if this property contains other painter attributes.</param>
        public void ModifyPropertyPosition(ref Rect position)
        {
            if (suffixWidth == -1)
            {
                style = new GUIStyle(GUI.skin.label);
                if (attribute.ItalicText)
                {
                    style.fontStyle = FontStyle.Italic;
                }
                GUIContent suffixContent = new GUIContent(attribute.label);
                suffixWidth = style.CalcSize(suffixContent).x;
            }
            position.width -= suffixWidth + SuffixLableHorizontalOffset;
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
            Rect suffixLabelPosition = new Rect(position.x + position.width + SuffixLableHorizontalOffset, propertyPosition.y, suffixWidth, propertyPosition.height);

            EditorGUI.BeginDisabledGroup(attribute.muted);
            GUI.Label(suffixLabelPosition, attribute.label, style);
            EditorGUI.EndDisabledGroup();
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
                if (property.propertyType == SerializedPropertyType.String || ApexReflection.HasPropertyDrawer(property) || ApexReflection.HasPropertyView(property))
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }
}