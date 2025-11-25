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
    [PainterTarget(typeof(LabelWidthAttribute))]
    public sealed class LabelWidthPainter : PropertyPainter
    {
        private LabelWidthAttribute attribute;
        private float previousWidth;

        /// <summary>
        /// Called once, before any other painter calls, 
        /// when the editor becomes active or enabled.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute painterAttribute, GUIContent label)
        {
            attribute = painterAttribute as LabelWidthAttribute;
        }

        public override void BeforePropertyGUI()
        {
            previousWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = attribute.width;
        }

        public override void AfterPropertyGUI()
        {
            EditorGUIUtility.labelWidth = previousWidth;
        }
    }
}