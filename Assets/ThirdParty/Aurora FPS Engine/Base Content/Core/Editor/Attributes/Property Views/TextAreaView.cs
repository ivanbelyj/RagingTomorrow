/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © Tamerlan Shakirov 2020 All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEditor;
using TextAreaAttribute = AuroraFPSRuntime.Attributes.TextAreaAttribute;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(TextAreaAttribute))]
    public sealed class TextAreaView : PropertyView
    {
        private Vector2 scrollPosition;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            GUIStyle style = new GUIStyle(EditorStyles.textArea);
            float height = style.CalcHeight(new GUIContent(property.stringValue), position.width);
            Rect viewPosition = new Rect(position.x, position.y, position.width, height);

            scrollPosition = GUI.BeginScrollView(viewPosition, scrollPosition, position);
            property.stringValue = EditorGUI.TextArea(position, property.stringValue);
            GUI.EndScrollView();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string[] lines = property.stringValue.Split('\n');
            float height = 36 * lines.Length;
            return Mathf.Min(height, 90);
        }
    }
}