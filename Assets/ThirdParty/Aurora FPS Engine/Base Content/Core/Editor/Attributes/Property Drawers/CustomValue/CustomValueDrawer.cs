/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEditor;
using AuroraFPSRuntime.CoreModules.ValueTypes;

namespace AuroraFPSEditor.Attributes
{
    [CustomPropertyDrawer(typeof(CustomValue))]
    internal class CustomValueDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueType = property.FindPropertyRelative("valueType");
            SerializedProperty numberValue = property.FindPropertyRelative("numberValue");
            SerializedProperty stringValue = property.FindPropertyRelative("stringValue");
            SerializedProperty axesValue = property.FindPropertyRelative("axesValue");
            SerializedProperty objectValue = property.FindPropertyRelative("objectValue");

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect foldoutPosition = new Rect(position.x, position.y, position.width, singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, GUIContent.none, false);
            if (property.isExpanded)
            {
                int lastEnumValueIndex = valueType.enumValueIndex;
                Rect valueTypePosition = new Rect(position.x, foldoutPosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                valueTypePosition = EditorGUI.PrefixLabel(valueTypePosition, new GUIContent("Type"));
                EditorGUI.PropertyField(valueTypePosition, valueType, GUIContent.none);
                if(lastEnumValueIndex != valueType.enumValueIndex)
                {
                    numberValue.floatValue = CustomValue.DefalutNumber;
                    stringValue.stringValue = CustomValue.DefalutString;
                    axesValue.quaternionValue = CustomValue.DefalutAxes;
                    objectValue.objectReferenceValue = CustomValue.DefalutObject;
                }
                switch (valueType.enumValueIndex)
                {
                    case 0:
                        {
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            int value = System.Convert.ToInt32(numberValue.floatValue);
                            value = EditorGUI.IntField(valuePosition, value);
                            numberValue.floatValue = value;
                        }
                        break;
                    case 1:
                        {
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            numberValue.floatValue = EditorGUI.FloatField(valuePosition, numberValue.floatValue);
                        }
                        break;
                    case 2:
                        {
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            stringValue.stringValue = EditorGUI.TextField(valuePosition, stringValue.stringValue);
                        }
                        break;
                    case 3:
                        {
                            bool value = System.Convert.ToBoolean(numberValue.floatValue);
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            value = EditorGUI.Toggle(valuePosition, value);
                            numberValue.floatValue = System.Convert.ToSingle(value);
                        }
                        break;
                    case 4:
                        {
                            Vector2 value = new Vector2(axesValue.quaternionValue.x, axesValue.quaternionValue.y);
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            value = EditorGUI.Vector2Field(valuePosition, GUIContent.none, value);
                            axesValue.quaternionValue = new Quaternion(value.x, value.y, 0, 0);
                        }
                        break;
                    case 5:
                        {
                            Vector3 value = new Vector3(axesValue.quaternionValue.x, axesValue.quaternionValue.y, axesValue.quaternionValue.z);
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            value = EditorGUI.Vector3Field(valuePosition, GUIContent.none, value);
                            axesValue.quaternionValue = new Quaternion(value.x, value.y, value.z, 0);
                        }
                        break;
                    case 6:
                        {
                            Vector4 value = new Vector4(axesValue.quaternionValue.x, axesValue.quaternionValue.y, axesValue.quaternionValue.z, axesValue.quaternionValue.w);
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            value = EditorGUI.Vector4Field(valuePosition, GUIContent.none, value);
                            axesValue.quaternionValue = new Quaternion(value.x, value.y, value.z, value.w);
                        }
                        break;
                    case 7:
                        {
                            Rect valuePosition = new Rect(position.x, valueTypePosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                            valuePosition = EditorGUI.PrefixLabel(valuePosition, new GUIContent("Value"));
                            objectValue.objectReferenceValue = EditorGUI.ObjectField(valuePosition, GUIContent.none, objectValue.objectReferenceValue, typeof(Object), false);
                        }
                        break;
                }
                
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight * 3 + (EditorGUIUtility.standardVerticalSpacing * 2);
            }
            else
            {
                return EditorGUIUtility.singleLineHeight;
            }
        }
    }
}
