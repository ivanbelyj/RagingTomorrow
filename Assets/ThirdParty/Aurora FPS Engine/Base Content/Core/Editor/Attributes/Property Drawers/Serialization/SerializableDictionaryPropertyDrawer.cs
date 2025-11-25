/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    /// <summary>
    /// Custom property drawers SerializableDictionaryBase / SerializableHashSetBase class.
    /// </summary>
    [DrawerTarget(typeof(SerializableDictionaryBase), SubClasses = true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private class ConflictState
        {
            public object conflictKey = null;
            public object conflictValue = null;
            public int conflictIndex = -1;
            public int conflictOtherIndex = -1;
            public bool conflictKeyPropertyExpanded = false;
            public bool conflictValuePropertyExpanded = false;
            public float conflictLineHeight = 0f;
        }

        private const string KeysFieldName = "keys";
        private const string ValuesFieldName = "values";
        private const float BackgroundHeight = 25.0f;
        private const float BackgroundBorderWidth = 1.0f;
        protected const float IndentWidth = 5;

        private GUIStyle s_buttonStyle;

        private static readonly Padding FoldoutPadding = new Padding(0.0f, 0.0f, 0.0f, 19.0f, 19.0f, 4.0f);

        private readonly static GUIContent s_iconPlus = IconContent("Toolbar Plus", "Add entry");
        private readonly static GUIContent s_iconMinus = IconContent("Toolbar Minus", "Remove entry");
        private readonly static GUIContent s_warningIconConflict = IconContent("console.warnicon.sml", "Conflicting key, this entry will be lost");
        private readonly static GUIContent s_warningIconOther = IconContent("console.infoicon.sml", "Conflicting key");
        private readonly static GUIContent s_warningIconNull = IconContent("console.warnicon.sml", "Null key, this entry will be lost");
        private readonly static GUIContent s_tempContent = new GUIContent();

        private struct PropertyIdentity
        {
            public PropertyIdentity(SerializedProperty property)
            {
                this.instance = property.serializedObject.targetObject;
                this.propertyPath = property.propertyPath;
            }

            public UnityEngine.Object instance;
            public string propertyPath;
        }

        private static Dictionary<PropertyIdentity, ConflictState> s_conflictStateDict = new Dictionary<PropertyIdentity, ConflictState>();

        private enum Action
        {
            None,
            Add,
            Remove
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (s_buttonStyle == null)
            {
                s_buttonStyle = new GUIStyle("IconButton");
            }

            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            label = EditorGUI.BeginProperty(position, label, property);

            Action buttonAction = Action.None;
            int buttonActionIndex = 0;

            var keyArrayProperty = property.FindPropertyRelative(KeysFieldName);
            var valueArrayProperty = property.FindPropertyRelative(ValuesFieldName);

            ConflictState conflictState = GetConflictState(property);

            if (conflictState.conflictIndex != -1)
            {
                keyArrayProperty.InsertArrayElementAtIndex(conflictState.conflictIndex);
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(conflictState.conflictIndex);
                SetPropertyValue(keyProperty, conflictState.conflictKey);
                keyProperty.isExpanded = conflictState.conflictKeyPropertyExpanded;

                if (valueArrayProperty != null)
                {
                    valueArrayProperty.InsertArrayElementAtIndex(conflictState.conflictIndex);
                    var valueProperty = valueArrayProperty.GetArrayElementAtIndex(conflictState.conflictIndex);
                    SetPropertyValue(valueProperty, conflictState.conflictValue);
                    valueProperty.isExpanded = conflictState.conflictValuePropertyExpanded;
                }
            }

            const float buttonWidth = 20;

            var backgroundPosition = DrawBackgroundGUI(position);

            var labelPosition = FoldoutPadding.PaddingRect(position);

            labelPosition.height = singleLineHeight;
            if (property.isExpanded)
                labelPosition.xMax -= 20;

            EditorGUI.PropertyField(labelPosition, property, label, false);
            // property.isExpanded = EditorGUI.Foldout(labelPosition, property.isExpanded, label);
            if (property.isExpanded)
            {
                var buttonPosition = new Rect(position.xMax - buttonWidth, position.y + 5.0f, buttonWidth, buttonWidth);
                EditorGUI.BeginDisabledGroup(conflictState.conflictIndex != -1);
                if (GUI.Button(buttonPosition, s_iconPlus, s_buttonStyle))
                {
                    buttonAction = Action.Add;
                    buttonActionIndex = keyArrayProperty.arraySize;
                }
                EditorGUI.EndDisabledGroup();

                var linePosition = new Rect(labelPosition.x, labelPosition.yMax + 6, labelPosition.width, labelPosition.height);

                foreach (var entry in EnumerateEntries(keyArrayProperty, valueArrayProperty))
                {
                    BeginExpandedBackgoundBorder(position, backgroundPosition);
                    var keyProperty = entry.keyProperty;
                    var valueProperty = entry.valueProperty;
                    int i = entry.index;

                    float lineHeight = DrawKeyValueLine(keyProperty, valueProperty, linePosition, i);

                    buttonPosition.y = linePosition.y;
                    if (GUI.Button(buttonPosition, s_iconMinus, s_buttonStyle))
                    {
                        buttonAction = Action.Remove;
                        buttonActionIndex = i;
                    }

                    if (i == conflictState.conflictIndex && conflictState.conflictOtherIndex == -1)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = s_buttonStyle.CalcSize(s_warningIconNull);
                        iconPosition.x -= iconPosition.size.x + 1;
                        GUI.Label(iconPosition, s_warningIconNull);
                    }
                    else if (i == conflictState.conflictIndex)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = s_buttonStyle.CalcSize(s_warningIconConflict);
                        iconPosition.x -= iconPosition.size.x + 1;
                        GUI.Label(iconPosition, s_warningIconConflict);
                    }
                    else if (i == conflictState.conflictOtherIndex)
                    {
                        var iconPosition = linePosition;
                        iconPosition.size = s_buttonStyle.CalcSize(s_warningIconOther);
                        iconPosition.x -= iconPosition.size.x + 1;
                        GUI.Label(iconPosition, s_warningIconOther);
                    }


                    linePosition.y += lineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EndExpandedBackgroundBorder(position, backgroundPosition);
                }

            }

            if (buttonAction == Action.Add)
            {
                keyArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
                if (valueArrayProperty != null)
                    valueArrayProperty.InsertArrayElementAtIndex(buttonActionIndex);
            }
            else if (buttonAction == Action.Remove)
            {
                DeleteArrayElementAtIndex(keyArrayProperty, buttonActionIndex);
                if (valueArrayProperty != null)
                    DeleteArrayElementAtIndex(valueArrayProperty, buttonActionIndex);
            }

            conflictState.conflictKey = null;
            conflictState.conflictValue = null;
            conflictState.conflictIndex = -1;
            conflictState.conflictOtherIndex = -1;
            conflictState.conflictLineHeight = 0f;
            conflictState.conflictKeyPropertyExpanded = false;
            conflictState.conflictValuePropertyExpanded = false;

            foreach (var entry1 in EnumerateEntries(keyArrayProperty, valueArrayProperty))
            {
                var keyProperty1 = entry1.keyProperty;
                int i = entry1.index;
                object keyProperty1Value = GetPropertyValue(keyProperty1);

                if (keyProperty1Value == null)
                {
                    var valueProperty1 = entry1.valueProperty;
                    SaveProperty(keyProperty1, valueProperty1, i, -1, conflictState);
                    DeleteArrayElementAtIndex(keyArrayProperty, i);
                    if (valueArrayProperty != null)
                        DeleteArrayElementAtIndex(valueArrayProperty, i);

                    break;
                }


                foreach (var entry2 in EnumerateEntries(keyArrayProperty, valueArrayProperty, i + 1))
                {
                    var keyProperty2 = entry2.keyProperty;
                    int j = entry2.index;
                    object keyProperty2Value = GetPropertyValue(keyProperty2);

                    if (ComparePropertyValues(keyProperty1Value, keyProperty2Value))
                    {
                        var valueProperty2 = entry2.valueProperty;
                        SaveProperty(keyProperty2, valueProperty2, j, i, conflictState);
                        DeleteArrayElementAtIndex(keyArrayProperty, j);
                        if (valueArrayProperty != null)
                            DeleteArrayElementAtIndex(valueArrayProperty, j);

                        goto breakLoops;
                    }
                }
            }
        breakLoops:

            EditorGUI.EndProperty();
        }

        private static float DrawKeyValueLine(SerializedProperty keyProperty, SerializedProperty valueProperty, Rect linePosition, int index)
        {
            bool keyCanBeExpanded = CanPropertyBeExpanded(keyProperty);

            if (valueProperty != null)
            {
                bool valueCanBeExpanded = CanPropertyBeExpanded(valueProperty);

                if (!keyCanBeExpanded && valueCanBeExpanded)
                {
                    return DrawKeyValueLineExpand(keyProperty, valueProperty, linePosition);
                }
                else
                {

                    var keyLabel = keyCanBeExpanded ? ("Key " + index.ToString()) : "";
                    var valueLabel = valueCanBeExpanded ? ("Value " + index.ToString()) : "";
                    return DrawKeyValueLineSimple(keyProperty, valueProperty, keyLabel, valueLabel, linePosition);
                }
            }
            else
            {
                if (!keyCanBeExpanded)
                {
                    return DrawKeyLine(keyProperty, linePosition, null);
                }
                else
                {
                    var keyLabel = string.Format("{0} {1}", ObjectNames.NicifyVariableName(keyProperty.type), index);
                    return DrawKeyLine(keyProperty, linePosition, keyLabel);
                }
            }
        }

        private static float DrawKeyValueLineSimple(SerializedProperty keyProperty, SerializedProperty valueProperty, string keyLabel, string valueLabel, Rect linePosition)
        {
            float space = 17;
            float labelWidth = linePosition.width / 2 - (space / 2);
            float labelWidthRelative = labelWidth / linePosition.width;

            float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            var keyPosition = linePosition;
            keyPosition.width = labelWidth;
            keyPosition.height = keyPropertyHeight;
            EditorGUIUtility.labelWidth = keyPosition.width * labelWidthRelative;
            EditorGUI.PropertyField(keyPosition, keyProperty, TempContent(keyLabel), true);

            float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
            var valuePosition = linePosition;
            valuePosition.height = valuePropertyHeight;
            valuePosition.xMin += labelWidth + space;
            EditorGUIUtility.labelWidth = valuePosition.width * labelWidthRelative;
            EditorGUI.indentLevel--;
            EditorGUI.PropertyField(valuePosition, valueProperty, TempContent(valueLabel), true);
            EditorGUI.indentLevel++;

            EditorGUIUtility.labelWidth = labelWidth;

            return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        }

        private static float DrawKeyValueLineExpand(SerializedProperty keyProperty, SerializedProperty valueProperty, Rect linePosition)
        {
            float labelWidth = EditorGUIUtility.labelWidth;

            float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            var keyPosition = linePosition;
            keyPosition.height = keyPropertyHeight;
            EditorGUI.PropertyField(keyPosition, keyProperty, GUIContent.none, true);

            float valuePropertyHeight = EditorGUI.GetPropertyHeight(valueProperty);
            var valuePosition = linePosition;
            valuePosition.height = valuePropertyHeight;
            EditorGUI.PropertyField(valuePosition, valueProperty, GUIContent.none, true);

            EditorGUIUtility.labelWidth = labelWidth;

            return Mathf.Max(keyPropertyHeight, valuePropertyHeight);
        }

        private static float DrawKeyLine(SerializedProperty keyProperty, Rect linePosition, string keyLabel)
        {
            float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            var keyPosition = linePosition;
            keyPosition.height = keyPropertyHeight;
            keyPosition.width = linePosition.width;

            var keyLabelContent = keyLabel != null ? TempContent(keyLabel) : GUIContent.none;
            EditorGUI.PropertyField(keyPosition, keyProperty, keyLabelContent, true);

            return keyPropertyHeight;
        }

        private static bool CanPropertyBeExpanded(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Quaternion:
                    return true;
                default:
                    return false;
            }
        }

        private static void SaveProperty(SerializedProperty keyProperty, SerializedProperty valueProperty, int index, int otherIndex, ConflictState conflictState)
        {
            conflictState.conflictKey = GetPropertyValue(keyProperty);
            conflictState.conflictValue = valueProperty != null ? GetPropertyValue(valueProperty) : null;
            float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
            float valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
            float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
            conflictState.conflictLineHeight = lineHeight;
            conflictState.conflictIndex = index;
            conflictState.conflictOtherIndex = otherIndex;
            conflictState.conflictKeyPropertyExpanded = keyProperty.isExpanded;
            conflictState.conflictValuePropertyExpanded = valueProperty != null ? valueProperty.isExpanded : false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float propertyHeight = BackgroundHeight;

            if (property.isExpanded)
            {
                var keysProperty = property.FindPropertyRelative(KeysFieldName);
                var valuesProperty = property.FindPropertyRelative(ValuesFieldName);

                foreach (var entry in EnumerateEntries(keysProperty, valuesProperty))
                {
                    var keyProperty = entry.keyProperty;
                    var valueProperty = entry.valueProperty;
                    float keyPropertyHeight = EditorGUI.GetPropertyHeight(keyProperty);
                    float valuePropertyHeight = valueProperty != null ? EditorGUI.GetPropertyHeight(valueProperty) : 0f;
                    float lineHeight = Mathf.Max(keyPropertyHeight, valuePropertyHeight);
                    propertyHeight += lineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                ConflictState conflictState = GetConflictState(property);

                if (conflictState.conflictIndex != -1)
                {
                    propertyHeight += conflictState.conflictLineHeight;
                }
            }

            return propertyHeight + 4;
        }

        private static ConflictState GetConflictState(SerializedProperty property)
        {
            ConflictState conflictState;
            PropertyIdentity propId = new PropertyIdentity(property);
            if (!s_conflictStateDict.TryGetValue(propId, out conflictState))
            {
                conflictState = new ConflictState();
                s_conflictStateDict.Add(propId, conflictState);
            }
            return conflictState;
        }

        private static Dictionary<SerializedPropertyType, PropertyInfo> s_serializedPropertyValueAccessorsDict;

        static SerializableDictionaryPropertyDrawer()
        {
            Dictionary<SerializedPropertyType, string> serializedPropertyValueAccessorsNameDict = new Dictionary<SerializedPropertyType, string>() {
            { SerializedPropertyType.Integer, "intValue" },
            { SerializedPropertyType.Boolean, "boolValue" },
            { SerializedPropertyType.Float, "floatValue" },
            { SerializedPropertyType.String, "stringValue" },
            { SerializedPropertyType.Color, "colorValue" },
            { SerializedPropertyType.ObjectReference, "objectReferenceValue" },
            { SerializedPropertyType.LayerMask, "intValue" },
            { SerializedPropertyType.Enum, "intValue" },
            { SerializedPropertyType.Vector2, "vector2Value" },
            { SerializedPropertyType.Vector3, "vector3Value" },
            { SerializedPropertyType.Vector4, "vector4Value" },
            { SerializedPropertyType.Rect, "rectValue" },
            { SerializedPropertyType.ArraySize, "intValue" },
            { SerializedPropertyType.Character, "intValue" },
            { SerializedPropertyType.AnimationCurve, "animationCurveValue" },
            { SerializedPropertyType.Bounds, "boundsValue" },
            { SerializedPropertyType.Quaternion, "quaternionValue" },
        };
            Type serializedPropertyType = typeof(SerializedProperty);

            s_serializedPropertyValueAccessorsDict = new Dictionary<SerializedPropertyType, PropertyInfo>();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            foreach (var kvp in serializedPropertyValueAccessorsNameDict)
            {
                PropertyInfo propertyInfo = serializedPropertyType.GetProperty(kvp.Value, flags);
                s_serializedPropertyValueAccessorsDict.Add(kvp.Key, propertyInfo);
            }
        }

        private static GUIContent IconContent(string name, string tooltip)
        {
            var builtinIcon = EditorGUIUtility.IconContent(name);
            return new GUIContent(builtinIcon.image, tooltip);
        }

        private static GUIContent TempContent(string text)
        {
            s_tempContent.text = text;
            return s_tempContent;
        }

        private static void DeleteArrayElementAtIndex(SerializedProperty arrayProperty, int index)
        {
            var property = arrayProperty.GetArrayElementAtIndex(index);
            // if(arrayProperty.arrayElementType.StartsWith("PPtr<$"))
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = null;
            }

            arrayProperty.DeleteArrayElementAtIndex(index);
        }

        public static object GetPropertyValue(SerializedProperty p)
        {
            PropertyInfo propertyInfo;
            if (s_serializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out propertyInfo))
            {
                return propertyInfo.GetValue(p, null);
            }
            else
            {
                if (p.isArray)
                    return GetPropertyValueArray(p);
                else
                    return GetPropertyValueGeneric(p);
            }
        }

        private static void SetPropertyValue(SerializedProperty p, object v)
        {
            PropertyInfo propertyInfo;
            if (s_serializedPropertyValueAccessorsDict.TryGetValue(p.propertyType, out propertyInfo))
            {
                propertyInfo.SetValue(p, v, null);
            }
            else
            {
                if (p.isArray)
                    SetPropertyValueArray(p, v);
                else
                    SetPropertyValueGeneric(p, v);
            }
        }

        private static object GetPropertyValueArray(SerializedProperty property)
        {
            object[] array = new object[property.arraySize];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                array[i] = GetPropertyValue(item);
            }
            return array;
        }

        private static object GetPropertyValueGeneric(SerializedProperty property)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var iterator = property.Copy();
            if (iterator.Next(true))
            {
                var end = property.GetEndProperty();
                do
                {
                    string name = iterator.name;
                    object value = GetPropertyValue(iterator);
                    dict.Add(name, value);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
            return dict;
        }

        private static void SetPropertyValueArray(SerializedProperty property, object v)
        {
            object[] array = (object[])v;
            property.arraySize = array.Length;
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty item = property.GetArrayElementAtIndex(i);
                SetPropertyValue(item, array[i]);
            }
        }

        private static void SetPropertyValueGeneric(SerializedProperty property, object v)
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)v;
            var iterator = property.Copy();
            if (iterator.Next(true))
            {
                var end = property.GetEndProperty();
                do
                {
                    string name = iterator.name;
                    SetPropertyValue(iterator, dict[name]);
                } while (iterator.Next(false) && iterator.propertyPath != end.propertyPath);
            }
        }

        private static bool ComparePropertyValues(object value1, object value2)
        {
            if (value1 is Dictionary<string, object> && value2 is Dictionary<string, object>)
            {
                var dict1 = (Dictionary<string, object>)value1;
                var dict2 = (Dictionary<string, object>)value2;
                return CompareDictionaries(dict1, dict2);
            }
            else
            {
                return object.Equals(value1, value2);
            }
        }

        private static bool CompareDictionaries(Dictionary<string, object> dict1, Dictionary<string, object> dict2)
        {
            if (dict1.Count != dict2.Count)
                return false;

            foreach (var kvp1 in dict1)
            {
                var key1 = kvp1.Key;
                object value1 = kvp1.Value;

                object value2;
                if (!dict2.TryGetValue(key1, out value2))
                    return false;

                if (!ComparePropertyValues(value1, value2))
                    return false;
            }

            return true;
        }

        private struct EnumerationEntry
        {
            public SerializedProperty keyProperty;
            public SerializedProperty valueProperty;
            public int index;

            public EnumerationEntry(SerializedProperty keyProperty, SerializedProperty valueProperty, int index)
            {
                this.keyProperty = keyProperty;
                this.valueProperty = valueProperty;
                this.index = index;
            }
        }

        private static IEnumerable<EnumerationEntry> EnumerateEntries(SerializedProperty keyArrayProperty, SerializedProperty valueArrayProperty, int startIndex = 0)
        {
            if (keyArrayProperty.arraySize > startIndex)
            {
                int index = startIndex;
                var keyProperty = keyArrayProperty.GetArrayElementAtIndex(startIndex);
                var valueProperty = valueArrayProperty != null ? valueArrayProperty.GetArrayElementAtIndex(startIndex) : null;
                var endProperty = keyArrayProperty.GetEndProperty();

                do
                {
                    yield return new EnumerationEntry(keyProperty, valueProperty, index);
                    index++;
                } while (keyProperty.Next(false)
                    && (valueProperty != null ? valueProperty.Next(false) : true)
                    && !SerializedProperty.EqualContents(keyProperty, endProperty));
            }
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