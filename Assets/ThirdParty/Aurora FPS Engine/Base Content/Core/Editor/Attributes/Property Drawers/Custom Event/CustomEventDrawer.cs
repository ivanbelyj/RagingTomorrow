/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using AuroraFPSRuntime.CoreModules.TypeExtensions;
using Object = UnityEngine.Object;

namespace AuroraFPSEditor.Attributes
{
    [DrawerTarget(typeof(CustomEvent))]
    internal sealed class CustomEventDrawer : PropertyDrawer
    {
        private SerializedProperty serializedType;
        private SerializedProperty serializedFunction;
        private SerializedProperty serializedParameters;

        private Type selectedType;
        private string[] parametersName;
        private Dictionary<int, Type> parametersType;
        private GUIStyle typeStyle = null;

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            serializedType = property.FindPropertyRelative("type");
            serializedFunction = property.FindPropertyRelative("function");
            serializedParameters = property.FindPropertyRelative("parameters");

            if (!string.IsNullOrEmpty(serializedType.stringValue))
            {
                selectedType = ApexReflection.FindType(serializedType.stringValue);
                if (selectedType != null)
                {
                    if (!string.IsNullOrEmpty(serializedFunction.stringValue))
                    {
                        if (ApexReflection.TryDeepFindMethods(selectedType, serializedFunction.stringValue, out MethodInfo[] methods))
                        {
                            bool methodIsFinded = false;
                            for (int i = 0; i < methods.Length; i++)
                            {
                                MethodInfo method = methods[i];
                                bool isValidParameters = true;
                                ParameterInfo[] parameters = method.GetParameters();
                                if (parameters.Length == serializedParameters.arraySize)
                                {
                                    for (int j = 0; j < parameters.Length; j++)
                                    {
                                        ParameterInfo parameter = parameters[j];
                                        if (!CustomEvent.Parameter.IsValidType(parameter.ParameterType))
                                        {
                                            isValidParameters = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    isValidParameters = false;
                                }

                                if (isValidParameters && parameters.Length > 0)
                                {
                                    parametersName = new string[parameters.Length];
                                    parametersType = new Dictionary<int, Type>();
                                    for (int j = 0; j < parameters.Length; j++)
                                    {
                                        ParameterInfo parameter = parameters[j];
                                        string parameterName = parameter.Name;
                                        parameterName = parameterName.ToTitle();
                                        parametersName[j] = parameterName;

                                        if (parameter.ParameterType.IsSubclassOf(typeof(Object)))
                                        {
                                            parametersType.Add(j, parameter.ParameterType);
                                        }
                                    }
                                    methodIsFinded = true;
                                    break;
                                }
                            }

                            if (!methodIsFinded)
                            {
                                ResetValues();
                            }
                        }
                    }

                }
                else
                {
                    ResetValues();
                }
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeParameterTypeStyle();

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;


            Rect typeLabelPosition = EditorGUI.PrefixLabel(new Rect(position.x, position.y, position.width, singleLineHeight), new GUIContent("Component"));
            Rect typePopupPosition = new Rect(typeLabelPosition.x, typeLabelPosition.y, position.x + position.width - typeLabelPosition.x, typeLabelPosition.height);
            if (GUI.Button(typePopupPosition, GetComponentDropdownLabel(), EditorStyles.popup))
            {
                SearchableMenu componentsMenu = CreateComponentsMenu();
                componentsMenu.ShowAsDropdown(typePopupPosition, new Vector2(typePopupPosition.width, 163));
            }

            if (selectedType != null)
            {
                Rect functionLabelPosition = EditorGUI.PrefixLabel(new Rect(position.x, typeLabelPosition.yMax + standardVerticalSpacing, position.width, singleLineHeight), new GUIContent("Function"));
                Rect functionPopupPosition = new Rect(functionLabelPosition.x, functionLabelPosition.y, functionLabelPosition.x + functionLabelPosition.width - functionLabelPosition.x, singleLineHeight);
                if (GUI.Button(functionPopupPosition, GetFunctionDropdownLabel(), EditorStyles.popup))
                {
                    SearchableMenu methodsMenu = CreateMethodsMenu(selectedType);
                    methodsMenu.ShowAsDropdown(functionPopupPosition, new Vector2(functionPopupPosition.width, 163));
                }

                if (serializedParameters.arraySize > 0)
                {
                    Rect foldoutPosition = new Rect(position.x, functionLabelPosition.yMax + standardVerticalSpacing, position.width, singleLineHeight);
                    serializedParameters.isExpanded = EditorGUI.Foldout(foldoutPosition, serializedParameters.isExpanded, "Parameters", true);
                    if (serializedParameters.isExpanded)
                    {
                        Rect parameterPosition = new Rect(position.x, foldoutPosition.y, position.width, singleLineHeight);
                        parameterPosition = EditorGUI.IndentedRect(parameterPosition);
                        for (int i = 0; i < serializedParameters.arraySize; i++)
                        {
                            parameterPosition.y += singleLineHeight + standardVerticalSpacing;
                            Rect parameterLabelPosition = EditorGUI.PrefixLabel(parameterPosition, new GUIContent(parametersName[i]));
                            Rect parameterTypePosition = new Rect(parameterLabelPosition.x, parameterLabelPosition.y, 65, singleLineHeight);
                            Rect parameterFieldPosition = new Rect(parameterTypePosition.xMax + 2, parameterTypePosition.y, parameterLabelPosition.width - 67, singleLineHeight);
                            SerializedProperty serializedParameter = serializedParameters.GetArrayElementAtIndex(i);

                            SerializedProperty serializedParameterType = serializedParameter.FindPropertyRelative("parameterType");
                            switch (serializedParameterType.enumValueIndex)
                            {
                                case 0:
                                    SerializedProperty serializedFloatParameter = serializedParameter.FindPropertyRelative("floatParameter");
                                    EditorGUI.LabelField(parameterTypePosition, "Float", typeStyle);
                                    EditorGUI.PropertyField(parameterFieldPosition, serializedFloatParameter, GUIContent.none);
                                    break;
                                case 1:
                                    SerializedProperty serializedIntegerParameter = serializedParameter.FindPropertyRelative("integerParameter");
                                    EditorGUI.LabelField(parameterTypePosition, "Integer", typeStyle);
                                    EditorGUI.PropertyField(parameterFieldPosition, serializedIntegerParameter, GUIContent.none);
                                    break;
                                case 2:
                                    SerializedProperty serializedBooleanParameter = serializedParameter.FindPropertyRelative("booleanParameter");
                                    EditorGUI.LabelField(parameterTypePosition, "Boolean", typeStyle);
                                    EditorGUI.PropertyField(parameterFieldPosition, serializedBooleanParameter, GUIContent.none);
                                    break;
                                case 3:
                                    SerializedProperty serializedStringParameter = serializedParameter.FindPropertyRelative("stringParameter");
                                    EditorGUI.LabelField(parameterTypePosition, "String", typeStyle);
                                    EditorGUI.PropertyField(parameterFieldPosition, serializedStringParameter, GUIContent.none);
                                    break;
                                case 4:
                                    SerializedProperty serializedVector3Parameter = serializedParameter.FindPropertyRelative("vector3Parameter");
                                    EditorGUI.LabelField(parameterTypePosition, "Vector3", typeStyle);
                                    EditorGUI.PropertyField(parameterFieldPosition, serializedVector3Parameter, GUIContent.none);
                                    break;
                                case 5:
                                    SerializedProperty serializedObjectParameter = serializedParameter.FindPropertyRelative("objectParameter");
                                    EditorGUI.LabelField(parameterTypePosition, "Object", typeStyle);
                                    serializedObjectParameter.objectReferenceValue = EditorGUI.ObjectField(parameterFieldPosition, serializedObjectParameter.objectReferenceValue, parametersType[i], false);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (serializedParameters.arraySize > 0)
            {
                float height = 60;
                if (serializedParameters.isExpanded)
                {
                    height += serializedParameters.arraySize * 20;
                }
                return height;
            }
            else if(selectedType != null)
            {
                return 38;
            }
            else
            {
                return 18;
            }
        }

        private void ResetValues()
        {
            serializedType.stringValue = null;
            serializedFunction.stringValue = null;
            serializedParameters.arraySize = 0;
            serializedType.serializedObject.ApplyModifiedProperties();
        }

        private void InitializeParameterTypeStyle()
        {
            if (typeStyle == null)
            {
                typeStyle = new GUIStyle(EditorStyles.helpBox);
                typeStyle.alignment = TextAnchor.MiddleCenter;
                typeStyle.fontStyle = FontStyle.Bold;
                typeStyle.fontSize = 11;
                typeStyle.contentOffset = new Vector2(0, -0.5f);
            }
        }

        private SearchableMenu CreateComponentsMenu()
        {
            IEnumerable<Type> types = ApexReflection.GetAllSubTypes(typeof(Component));
            SearchableMenu componentsMenu = new SearchableMenu();
            foreach (Type type in types)
            {
                componentsMenu.AddItem(new GUIContent(type.Name), true, () =>
                {
                    selectedType = type;
                    serializedType.stringValue = type.Name;
                    serializedFunction.stringValue = null;
                    serializedParameters.arraySize = 0;
                    serializedType.serializedObject.ApplyModifiedProperties();
                });
            }
            return componentsMenu;
        }

        private SearchableMenu CreateMethodsMenu(Type type)
        {
            SearchableMenu componentsMenu = new SearchableMenu();
            if (type != null)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (CustomEvent.IsValidMethod(method))
                    {
                        bool isValidParameters = true;
                        ParameterInfo[] parameters = method.GetParameters();
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            ParameterInfo parameter = parameters[j];
                            if (!CustomEvent.Parameter.IsValidType(parameter.ParameterType))
                            {
                                isValidParameters = false;
                                break;
                            }
                        }

                        if (isValidParameters)
                        {
                            componentsMenu.AddItem(method.ToContent(), true, () =>
                            {
                                serializedFunction.stringValue = method.Name;
                                serializedParameters.arraySize = parameters.Length;
                                parametersName = new string[parameters.Length];
                                parametersType = new Dictionary<int, Type>();
                                for (int j = 0; j < parameters.Length; j++)
                                {
                                    SerializedProperty serializedParameter = serializedParameters.GetArrayElementAtIndex(j);
                                    SerializedProperty serializedParameterType = serializedParameter.FindPropertyRelative("parameterType");
                                    ParameterInfo parameter = parameters[j];
                                    string parameterName = parameter.Name;
                                    parameterName = parameterName.ToTitle();
                                    parametersName[j] = parameterName;
                                    if (parameter.ParameterType == typeof(float))
                                        serializedParameterType.enumValueIndex = 0;
                                    else if (parameter.ParameterType == typeof(int))
                                        serializedParameterType.enumValueIndex = 1;
                                    else if(parameter.ParameterType == typeof(bool))
                                        serializedParameterType.enumValueIndex = 2;
                                    else if(parameter.ParameterType == typeof(string))
                                        serializedParameterType.enumValueIndex = 3;
                                    else if(parameter.ParameterType == typeof(Vector3))
                                        serializedParameterType.enumValueIndex = 4;
                                    else if(parameter.ParameterType.IsSubclassOf(typeof(Object)))
                                    {
                                        serializedParameterType.enumValueIndex = 5;
                                        parametersType.Add(j, parameter.ParameterType);
                                    }
                                }
                                serializedFunction.serializedObject.ApplyModifiedProperties();
                            });
                        }
                    }
                }
            }
            return componentsMenu;
        }

        private string GetComponentDropdownLabel()
        {
            return string.IsNullOrEmpty(serializedType.stringValue) ? "Select component..." : serializedType.stringValue;
        }

        private string GetFunctionDropdownLabel()
        {
            return selectedType == null || string.IsNullOrEmpty(serializedFunction.stringValue) ? "Select function..." : serializedFunction.stringValue.ToTitle().LettersOnly();
        }
    }
}