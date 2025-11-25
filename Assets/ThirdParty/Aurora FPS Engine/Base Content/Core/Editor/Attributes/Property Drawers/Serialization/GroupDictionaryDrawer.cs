/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory;
using System;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    [DrawerTarget(typeof(GroupDictionary))]
    public class GroupDictionaryDrawer : PropertyDrawer
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

        public override void OnInitialize(SerializedProperty property, GUIContent label)
        {
            keys = property.FindPropertyRelative("keys");
            values = property.FindPropertyRelative("values");

            plusIcon = EditorGUIUtility.IconContent("Toolbar Plus@2x");
            minusIcon = EditorGUIUtility.IconContent("Toolbar Minus@2x");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            Rect backgroundPosition = DrawBackgroundGUI(position);
            DrawPlusButtonGUI(backgroundPosition, OnAddElementCallback);

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

                        Rect labelKeyPosition = new Rect(elementPosition.x, elementPosition.y + 1, 40, singleLineHeight);
                        key.isExpanded = EditorGUI.Foldout(labelKeyPosition, key.isExpanded, "Name");

                        Rect keyPosition = new Rect(labelKeyPosition.xMax, elementPosition.y + 1, elementPosition.width - labelKeyPosition.width - 21, singleLineHeight);
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

                        Rect minusButtonPosition = new Rect(keyPosition.xMax + 2, keyPosition.y + 1, 20, 20);
                        if (GUI.Button(minusButtonPosition, minusIcon, "IconButton"))
                        {
                            OnRemoveElementCallback(i);
                            break;
                        }

                        float childrenHeight = 0;
                        if (key.isExpanded)
                        {
                            SerializedProperty value = values.GetArrayElementAtIndex(i);
                            SerializedProperty valueKeys = value.FindPropertyRelative("slots.keys");
                            SerializedProperty valueValues = value.FindPropertyRelative("slots.values");

                            Rect valuePosition = new Rect(labelKeyPosition.xMax + 1, elementPosition.yMax + 3, elementPosition.width - labelKeyPosition.width - minusButtonPosition.width, singleLineHeight);

                            Rect buttonPosition = new Rect(valuePosition.xMax, valuePosition.y, 20, 20);
                            if (GUI.Button(buttonPosition, plusIcon, "IconButton"))
                            {
                                int index = valueKeys.arraySize;
                                valueKeys.arraySize++;
                                valueValues.arraySize++;
                                valueKeys.GetArrayElementAtIndex(index).stringValue = string.Format("Input {0}", (index + 1));
                            }

                            for (int j = 0; j < valueKeys.arraySize; j++)
                            {
                                SerializedProperty valueKey = valueKeys.GetArrayElementAtIndex(j);
                                SerializedProperty valueValue = valueValues.GetArrayElementAtIndex(j);

                                Rect valueLabelPosition = new Rect(valuePosition.x, valuePosition.y, 30, singleLineHeight);
                                GUI.Label(valueLabelPosition, "Item");


                                Rect valueKeyPosition = new Rect(valueLabelPosition.xMax , valuePosition.y, valuePosition.width - 50, singleLineHeight);
                                Rect[] rects = ApexEditorUtilities.SplitRect2(valueKeyPosition, 2);

                                string localValueKey = valueKey.stringValue;
                                localValueKey = EditorGUI.DelayedTextField(rects[0], localValueKey);
                                if (!ContainsKey(valueKeys, localValueKey, j))
                                {
                                    valueKey.stringValue = localValueKey;
                                }
                                else
                                {
                                    EditorUtility.DisplayDialog("Aurora FPS: Collision", "Inventory group already contains same input key!", "Ok");
                                    GUIUtility.ExitGUI();
                                }

                                Rect valueValuePosition = rects[1];
                                EditorGUI.PropertyField(valueValuePosition, valueValue, GUIContent.none);

                                Rect minusValueButtonPosition = new Rect(valueValuePosition.xMax + 2, valueValuePosition.y + 1, 20, 20);
                                if (GUI.Button(minusValueButtonPosition, minusIcon, "IconButton"))
                                {
                                    valueKeys.DeleteArrayElementAtIndex(j);
                                    valueValues.DeleteArrayElementAtIndex(j);
                                    break;
                                }

                                childrenHeight += singleLineHeight + standardVerticalSpacing;
                                valuePosition.y += singleLineHeight + standardVerticalSpacing;
                            }

                            if(valueKeys.arraySize == 0)
                            {
                                childrenHeight += singleLineHeight + standardVerticalSpacing;
                            }
                        }
                        elementPosition.y += childrenHeight;
                        elementPosition.y += elementPosition.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    GUI.Label(elementPosition, "Empty...");
                }
                EndExpandedBackgroundBorder(position, backgroundPosition);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = BackgroundHeight + 5;
            if (property.isExpanded)
            {
                if(keys.arraySize > 0)
                {
                    for (int i = 0; i < keys.arraySize; i++)
                    {
                        height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                        SerializedProperty key = keys.GetArrayElementAtIndex(i);
                        if (key.isExpanded)
                        {

                            SerializedProperty value = values.GetArrayElementAtIndex(i);
                            SerializedProperty valueKeys = value.FindPropertyRelative("slots.keys");
                            if (valueKeys.arraySize > 0)
                            {
                                for (int j = 0; j < valueKeys.arraySize; j++)
                                {
                                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                                }
                            }
                            else
                            {
                                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                            }
                        }
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

        private void DrawPlusButtonGUI(Rect backgroundPosition, Action onClickCallback)
        {
            const float ButtonSize = 20;
            Rect buttonPosition = new Rect(backgroundPosition.xMax - ButtonSize, backgroundPosition.yMin + 5, ButtonSize, ButtonSize);
            if (GUI.Button(buttonPosition, plusIcon, "IconButton"))
            {
                onClickCallback?.Invoke();
            }
        }

        private void DrawMinusButtonGUI(Rect position, int index, Action<int> onClickCallback)
        {
            if (GUI.Button(position, minusIcon, "IconButton"))
            {
                onClickCallback?.Invoke(index);
            }
        }

        private void OnAddElementCallback()
        {
            SerializedObject serializedObject = keys.serializedObject;
            int index = keys.arraySize;
            keys.arraySize++;
            values.arraySize++;
            keys.GetArrayElementAtIndex(index).stringValue = string.Format("Group {0}", (index + 1));
            values.GetArrayElementAtIndex(index).FindPropertyRelative("slots.keys").arraySize = 0;
            values.GetArrayElementAtIndex(index).FindPropertyRelative("slots.values").arraySize = 0;
            serializedObject.ApplyModifiedProperties();
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
    }
}