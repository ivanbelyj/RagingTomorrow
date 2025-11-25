/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(MinMaxSliderAttribute))]
    public sealed class MinMaxSliderView : PropertyView, IPropertyValidatorReceiver
    {
        private MinMaxSliderAttribute minMaxSliderAttribute;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            minMaxSliderAttribute = viewAttribute as MinMaxSliderAttribute;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);

            Rect[] splitRect = ApexEditorUtilities.SplitRect(position, 3);

            int padding = (int)splitRect[0].width - 41 - (EditorGUI.indentLevel * 17);
            int space = 3;

            splitRect[0].width -= padding + space;
            splitRect[2].width -= padding + space;
            splitRect[1].x -= padding;
            splitRect[1].width += padding * 2;
            splitRect[2].x += padding + space - 1;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    Vector2 vector = property.vector2Value;
                    vector.x = EditorGUI.FloatField(splitRect[0], vector.x);
                    if (vector.x < minMaxSliderAttribute.min)
                        vector.x = minMaxSliderAttribute.min;
                    else if (vector.x > vector.y)
                        vector.x = vector.y;

                    vector.y = EditorGUI.FloatField(splitRect[2], vector.y);
                    if (vector.y > minMaxSliderAttribute.max)
                        vector.y = minMaxSliderAttribute.max;
                    else if (vector.y < vector.x)
                        vector.y = vector.x;

                    EditorGUI.MinMaxSlider(splitRect[1], ref vector.x, ref vector.y, minMaxSliderAttribute.min, minMaxSliderAttribute.max);

                    property.vector2Value = vector;
                    break;
                case SerializedPropertyType.Vector2Int:
                    int min = Convert.ToInt32(minMaxSliderAttribute.min);
                    int max = Convert.ToInt32(minMaxSliderAttribute.max);

                    Vector2Int vectorInt = property.vector2IntValue;
                    vectorInt.x = EditorGUI.IntField(splitRect[0], vectorInt.x);
                    if (vectorInt.x < min)
                        vectorInt.x = min;
                    else if (vectorInt.x > vectorInt.y)
                        vectorInt.x = vectorInt.y;

                    vectorInt.y = EditorGUI.IntField(splitRect[2], vectorInt.y);
                    if (vectorInt.y > max)
                        vectorInt.y = max;
                    else if (vectorInt.y < vectorInt.x)
                        vectorInt.y = vectorInt.x;

                    float xInt = vectorInt.x;
                    float yInt = vectorInt.y;
                    EditorGUI.MinMaxSlider(splitRect[1], ref xInt, ref yInt, min, max);
                    vectorInt.x = Convert.ToInt32(xInt);
                    vectorInt.y = Convert.ToInt32(yInt);

                    property.vector2IntValue = vectorInt;
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector2Int;
        }
    }
}