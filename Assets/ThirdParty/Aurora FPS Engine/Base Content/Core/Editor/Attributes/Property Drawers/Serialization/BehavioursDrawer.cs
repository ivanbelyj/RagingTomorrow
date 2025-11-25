/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSEditor.AI;
using AuroraFPSRuntime.AIModules;
using AuroraFPSRuntime.AIModules.Behaviour;
using AuroraFPSRuntime.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [DrawerTarget(typeof(AIController.Behaviours))]
    internal sealed class BehavioursDrawer : PropertyDrawer
    {
        #region [Const Properties]
        private const float BackgroundHeight = 25.0f;
        private const float BackgroundBorderWidth = 1.0f;
        #endregion

        #region [Static Readonly Properties]
        private static readonly Padding FoldoutPadding = new Padding(0.0f, 0.0f, 0.0f, 19.0f, 19.0f, 4.0f);
        #endregion

        private SerializedProperty keys;
        private SerializedProperty values;

        private GUIContent plusIcon;
        private GUIContent minusIcon;
        private GUIContent settingsIcon;

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            keys = property.FindPropertyRelative("keys");
            values = property.FindPropertyRelative("values");

            plusIcon = EditorGUIUtility.IconContent("Toolbar Plus@2x");
            minusIcon = EditorGUIUtility.IconContent("Toolbar Minus@2x");
            settingsIcon = EditorGUIUtility.IconContent("_Popup");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect backgroundPosition = DrawBackgroundGUI(position);
            DrawPlusButtonGUI(backgroundPosition, OnAddElementCallback, property);

            Rect foldoutPosition = new Rect(position.x, position.y, position.width, singleLineHeight);
            foldoutPosition = FoldoutPadding.PaddingRect(foldoutPosition);
            property.isExpanded = EditorGUI.Foldout(foldoutPosition, property.isExpanded, label, true);
            if (property.isExpanded)
            {
                BeginExpandedBackgoundBorder(position, backgroundPosition);
                Rect elementPosition = new Rect(foldoutPosition.x, foldoutPosition.yMax + 5, foldoutPosition.width, singleLineHeight);
                if (keys.arraySize > 0)
                {
                    for (int i = 0; i < keys.arraySize; i++)
                    {
                        SerializedProperty key = keys.GetArrayElementAtIndex(i);
                        SerializedProperty value = values.GetArrayElementAtIndex(i);

                        EditorGUI.BeginDisabledGroup(key.stringValue == "Idle");
                        Rect keyPosition = new Rect(elementPosition.x, elementPosition.y + 1, elementPosition.width - 41, singleLineHeight);
                        string localKey = key.stringValue;
                        localKey = EditorGUI.DelayedTextField(keyPosition, localKey);
                        if (!ContainsKey(keys, localKey, i))
                        {
                            key.stringValue = localKey;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Aurora FPS: Collision", "Inventory already contains same group name!", "Ok");
                            GUIUtility.ExitGUI();
                        }
                        EditorGUI.EndDisabledGroup();

                        Rect settingsButtonPosition = new Rect(keyPosition.xMax + 2, keyPosition.y + 1, 20, 20);
                        if (GUI.Button(settingsButtonPosition, settingsIcon, "IconButton"))
                        {
                            BehaviourEditorWindow.Open(key.stringValue, property.serializedObject.targetObject, value.propertyPath);
                        }

                        if (key.stringValue != "Idle")
                        {
                            Rect minusButtonPosition = new Rect(settingsButtonPosition.xMax - 1, settingsButtonPosition.y, 20, 20);
                            if (GUI.Button(minusButtonPosition, minusIcon, "IconButton"))
                            {
                                BehaviourEditorWindow behaviourEditorWindow = EditorWindow.GetWindow<BehaviourEditorWindow>();
                                behaviourEditorWindow.Close();
                                OnRemoveElementCallback(i);
                                break;
                            }
                        }

                        CheckRecursion(key, value);

                        elementPosition.y += elementPosition.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    GUI.Label(elementPosition, "Add new elements...");
                }
                EndExpandedBackgroundBorder(position, backgroundPosition);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = BackgroundHeight;
            if (property.isExpanded)
            {
                height += 5;
                if (keys.arraySize > 0)
                {
                    for (int i = 0; i < keys.arraySize; i++)
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    height += EditorGUIUtility.singleLineHeight;
                }
            }
            return height;
        }

        private bool ContainsKey(SerializedProperty array, string key, int ignoreKey)
        {
            for (int i = 0; i < array.arraySize; i++)
            {
                if (i == ignoreKey)
                    continue;

                if (key == array.GetArrayElementAtIndex(i).stringValue)
                {
                    return true;
                }
            }
            return false;
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

        private void DrawPlusButtonGUI(Rect backgroundPosition, Action<SerializedProperty> onClickCallback, SerializedProperty property)
        {
            const float ButtonSize = 20;
            Rect buttonPosition = new Rect(backgroundPosition.xMax - ButtonSize, backgroundPosition.yMin + 5, ButtonSize, ButtonSize);
            if (GUI.Button(buttonPosition, plusIcon, "IconButton"))
            {
                onClickCallback?.Invoke(property);
            }
        }

        private void DrawMinusButtonGUI(Rect position, int index, Action<int> onClickCallback)
        {
            if (GUI.Button(position, minusIcon, "IconButton"))
            {
                onClickCallback?.Invoke(index);
            }
        }

        private void OnAddElementCallback(SerializedProperty property)
        {
            GenericMenu genericMenu = new GenericMenu();
            IEnumerable<Type> types = ApexReflection.FindSubclassesOf<AIBehaviour>();
            foreach (var type in types)
            {
                SerializedObject serializedObject = property.serializedObject;
                AIController controller = serializedObject.targetObject as AIController;
                AIBehaviourMenuAttribute attribute = ApexReflection.GetAttribute<AIBehaviourMenuAttribute>(type);
                if (attribute != null && !attribute.Hide && IsValidCore(controller, type))
                {
                    genericMenu.AddItem(new GUIContent(attribute.Path), false, () =>
                    {
                        if (AddRequiredComponents(controller, type))
                        {
                            int index = keys.arraySize;
                            keys.arraySize++;
                            values.arraySize++;

                            keys.GetArrayElementAtIndex(index).stringValue = string.Format("{0} {1}", attribute.Name, (index + 1));
                            values.GetArrayElementAtIndex(index).managedReferenceValue = Activator.CreateInstance(type);
                            serializedObject.ApplyModifiedProperties();
                        }
                    });
                }
            }
            genericMenu.ShowAsContext();
        }

        private void OnRemoveElementCallback(int index)
        {
            SerializedObject serializedObject = keys.serializedObject;
            keys.DeleteArrayElementAtIndex(index);
            values.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
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

        private bool IsValidCore(AIController controller, Type type)
        {
            IEnumerable<AICoreSupportAttribute> supportAttributes = type.GetCustomAttributes<AICoreSupportAttribute>();
            if (supportAttributes.Count() > 0)
            {
                foreach (AICoreSupportAttribute supportAttribute in supportAttributes)
                {
                    if (controller.GetType() == supportAttribute.target.GetType())
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private void CheckRecursion(SerializedProperty key, SerializedProperty value)
        {
            SerializedProperty serializedTransitions = value.FindPropertyRelative("transitions");
            for (int i = 0; i < serializedTransitions.arraySize; i++)
            {
                SerializedProperty serializedTransition = serializedTransitions.GetArrayElementAtIndex(i);
                SerializedProperty serializedTargetBehaviour = serializedTransition.FindPropertyRelative("targetBehaviour");
                if (key.stringValue == serializedTargetBehaviour.stringValue)
                {
                    string text = string.Format("AI Core engine detected recursion in {0} behaviour, in {1}th transition index.", key.stringValue, i + 1);
                    if (EditorUtility.DisplayDialog("AI Core: Recursion detected!", text, "Ok"))
                    {
                        serializedTargetBehaviour.stringValue = string.Empty;
                        BehaviourEditorWindow behaviourEditorWindow = EditorWindow.GetWindow<BehaviourEditorWindow>();
                        behaviourEditorWindow.Repaint();
                    }
                }
            }
        }

        private bool AddRequiredComponents(AIController controller, Type type)
        {
            bool allDependsAdded = true;
            IEnumerable<RequireComponent> requireComponents = type.GetCustomAttributes<RequireComponent>();
            foreach (RequireComponent requireComponent in requireComponents)
            {
                if (requireComponent.m_Type0 != null && controller.GetComponent(requireComponent.m_Type0) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        controller.gameObject.AddComponent(requireComponent.m_Type0);
                    }
                    else
                    {
                        string message = string.Format("This behaviour depending from {0} type. Add some realization of {0} type before adding this behaviour.", requireComponent.m_Type0.Name);
                        EditorUtility.DisplayDialog("Aurora FPS: AI Core", message, "Ok");
                        allDependsAdded = false;
                    }
                }

                if (requireComponent.m_Type1 != null && controller.GetComponent(requireComponent.m_Type1) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        controller.gameObject.AddComponent(requireComponent.m_Type1);
                    }
                    else
                    {
                        string message = string.Format("This behaviour depending from {0} type. Add some realization of {0} type before adding this behaviour.", requireComponent.m_Type0.Name);
                        EditorUtility.DisplayDialog("Aurora FPS: AI Core", message, "Ok");
                        allDependsAdded = false;
                    }
                }

                if (requireComponent.m_Type2 != null && controller.GetComponent(requireComponent.m_Type2) == null)
                {
                    if (!requireComponent.m_Type0.IsInterface && !requireComponent.m_Type0.IsAbstract)
                    {
                        controller.gameObject.AddComponent(requireComponent.m_Type2);
                    }
                    else
                    {
                        string message = string.Format("This behaviour depending from {0} type. Add some realization of {0} type before adding this behaviour.", requireComponent.m_Type0.Name);
                        EditorUtility.DisplayDialog("Aurora FPS: AI Core", message, "Ok");
                        allDependsAdded = false;
                    }
                }
            }
            return allDependsAdded;
        }
    }
}
