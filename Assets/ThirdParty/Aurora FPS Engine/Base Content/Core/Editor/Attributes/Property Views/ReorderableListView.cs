/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(ReorderableListAttribute))]
    public sealed class ReorderableListView : PropertyView
    {
        private const string ElementNameArg_Index = "{index}";
        private const string ElementNameArg_NiceIndex = "{niceIndex}";
        private const string ClearButtonLabel = "Clear";

        private ReorderableListAttribute attribute;
        private ReorderableList list;
        private List<ApexProperty> children;
        private int selectedIndex;
        private bool isIgnoreElementType;

        private object target;
        private MethodInfo onElementGUICallback;
        private MethodInfo onNoneElementGUICallback;
        private MethodInfo getElementHeightCallback;
        private MethodInfo getElementLabelCallback;
        private MethodInfo onAddElementCallback;
        private MethodInfo onDropdownButtonCallback;
        private MethodInfo onRemoveElementCallback;
        private MethodInfo onHeaderGUICallback;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            attribute = viewAttribute as ReorderableListAttribute;

            InitializeChildren(property);

            target = ApexReflection.GetDeclaringObjectOfProperty(property);

            Type type = target.GetType();
            onElementGUICallback = GetElementGUICallback(type, attribute.OnElementGUICallback);
            onNoneElementGUICallback = GetNoneElementGUICallback(type, attribute.OnNoneElementGUICallback);
            getElementHeightCallback = GetElementHeightCallback(type, attribute.GetElementHeightCallback);
            getElementLabelCallback = GetElementLabelCallback(type, attribute.GetElementLabelCallback);
            onAddElementCallback = GetAddElementCallback(type, attribute.OnAddCallbackCallback);
            onRemoveElementCallback = GetRemoveElementCallback(type, attribute.OnRemoveCallbackCallback);
            onDropdownButtonCallback = GetDropDownButtonCallback(type, attribute.OnDropdownButtonCallback);
            onHeaderGUICallback = GetHeaderGUICallback(type, attribute.OnHeaderGUICallback);

            list = CreateReorderableList(property, label);

            ApexSettings apexSettings = ApexSettings.Current;
            isIgnoreElementType = apexSettings.GetDefaultTypes().Any(t => t == property.arrayElementType);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            list?.DoList(position);
        }

        public ReorderableList CreateReorderableList(SerializedProperty property, GUIContent label)
        {
            list = new ReorderableList(property.serializedObject, property, attribute.Draggable, attribute.DisplayHeader, true, true);

            if (!attribute.DisplayHeader)
            {
                list.headerHeight = 1.5f;
            }
            else if (!string.IsNullOrEmpty(attribute.OnHeaderGUICallback) && onHeaderGUICallback != null)
            {
                list.drawHeaderCallback = (rect) =>
                {
                    onHeaderGUICallback.Invoke(target, new object[] { rect });
                };
            }
            else if (attribute.DrawClearButton)
            {
                list.drawHeaderCallback = (Rect position) =>
                {
                    Rect labelPosition = new Rect(position.x, position.y, position.width - 47, position.height);
                    EditorGUI.LabelField(labelPosition, label);

                    Rect clearButtonPosition = new Rect(position.x + labelPosition.width, position.y + 0.5f, 50, 16);
                    if (GUI.Button(clearButtonPosition, ClearButtonLabel))
                    {
                        property.ClearArray();
                    }
                };
            }
            else
            {
                list.drawHeaderCallback = (Rect position) =>
                {
                    Rect labelPosition = new Rect(position.x, position.y, position.width, position.height);
                    EditorGUI.LabelField(labelPosition, label);
                };
            }

            list.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                if (children.Count <= index)
                {
                    InitializeChildren(property);
                }

                if (isActive || isFocused)
                {
                    selectedIndex = index;
                }

                if(attribute.DisplaySeparator && index > 0)
                {
                    EditorGUI.DrawRect(new Rect(position.x - 20, position.yMin, position.width + 27, 1.0f), Color.black);
                    position.y += 2;
                }

                position.y += 1f;
                position.height = EditorGUIUtility.singleLineHeight;
                SerializedProperty element = property.GetArrayElementAtIndex(index);
                GUIContent content = new GUIContent(string.Format("Element {0}", index));
                if (getElementLabelCallback != null)
                {
                    object[] parameters = new object[2] { element, index };
                    content.text = (string)getElementLabelCallback.Invoke(target, parameters);
                }
                else if (!string.IsNullOrEmpty(attribute.ElementLabel))
                {
                    content.text = GetElementName(attribute.ElementLabel, index);
                }
                else
                {
                    content = GUIContent.none;
                }

                ApexProperty child = children[index];
                if (onElementGUICallback != null)
                {
                    object[] parameters = new object[3] { position, element, content };
                    onElementGUICallback.Invoke(target, parameters);
                }
                else if (child.HasCustomDrawer() || child.TargetSerializedProperty.propertyType == SerializedPropertyType.ObjectReference || child.TargetSerializedProperty.propertyType == SerializedPropertyType.String || isIgnoreElementType || !child.HasChildren())
                {
                    child.DrawField(position, content);
                }
                else 
                {
                    Rect contentPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                    if (!attribute.RightFoldoutToggle)
                    {
                        element.isExpanded = ApexEditorUtilities.IndentFoldoutGUI(contentPosition, element.isExpanded, content, true);
                    }
                    else
                    {
                        EditorGUI.LabelField(contentPosition, content);

                        Rect foldoutPosition = new Rect(contentPosition.xMax - (element.isExpanded ? 0 : 30), contentPosition.y + 1, 15, EditorGUIUtility.singleLineHeight);
                        if (element.isExpanded)
                            GUI.matrix = Matrix4x4.identity;
                        else
                            GUIUtility.RotateAroundPivot(180, foldoutPosition.center);
                        element.isExpanded = ApexEditorUtilities.IndentFoldoutGUI(foldoutPosition, element.isExpanded, GUIContent.none, true);
                        GUI.matrix = Matrix4x4.identity;
                    }
                    if (element.isExpanded)
                    {
                        contentPosition.y += EditorGUIUtility.standardVerticalSpacing;
                        contentPosition.x += 14;
                        contentPosition.width -= 14;

                        Rect childPosition = new Rect(contentPosition.x, contentPosition.yMax, contentPosition.width, contentPosition.height);
                        if (child.HasChildren())
                            child.DrawChildren(childPosition);
                        else
                            child.DrawField(childPosition);
                    }
                }
            };

            list.elementHeightCallback = (int index) =>
            {
                if (children.Count <= index)
                {
                    InitializeChildren(property);
                }

                if (getElementHeightCallback != null)
                {
                    object[] parameters = new object[2] { property, label };
                    return (float)getElementHeightCallback.Invoke(target, parameters);
                }
                return children[index].GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing + (attribute.DisplaySeparator ? 2 : 0);
            };

            list.onAddCallback = (list) =>
            {
                if (onAddElementCallback != null)
                {
                    onAddElementCallback.Invoke(target, new object[1] { property });
                }
                {
                    ReorderableList.defaultBehaviours.DoAddButton(list);
                }

                InitializeChildren(property);
            };

            list.onRemoveCallback = (list) =>
            {
                if (onRemoveElementCallback != null)
                {
                    onRemoveElementCallback.Invoke(target, new object[2] { property, selectedIndex });
                }
                else
                {
                    ReorderableList.defaultBehaviours.DoRemoveButton(list);
                }
                InitializeChildren(property);
            };

            if (onDropdownButtonCallback != null)
            {
                list.onAddDropdownCallback = (rect, list) =>
                {
                    onDropdownButtonCallback.Invoke(target, new object[2] { rect, property });
                };
            }

            list.drawNoneElementCallback = (rect) =>
            {
                if (onNoneElementGUICallback != null)
                {
                    onNoneElementGUICallback.Invoke(target, new object[1] { rect });
                }
                else
                {
                    if (string.IsNullOrEmpty(attribute.NoneElementLabel))
                    {
                        ReorderableList.defaultBehaviours.DrawNoneElement(rect, attribute.Draggable);
                    }
                    else
                    {
                        EditorGUI.LabelField(rect, attribute.NoneElementLabel);
                    }
                }
            };

            return list;
        }

        private void InitializeChildren(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            children = new List<ApexProperty>(property.arraySize);
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty child = property.GetArrayElementAtIndex(i);
                ApexProperty apexProperty = new ApexProperty(child);
                children.Add(apexProperty);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return list.GetHeight() + EditorGUIUtility.standardVerticalSpacing;
        }

        public string GetElementName(string elementLabel, int index)
        {
            if (elementLabel.Contains(ElementNameArg_Index))
                elementLabel = elementLabel.Replace(ElementNameArg_Index, index.ToString());
            else if (elementLabel.Contains(ElementNameArg_NiceIndex))
                elementLabel = elementLabel.Replace(ElementNameArg_NiceIndex, (index + 1).ToString());
            return elementLabel;
        }

        public MethodInfo GetElementGUICallback(Type type, string methodName)
        {
            if(ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 3)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "Rect" &&
                            parameters[1].ParameterType.Name == "SerializedProperty" &&
                            parameters[2].ParameterType.Name == "GUIContent")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetHeaderGUICallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 1)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "Rect")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetElementHeightCallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 2 && method.ReturnType.Name == "Single")
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty" &&
                            parameters[1].ParameterType.Name == "GUIContent")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetNoneElementGUICallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 1)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "Rect")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetElementLabelCallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 2 && method.ReturnType.Name == "String")
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty" &&
                            parameters[1].ParameterType.Name == "Int32")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetDropDownButtonCallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 2)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "Rect" &&
                            parameters[1].ParameterType.Name == "SerializedProperty")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetAddElementCallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 1)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }

        public MethodInfo GetRemoveElementCallback(Type type, string methodName)
        {
            if (ApexReflection.TryDeepFindMethods(type, methodName, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.Name == methodName && method.GetParameters().Length == 2)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty" &
                            parameters[1].ParameterType.Name == "Int32")
                        {
                            return method;
                        }
                    }
                }
            }
            return null;
        }
    }
}