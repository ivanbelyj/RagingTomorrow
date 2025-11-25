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
using Vexe.Runtime.Extensions;

namespace AuroraFPSEditor.Attributes
{
    [PainterTarget(typeof(InlineButtonAttribute))]
    public sealed class InlineButtonPainter : PropertyPainter, IPropertyPositionModifyReceiver
    {
        private MethodCaller<object, object> action;
        private GUIContent content;
        private GUIStyle buttonStyle;
        private InlineButtonAttribute inlineButtonAttribute;
        private bool includeProperty;

        public override void OnInitialize(SerializedProperty property, PainterAttribute attribute, GUIContent label)
        {
            inlineButtonAttribute = attribute as InlineButtonAttribute;
            if (ApexReflection.TryDeepFindMethods(property.serializedObject.targetObject.GetType(), inlineButtonAttribute.name, out MethodInfo[] methods))
            {
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo method = methods[i];
                    if (method.GetParameters().Length == 0 || 
                        (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(SerializedProperty)))
                    {
                        includeProperty = method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(SerializedProperty);
                        action = method.DelegateForCall();
                    }
                }
            }

            string methodLabel = inlineButtonAttribute.Label;
            if (!string.IsNullOrEmpty(methodLabel) && methodLabel.Length > 1 && methodLabel[0] == '@')
            {
                string iconName = methodLabel.Remove(0, 1);
                content = EditorGUIUtility.IconContent(iconName);
            }
            else
            {
                content = new GUIContent(inlineButtonAttribute.Label);
            }
        }

        public void ModifyPropertyPosition(ref Rect position)
        {
            if(buttonStyle == null)
            {
                buttonStyle = new GUIStyle(inlineButtonAttribute.Style);
                if(inlineButtonAttribute.Width == -1)
                {
                    inlineButtonAttribute.Width = buttonStyle.CalcSize(content).x;
                }
            }
            position.width -= inlineButtonAttribute.Width + 1;
        }

        public override void OnPainterGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect propertyPosition = GetPropertyPosition();
            Rect buttonRect = new Rect(position.x + position.width + 1, propertyPosition.y + 1, inlineButtonAttribute.Width, 16);
            if (GUI.Button(buttonRect, content, buttonStyle) && action != null)
            {
                action(property.serializedObject.targetObject, includeProperty ? new object[1] { property } : null);
            }
        }
    }
}