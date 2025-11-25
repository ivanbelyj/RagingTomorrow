/* ================================================================
   ----------------------------------------------------------------
   Project   :    Apex Inspector
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
    [ViewTarget(typeof(SliderAttribute))]
    public sealed class SliderView : PropertyView, IPropertyValidatorReceiver
    {
        private SliderAttribute attribute;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            attribute = viewAttribute as SliderAttribute;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    OnIntegerSliderGUI(position, property, label);
                    break;
                case SerializedPropertyType.Float:
                    OnFloatSliderGUI(position, property, label);
                    break;
            }
            
        }

        public void OnIntegerSliderGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minProperty = !string.IsNullOrEmpty(attribute.minProperty) ? property.FindPropertyRelativeParent(attribute.minProperty) : null;
            int min = minProperty != null && minProperty.propertyType == SerializedPropertyType.Integer ? minProperty.intValue : System.Convert.ToInt32(attribute.minValue);

            SerializedProperty maxProperty = !string.IsNullOrEmpty(attribute.maxProperty) ? property.FindPropertyRelativeParent(attribute.maxProperty) : null;
            int max = maxProperty != null && maxProperty.propertyType == SerializedPropertyType.Integer ? maxProperty.intValue : System.Convert.ToInt32(attribute.maxValue);

            property.intValue = EditorGUI.IntSlider(position, label, property.intValue, min, max);
        }

        public void OnFloatSliderGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty minProperty = !string.IsNullOrEmpty(attribute.minProperty) ? property.FindPropertyRelativeParent(attribute.minProperty) : null;
            float min = minProperty != null && minProperty.propertyType == SerializedPropertyType.Float ? minProperty.floatValue : attribute.minValue;

            SerializedProperty maxProperty = !string.IsNullOrEmpty(attribute.maxProperty) ? property.FindPropertyRelativeParent(attribute.maxProperty) : null;
            float max = maxProperty != null && maxProperty.propertyType == SerializedPropertyType.Float ? maxProperty.floatValue : attribute.maxValue;

            property.floatValue = EditorGUI.Slider(position, label, property.floatValue, min, max);
        }

        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Float || property.propertyType == SerializedPropertyType.Integer;
        }
    }
}