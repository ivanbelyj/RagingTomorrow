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
    [PainterTarget(typeof(MessageAttribute))]
    public sealed class MessagePanter : PropertyPainter
    {
        public const float LineWidth = 2.5f;
        public const int TextSize = 11;
        public const float MessageSpace = 3.0f;

        private string text;
        private Color lineColor;
        private Color highlightColor;
        private float height;
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
            MessageAttribute messageAttribute = painterAttribute as MessageAttribute;
            text = messageAttribute.text;
            CalculateColor(messageAttribute.messageStyle, out lineColor, out highlightColor);
        }

        /// <summary>
        /// Called for drawing GUI elements.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the painter GUI.</param>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect propertyPosition = EditorGUI.IndentedRect(GetPropertyPosition());

            if (height == 0)
            {
                InitializeStyle();
                CalculateHeight(text, style, out height);
                style.fontSize = TextSize;
            }

            Rect linePosition = new Rect(propertyPosition.x, position.y + MessageSpace, LineWidth, height);
            EditorGUI.DrawRect(linePosition, lineColor);

            Rect highligthPosition = new Rect(linePosition.x + linePosition.width, linePosition.y, propertyPosition.width - linePosition.width, height);
            EditorGUI.DrawRect(highligthPosition, highlightColor);

            Rect textPosition = new Rect(linePosition.x + (LineWidth + LineWidth), linePosition.y, propertyPosition.width, height);
            GUI.Label(textPosition, text, style);
        }

        /// <summary>
        /// Get the height of the painter, which required to display it.
        /// Calculate only the size of the current painter, not the entire property.
        /// The painter height will be added to the total size of the property with other painter.
        /// </summary>
        /// <param name="property">Reference of serialized property painter attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public override float GetPainterHeight(SerializedProperty property, GUIContent label)
        {
            if (height == 0)
            {
                InitializeStyle();
                CalculateHeight(text, style, out height);
                style.fontSize = TextSize;
            }
            return height + MessageSpace;
        }

        private void CalculateHeight(string text, GUIStyle style, out float height)
        {
            GUIContent content = new GUIContent(text);
            height = style.CalcSize(content).y;
        }

        private void CalculateColor(MessageStyle messageStyle, out Color lineColor, out Color highlightColor)
        {
            switch (messageStyle)
            {
                case MessageStyle.Info:
                    lineColor = Color.gray;
                    highlightColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
                    break;
                case MessageStyle.Warning:
                    lineColor = new Color(1.0f, 0.7f, 0.3f, 1.0f);
                    highlightColor = new Color(1.0f, 0.7f, 0.3f, 0.1f);
                    break;
                case MessageStyle.Error:
                    lineColor = new Color(1.0f, 0.07f, 0.125f, 1.0f);
                    highlightColor = new Color(1.0f, 0.07f, 0.125f, 0.1f);
                    break;
                default:
                    lineColor = Color.gray;
                    highlightColor = new Color(1.0f, 1.0f, 1.0f, 0.1f);
                    break;
            }
        }

        private void InitializeStyle()
        {
            style = new GUIStyle(GUI.skin.label)
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleLeft
            };
        }
    }
}