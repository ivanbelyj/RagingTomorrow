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
    [PainterTarget(typeof(IndentAttribute))]
    public sealed class IndentPainter : PropertyPainter
    {
        private int level;
        private bool following;
        private int previousLevel;

        /// <summary>
        /// Called once, when initializing PropertyPainter.
        /// </summary>
        /// <param name="property">Serialized property reference with current painter attribute.</param>
        /// <param name="painterAttribute">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnInitialize(SerializedProperty property, PainterAttribute attribute, GUIContent label)
        {
            IndentAttribute indentAttribute = attribute as IndentAttribute;
            level = indentAttribute.level;
            following = indentAttribute.following;
        }

        public override void BeforePropertyGUI()
        {
            previousLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = level;
        }

        public override void AfterPropertyGUI()
        {
            if (!following)
                EditorGUI.indentLevel = previousLevel;
        }
    }
}