/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSRuntime.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(ValueDropdownAttribute))]
    public sealed class ValueDropdownView : PropertyView
    {
        private object target;
        private FieldInfo fieldInfo;
        private MethodInfo ienumerableMethod;
        private PropertyInfo ienumerableProperty;
        private GenericMenu genericMenu;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            ValueDropdownAttribute dropdownValuesAttribute = viewAttribute as ValueDropdownAttribute;
            target = ApexReflection.GetDeclaringObjectOfProperty(property);
            fieldInfo = ApexReflection.GetField(target, property.name);

            MethodInfo[] methodInfos = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];
                if (methodInfo.Name == dropdownValuesAttribute.ienumerable)
                {
                    ienumerableMethod = methodInfo;
                    return;
                }
            }

            PropertyInfo[] propertyInfos = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                if (propertyInfo.Name == dropdownValuesAttribute.ienumerable)
                {
                    ienumerableProperty = propertyInfo;
                    return;
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            string nameofObjectValue = GetNameOfObject(property, fieldInfo.GetValue(target));
            if (string.IsNullOrEmpty(nameofObjectValue))
            {
                nameofObjectValue = "None";
            }
            if (GUI.Button(position, nameofObjectValue, EditorStyles.popup))
            {
                genericMenu = new GenericMenu();
                int count = 0;
                IEnumerable enumerable = null;
                if (ienumerableMethod != null)
                {
                    enumerable = ienumerableMethod.Invoke(target, null) as IEnumerable;
                }
                else if (ienumerableProperty != null)
                {
                    enumerable = ienumerableProperty.GetValue(target, null) as IEnumerable;
                }

                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        genericMenu.AddItem(new GUIContent(GetNameOfObject(property, item)), false, () =>
                        {
                            property.serializedObject.Update();
                            fieldInfo.SetValue(target, item);
                            property.serializedObject.ApplyModifiedProperties();
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        });
                        count++;
                    }
                    if (count == 0)
                    {
                        genericMenu.AddDisabledItem(new GUIContent("No content available..."));
                        genericMenu.AddSeparator("");

                        genericMenu.AddItem(new GUIContent("Set To Default"), false, () =>
                        {
                            property.serializedObject.Update();
                            fieldInfo.SetValue(target, null);
                            property.serializedObject.ApplyModifiedProperties();
                            EditorUtility.SetDirty(property.serializedObject.targetObject);
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        });
                    }
                    genericMenu.DropDown(position);
                }
            }
        }

        private string GetNameOfObject(SerializedProperty property, object item)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    Array array = item as Array;
                    if (array == null)
                    {
                        return item?.ToString() ?? "None";
                    }
                    else
                    {
                        List<string> elements = new List<string>();
                        for (int i = 0; i < array.Length; i++)
                        {
                            elements.Add(array.GetValue(i).ToString());
                        }
                        string elementsString = string.Join(", ", elements);
                        return $"{array.GetType().Name}, ({elementsString})";
                    }
                case SerializedPropertyType.Integer:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Boolean:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Float:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.String:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Color:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.ObjectReference:
                    Object objectReference = item as Object;
                    return objectReference?.name ?? $"None ({fieldInfo.FieldType.Name})";
                case SerializedPropertyType.LayerMask:
                    LayerMask layerMask = (LayerMask)item;
                    return layerMask.value.ToString();
                case SerializedPropertyType.Enum:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Vector2:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Vector3:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Vector4:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Rect:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.ArraySize:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Character:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.AnimationCurve:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Bounds:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Gradient:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Quaternion:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.ExposedReference:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.FixedBufferSize:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Vector2Int:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.Vector3Int:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.RectInt:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.BoundsInt:
                    return item?.ToString() ?? "None";
                case SerializedPropertyType.ManagedReference:
                    return item?.GetType().Name ?? "Null";
            }
            return "Undefined type...";
        }
    }
}