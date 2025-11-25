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
    [ValidatorTarget(typeof(NotNullAttribute))]
    public sealed class NotNullValidator : PropertyValidator, IPropertyValidatorReceiver
    {
        public const float SmallHeight = 20;
        public const float MediumHeight = 25;
        public const float BigHeight = 35;

        private NotNullAttribute attribute;
        private bool isNull;
        private float height;

        public override void OnInitialize(SerializedProperty property, ValidatorAttribute validatorAttribute, GUIContent label)
        {
            attribute = validatorAttribute as NotNullAttribute;
            switch (this.attribute.Size)
            {
                case MessageBoxSize.Small:
                    this.height = SmallHeight;
                    break;
                case MessageBoxSize.Medium:
                    this.height = MediumHeight;
                    break;
                case MessageBoxSize.Big:
                    this.height = BigHeight;
                    break;
            }
        }

        /// <summary>
        /// Called every inspector update time before drawing property.
        /// </summary>
        /// <param name="property">Serialized property with ValidatorAttribute.</param>
        public override void Validate(SerializedProperty property)
        {
            isNull = property.objectReferenceValue == null;
        }

        public override void OnValidatorGUI(Rect originalPosition, Rect validatorPosition, SerializedProperty property, GUIContent label)
        {
            if (isNull)
            {
                string message = attribute.Format;
                if (message.Contains(NotNullAttribute.NameArgument))
                {
                    message = message.Replace(NotNullAttribute.NameArgument, property.displayName);
                }

                originalPosition = EditorGUI.IndentedRect(originalPosition);
                Rect helpboxPosition = new Rect(originalPosition.x, validatorPosition.y + 2, originalPosition.width, height);
                EditorGUI.HelpBox(helpboxPosition, message, MessageType.Error);
            }
        }

        public override float GetValidatorHeight(SerializedProperty property, GUIContent label)
        {
            return isNull ? height + EditorGUIUtility.standardVerticalSpacing : 0;
        }

        /// <summary>
        /// Return true if this property valid the using with this attribute.
        /// If return false, this property attribute will be ignored.
        /// </summary>
        /// <param name="property">Reference of serialized property decorator attribute.</param>
        /// <param name="label">Display label of serialized property.</param>
        public bool IsValidProperty(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference;
        }
    }
}