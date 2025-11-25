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
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(CustomViewAttribute))]
    public sealed class CustomViewView : PropertyView
    {
        private object target;
        private MethodInfo viewInitialization;
        private MethodInfo viewGUI;
        private MethodInfo viewHeight;

        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            CustomViewAttribute customViewAttribute = viewAttribute as CustomViewAttribute;

            target = ApexReflection.GetDeclaringObjectOfProperty(property);
            
            if (ApexReflection.TryDeepFindMethods(target.GetType(), customViewAttribute.ViewInitialization, out MethodInfo[] viewInitializationMethods))
            {
                for (int i = 0; i < viewInitializationMethods.Length; i++)
                {
                    MethodInfo method = viewInitializationMethods[i];
                    if (method.GetParameters().Length == 2)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty" &&
                            parameters[1].ParameterType.Name == "GUIContent")
                        {
                            viewInitialization = method;
                        }
                    }
                }
            }

            if (ApexReflection.TryDeepFindMethods(target.GetType(), customViewAttribute.ViewGUI, out MethodInfo[] viewGUIMethods))
            {
                for (int i = 0; i < viewGUIMethods.Length; i++)
                {
                    MethodInfo method = viewGUIMethods[i];
                    if (method.GetParameters().Length == 3)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "Rect" &&
                            parameters[1].ParameterType.Name == "SerializedProperty" &&
                            parameters[2].ParameterType.Name == "GUIContent")
                        {
                            viewGUI = method;
                        }
                    }
                }
            }

            if (ApexReflection.TryDeepFindMethods(target.GetType(), customViewAttribute.ViewHeight, out MethodInfo[] viewHeightMethods))
            {
                for (int i = 0; i < viewHeightMethods.Length; i++)
                {
                    MethodInfo method = viewHeightMethods[i];
                    if (method.GetParameters().Length == 2 && method.ReturnType.Name == "Single")
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters[0].ParameterType.Name == "SerializedProperty" &&
                            parameters[1].ParameterType.Name == "GUIContent")
                        {
                            viewHeight = method;
                        }
                    }
                }
            }

            viewInitialization?.Invoke(target, new object[2] { property, label });
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(viewGUI != null)
            {
                object[] parameters = new object[3] { position, property, label };
                viewGUI.Invoke(target, parameters);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(viewHeight != null)
            {
                object[] parameters = new object[2] { property, label };
                return (float)viewHeight.Invoke(target, parameters);
            }
            return base.GetPropertyHeight(property, label);
        }
    }
}