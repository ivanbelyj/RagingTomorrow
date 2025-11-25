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
    [ValidatorTarget(typeof(MinValueAttribute))]
    public class MinValueValidator : PropertyValidator, IPropertyValidatorReceiver
    {
        private SerializedProperty validateProperty;
        private float minValue_Single;
        private int minValue_Int32;

        public override void OnInitialize(SerializedProperty property, ValidatorAttribute validatorAttribute, GUIContent label)
        {
            MinValueAttribute minValueAttribute = validatorAttribute as MinValueAttribute;
            if (string.IsNullOrEmpty(minValueAttribute.property))
            {
                minValue_Single = minValueAttribute.value;
                minValue_Int32 = Convert.ToInt32(minValueAttribute.value);
            }
            else
            {
                validateProperty = property.serializedObject.FindProperty(minValueAttribute.property);

            }
        }

        public override void Validate(SerializedProperty property)
        {
            if (validateProperty != null)
            {
                switch (validateProperty.propertyType)
                {
                    case SerializedPropertyType.Integer:
                        minValue_Int32 = validateProperty.intValue;
                        break;
                    case SerializedPropertyType.Float:
                        minValue_Single = validateProperty.floatValue;
                        break;
                }
            }

            if (property.propertyType == SerializedPropertyType.Float && property.floatValue < minValue_Single)
            {
                property.floatValue = minValue_Single;
            }
            else if (property.propertyType == SerializedPropertyType.Integer && property.intValue < minValue_Int32)
            {
                property.intValue = minValue_Int32;
            }
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property decorator attribute.</param>
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