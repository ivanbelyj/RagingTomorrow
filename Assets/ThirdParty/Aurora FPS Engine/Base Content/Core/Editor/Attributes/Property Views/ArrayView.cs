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
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [ViewTarget(typeof(ArrayAttribute))]
    public class ArrayView : PropertyView
    {
        #region [Const Properties]
        private const float BackgroundHeight = 25.0f;
        private const float BackgroundBorderWidth = 1.0f;
        private const float CustomVerticalSpacing = 5.0f;

        private const string ElementNameArg_Index = "{index}";
        private const string ElementNameArg_NiceIndex = "{niceIndex}";
        #endregion

        #region [Static Readonly Properties]
        private static readonly Padding FoldoutPadding = new Padding(0.0f, 0.0f, 0.0f, 19.0f, 19.0f, 4.0f);
        #endregion

        // Stored ArrayAttribute instance.
        private ArrayAttribute arrayAttribute;
        private ApexProperty[] children;

        // Stored label GUIContents.
        private GUIContent plusIcon;
        private GUIContent minusIcon;

        // Stored custom callbacks.
        private object target;
        private MethodInfo getElementLabelCallback;


        public override void OnInitialize(SerializedProperty property, ViewAttribute viewAttribute, GUIContent label)
        {
            arrayAttribute = viewAttribute as ArrayAttribute;

            children = new ApexProperty[property.arraySize];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = new ApexProperty(property.GetArrayElementAtIndex(i));
            }

            plusIcon = EditorGUIUtility.IconContent("Toolbar Plus@2x");
            minusIcon = EditorGUIUtility.IconContent("Toolbar Minus@2x");

            target = ApexReflection.GetDeclaringObjectOfProperty(property);
            Type type = target.GetType();
            getElementLabelCallback = GetElementLabelCallback(type, arrayAttribute.GetElementLabelCallback);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect backgroundPosition = DrawBackgroundGUI(position);

            Rect foldoutPosition = new Rect(backgroundPosition.x + 5, backgroundPosition.y, backgroundPosition.width - 25, backgroundPosition.height);
            property.isExpanded = ApexEditorUtilities.IndentFoldoutGUI(foldoutPosition, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                BeginExpandedBackgoundBorder(position, backgroundPosition);
                Rect elementPosition = new Rect(foldoutPosition.x, foldoutPosition.yMax, foldoutPosition.width, singleLineHeight);
                elementPosition.x += 14;
                elementPosition.width -= 16;
                for (int i = 0; i < children.Length; i++)
                {
                    Rect separatorPosition = new Rect(position.xMin, elementPosition.yMin, position.width, BackgroundBorderWidth);
                    EditorGUI.DrawRect(separatorPosition, ApexSettings.PropertyBorderColor);

                    Rect minusButtonPosition = new Rect(foldoutPosition.xMax, elementPosition.yMin + 4, 20, 20);
                    if (GUI.Button(minusButtonPosition, minusIcon, "IconButton"))
                    {
                        property.DeleteArrayElementAtIndex(i);
                        property.serializedObject.ApplyModifiedProperties();
                        children = new ApexProperty[property.arraySize];
                        for (int j = 0; j < children.Length; j++)
                        {
                            children[j] = new ApexProperty(property.GetArrayElementAtIndex(j));
                        }
                        return;
                    }

                    ApexProperty child = children[i];
                    string elementLabel = getElementLabelCallback?.Invoke(target, new object[2] { property, i }).ToString() ?? GetElementName(i);

                    Rect childPosition = new Rect(elementPosition.x, elementPosition.y + 2.5f, elementPosition.width, child.GetFieldHeight());
                    child.DrawField(childPosition, new GUIContent(elementLabel));
                    elementPosition.y += childPosition.height + CustomVerticalSpacing;
                }

                if (children.Length == 0)
                {
                    elementPosition.y += 2.5f;
                    GUI.Label(elementPosition, "Add new elements...");
                }
                EndExpandedBackgroundBorder(position, backgroundPosition);
            }

            Rect plusButtonPosition = new Rect(foldoutPosition.xMax, foldoutPosition.yMin + 5, 20, 20);
            if(GUI.Button(plusButtonPosition, plusIcon, "IconButton"))
            {
                property.arraySize++;
                property.serializedObject.ApplyModifiedProperties();
                children = new ApexProperty[property.arraySize];
                for (int i = 0; i < children.Length; i++)
                {
                    children[i] = new ApexProperty(property.GetArrayElementAtIndex(i));
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = BackgroundHeight;
            if (children != null && property.isExpanded)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    height += children[i].GetFieldHeight() + CustomVerticalSpacing;
                }

                if(children.Length == 0)
                {
                    height += 23;
                }
            }
            return height;
        }

        private Rect DrawBackgroundGUI(Rect position)
        {
            Rect foldoutBackground = new Rect(position.x, position.y, position.width, BackgroundHeight);
            EditorGUI.DrawRect(foldoutBackground, ApexSettings.PropertyColor);

            Rect foldoutTopBorderPosition = new Rect(foldoutBackground.xMin, foldoutBackground.yMin, foldoutBackground.width, BackgroundBorderWidth);
            EditorGUI.DrawRect(foldoutTopBorderPosition, ApexSettings.PropertyBorderColor);

            Rect foldoutBottomBorderPosition = new Rect(foldoutBackground.xMin, foldoutBackground.yMax, foldoutBackground.width, BackgroundBorderWidth);
            EditorGUI.DrawRect(foldoutBottomBorderPosition, ApexSettings.PropertyBorderColor);

            Rect foldoutLeftBorderPosition = new Rect(foldoutBackground.xMin, foldoutBackground.yMin, BackgroundBorderWidth, BackgroundHeight);
            EditorGUI.DrawRect(foldoutLeftBorderPosition, ApexSettings.PropertyBorderColor);

            Rect foldoutRightBorderPosition = new Rect(foldoutBackground.xMax, foldoutBackground.yMin, BackgroundBorderWidth, BackgroundHeight);
            EditorGUI.DrawRect(foldoutRightBorderPosition, ApexSettings.PropertyBorderColor);

            return foldoutBackground;
        }

        private void DrawSeparator(Rect position)
        {
            EditorGUI.DrawRect(position, ApexSettings.PropertyBorderColor);
        }

        private void BeginExpandedBackgoundBorder(Rect position, Rect backgroundPosition)
        {
            Rect expandLeftBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMax, BackgroundBorderWidth, position.height - BackgroundHeight);
            EditorGUI.DrawRect(expandLeftBorderPosition, ApexSettings.PropertyBorderColor);

            Rect expandRightBorderPosition = new Rect(backgroundPosition.xMax, backgroundPosition.yMax, BackgroundBorderWidth, position.height - BackgroundHeight);
            EditorGUI.DrawRect(expandRightBorderPosition, ApexSettings.PropertyBorderColor);
        }

        private void EndExpandedBackgroundBorder(Rect position, Rect backgroundPosition)
        {
            Rect expandBottomBorderPosition = new Rect(backgroundPosition.xMin, position.yMax, backgroundPosition.width, BackgroundBorderWidth);
            EditorGUI.DrawRect(expandBottomBorderPosition, ApexSettings.PropertyBorderColor);
        }

        public string GetElementName(int index)
        {
            if (arrayAttribute.ElementLabel.Contains(ElementNameArg_Index))
                return arrayAttribute.ElementLabel.Replace(ElementNameArg_Index, index.ToString());
            else if (arrayAttribute.ElementLabel.Contains(ElementNameArg_NiceIndex))
                return arrayAttribute.ElementLabel.Replace(ElementNameArg_NiceIndex, (index + 1).ToString());
            return string.Format("Element {0}", index);
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
    }
}