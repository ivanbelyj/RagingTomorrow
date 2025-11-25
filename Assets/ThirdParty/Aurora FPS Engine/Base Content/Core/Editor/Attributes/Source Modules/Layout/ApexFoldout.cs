/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Vexe.Runtime.Extensions;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexFoldout : ApexLayout
    {
        // Apex foldout properties.
        private string title;
        private string style;
        private bool isExpanded;
        private bool headerAction;
        private MethodCaller<object, object> action;

        // Stored Apex settings.
        private ApexSettings apexSettings;

        // Stored required properties.
        private bool isInsideGroup;

        public ApexFoldout(SerializedProperty serializedProperty, string title, string style, List<ApexSerializedField> children) : base(serializedProperty, children)
        {
            this.title = title;
            this.style = style;
            apexSettings = ApexSettings.Current;

            if (style != null && style.Contains("HeaderAction:"))
            {
                string actionName = style.Remove(0, 16);
                System.Type type = ApexReflection.GetPropertyType(TargetSerializedProperty);
                if (ApexReflection.TryDeepFindMethods(type, actionName, out MethodInfo[] methods))
                {
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(Rect))
                        {
                            action = method.DelegateForCall();
                            headerAction = true;
                        }
                    }
                }
            }
            
        }

        public ApexFoldout(SerializedProperty serializedProperty, string title, string style, params ApexSerializedField[] children) : base(serializedProperty, children)
        {
            this.title = title;
            this.style = style;
            apexSettings = ApexSettings.Current;

            if (style != null && style.Contains("HeaderAction:"))
            {
                string actionName = style.Remove(0, 13);
                Debug.Log(actionName);
                if (ApexReflection.TryDeepFindMethods(TargetSerializedProperty.serializedObject.targetObject.GetType(), actionName, out MethodInfo[] methods))
                {
                    for (int i = 0; i < methods.Length; i++)
                    {
                        MethodInfo method = methods[i];
                        Debug.Log(method.Name);
                        if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(Rect))
                        {
                            action = method.DelegateForCall();
                            headerAction = true;
                        }
                    }
                }
            }
        }

        public override void DrawField(Rect position)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            if (string.IsNullOrEmpty(style))
            {
                isExpanded = EditorGUI.Foldout(position, isExpanded, title, true);
                if (isExpanded)
                {
                    EditorGUI.indentLevel++;
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    DrawChildren(position);
                    EditorGUI.indentLevel--;
                }
            }
            else if(style == "Indent")
            {
                isExpanded = ApexEditorUtilities.IndentFoldoutGUI(position, isExpanded, title, true);
                if (isExpanded)
                {
                    position.x += 16;
                    position.width -= 16;
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    DrawChildren(position);
                }
            }
            else if (style == "Header")
            {
                isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, isExpanded, title);
                if (isExpanded)
                {
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    DrawChildren(position);
                }
                EditorGUI.EndFoldoutHeaderGroup();
            }
            else if (headerAction)
            {
                isExpanded = EditorGUI.BeginFoldoutHeaderGroup(position, isExpanded, title, menuAction: Action);
                if (isExpanded)
                {
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    DrawChildren(position);
                }
                EditorGUI.EndFoldoutHeaderGroup();
            }
        }

        public void Action(Rect rect)
        {
            action.Invoke(TargetSerializedProperty.serializedObject.targetObject, new object[1] { rect });
        }

        private void DrawChildren(Rect position)
        {
            for (int i = 0; i < children.Count; i++)
            {
                ApexSerializedField child = children[i];
                if (child.IsVisible())
                {
                    position.height = child.GetFieldHeight();
                    child.DrawField(position);
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        public override float GetFieldHeight()
        {
            float height = style == "Header" ? 20 : EditorGUIUtility.singleLineHeight;
            if (isExpanded)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    ApexSerializedField child = children[i];
                    if (child.IsVisible())
                    {
                        height += child.GetFieldHeight() + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
            }
            return height;
        }

        #region [Getter / Setter]
        public string GetName()
        {
            return title;
        }

        public void SetName(string value)
        {
            title = value;
        }

        public string GetStyle()
        {
            return style;
        }

        public void SetStyle(string value)
        {
            style = value;
        }

        public bool IsExpanded()
        {
            return isExpanded;
        }

        public void IsExpanded(bool value)
        {
            isExpanded = value;
        }

        public bool IsInsideGroup()
        {
            return isInsideGroup;
        }

        public void IsInsideGroup(bool value)
        {
            isInsideGroup = value;
        }
        #endregion
    }
}