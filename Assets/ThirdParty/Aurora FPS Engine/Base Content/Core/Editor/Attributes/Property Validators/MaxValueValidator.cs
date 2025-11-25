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
    [ValidatorTarget(typeof(MaxValueAttribute))]
    public class MaxValueValidator : PropertyValidator, IPropertyValidatorReceiver
    {
        private SerializedProperty validateProperty;
        private float maxValue_Single;
        private int maxValue_Int32;

        public override void OnInitialize(SerializedProperty property, ValidatorAttribute validatorAttribute, GUIContent label)
        {
            MaxValueAttribute maxValueAttribute = validatorAttribute as MaxValueAttribute;
            if (string.IsNullOrEmpty(maxValueAttribute.property))
            {
                maxValue_Single = maxValueAttribute.value;
                maxValue_Int32 = Convert.ToInt32(maxValueAttribute.value);
            }
            else
            {
                validateProperty = property.serializedObject.FindProperty(maxValueAttribute.property);
            }
        }

        public override void Validate(SerializedProperty property)
        {
            if(validateProperty != null)
            {
                switch (validateProperty.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        maxValue_Int32 = validateProperty.intValue;
                        break;
                    case SerializedPropertyType.Float:
                        maxValue_Single = validateProperty.floatValue;
                        break;
                }
            }

            if (property.propertyType == SerializedPropertyType.Float && property.floatValue > maxValue_Single)
            {
                property.floatValue = maxValue_Single;
            }
            else if (property.propertyType == SerializedPropertyType.Integer && property.intValue > maxValue_Int32)
            {
                property.intValue = maxValue_Int32;
            }
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Sserialized property of current attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Float ||
                property.propertyType == SerializedPropertyType.Integer)
            {
                return true;
            }
            return false;
        }
    }
}