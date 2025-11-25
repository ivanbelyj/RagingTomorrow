/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public class VariablesView : BehaviourTreeEditorView
    {
        public new class UxmlFactory : UxmlFactory<VariablesView, VisualElement.UxmlTraits> { }

        private SerializedObject globalSerializedObject;
        private SerializedObject localSerializedObject;

        private GlobalVariablesStorage globalVariables;

        private string variableName;
        private Type selectedType = typeof(TransformVariable);

        private Vector2 scrollPosition;
        private int toolbarIndex;

        private GUIContent addButtonContent;
        private GUIContent removeButtonContent;

        public VariablesView()
        {
            GlobalVariablesStorage[] storages = Resources.LoadAll<GlobalVariablesStorage>(string.Empty);
            if (storages.Length == 0)
            {
                GlobalVariablesStorage storage = ScriptableObject.CreateInstance<GlobalVariablesStorage>();

                string resourcesPath = Directory.GetDirectories(ApexSettings.Current.GetRootPath(), "resources", SearchOption.AllDirectories).FirstOrDefault();
                if (string.IsNullOrEmpty(resourcesPath))
                {
                    resourcesPath = Path.Combine(ApexSettings.Current.GetRootPath(), "Base Content", "Resources", "AI", "Behaviour Tree", "Variables");
                    Directory.CreateDirectory(resourcesPath);
                }

                string path = AssetDatabase.GenerateUniqueAssetPath($"{resourcesPath}/New {storage.GetType().Name}.asset");
                AssetDatabase.CreateAsset(storage, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Assert(storages.Length == 1, "You can use only one global variable storage asset file in resources directory!");
                globalVariables = storages[0];
            }

            globalSerializedObject = new SerializedObject(globalVariables);

            addButtonContent = EditorGUIUtility.IconContent("Toolbar Plus");
            removeButtonContent = EditorGUIUtility.IconContent("Toolbar Minus");

            UpdateSelection(null);
        }

        public void UpdateSelection(BehaviourTreeAsset behaviourTree)
        {
            if (behaviourTree != null)
            {
                localSerializedObject = new SerializedObject(behaviourTree);
            }

            Clear();
            Add(new IMGUIContainer(() => OnGUI(behaviourTree)));
        }

        private void OnGUI(BehaviourTreeAsset behaviourTree)
        {
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 70;

            EditorGUI.BeginDisabledGroup(behaviourTree == null);

            // Toolbar
            Rect toolbarPosition = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
            toolbarIndex = GUI.Toolbar(toolbarPosition, toolbarIndex, new string[] { "Global", "Local" });

            GUILayout.Space(3);

            // Adding Variables Menu
            Rect position = GUILayoutUtility.GetRect(0, 38);
            position.x += 2;
            position.width -= 4;

            Rect variablePosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            variableName = EditorGUI.TextField(variablePosition, "Name", variableName);

            Rect typePosition = new Rect(position.x, variablePosition.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, variablePosition.height);
            typePosition = EditorGUI.PrefixLabel(typePosition, new GUIContent("Type"));
            typePosition.width -= 26;
            if (EditorGUI.DropdownButton(typePosition, new GUIContent(selectedType.Name.Replace("Variable", "")), FocusType.Passive))
            {
                GetTypesMenu().DropDown(typePosition);
            }
            Rect dropDownPosition = new Rect(typePosition.xMax, typePosition.y, 26, typePosition.height);
            if (GUI.Button(dropDownPosition, addButtonContent))
            {
                if (ValidVariableName(variableName))
                {
                    variableName = variableName.Trim();
                    TreeVariable variable = Activator.CreateInstance(selectedType) as TreeVariable;

                    switch (toolbarIndex)
                    {
                        case 0:
                            globalVariables.AddVariable(variableName, variable);
                            EditorUtility.SetDirty(globalVariables);
                            AssetDatabase.SaveAssets();

                            globalSerializedObject = new SerializedObject(globalVariables);
                            break;
                        case 1:
                            behaviourTreeEditor.GetBehaviourTree().AddVariable("Local/" + variableName, variable);
                            EditorUtility.SetDirty(behaviourTreeEditor.GetBehaviourTree());
                            AssetDatabase.SaveAssets();

                            localSerializedObject = new SerializedObject(behaviourTreeEditor.GetBehaviourTree());
                            break;
                    }
                }
            }

            EditorGUI.EndDisabledGroup();

            if (behaviourTree == null)
            {
                EditorGUILayout.HelpBox("No behaviour tree selected. Create a new behavior tree or select one of the created ones.", MessageType.Info);
                return;
            }

            GUILayout.Space(3);

            DrawHorizontalLine(new Color(35 / 255f, 35 / 255f, 35 / 255f));
            DrawHorizontalLine(new Color(96 / 255f, 96 / 255f, 96 / 255f));

            // Variables
            if (toolbarIndex == 0)
            {
                SerializedProperty serializedProperty = globalSerializedObject.FindProperty("variables");
                serializedProperty.serializedObject.Update();

                SerializedProperty keys = serializedProperty.FindPropertyRelative("keys");
                SerializedProperty values = serializedProperty.FindPropertyRelative("values");

                int removeIndex = -1;

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty key = keys.GetArrayElementAtIndex(i);
                    SerializedProperty value = values.GetArrayElementAtIndex(i);

                    // Type
                    Rect typePos = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                    typePos.x += 2;
                    typePos.width -= 4;
                    typePos = EditorGUI.PrefixLabel(typePos, new GUIContent("Type"));
                    typePos.width -= 20;

                    if (value.FindPropertyRelative("value") == null)
                    {
                        removeIndex = i;
                        break;
                    }

                    Type type = ApexReflection.GetPropertyType(value.FindPropertyRelative("value"));
                    if (type.Name == "List`1")
                    {
                        EditorGUI.LabelField(typePos, new GUIContent($"List<{type.GenericTypeArguments[0].Name}>"));
                    }
                    else
                    {
                        EditorGUI.LabelField(typePos, new GUIContent(type.Name));
                    }

                    typePos.x += typePos.width + 2;
                    typePos.y += 2;
                    typePos.width = 20;
                    if (GUI.Button(typePos, removeButtonContent, "IconButton"))
                    {
                        removeIndex = i;
                        break;
                    }

                    GUILayout.Space(2);

                    // Name
                    Rect namePos = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                    namePos.x += 2;
                    namePos.width -= 4;
                    namePos = EditorGUI.PrefixLabel(namePos, new GUIContent("Name"));
                    key.stringValue = EditorGUI.TextField(namePos, key.stringValue);

                    GUILayout.Space(2);
                    // Value
                    GUIContent valueContent = new GUIContent("Value");
                    SerializedProperty valueProperty = value.FindPropertyRelative("value");
                    float propertyHeight = EditorGUI.GetPropertyHeight(valueProperty, valueContent, true);
                    Rect valuePos = GUILayoutUtility.GetRect(0, propertyHeight);
                    valuePos.x += 2;
                    valuePos.width -= 4;
                    EditorGUI.PropertyField(valuePos, value.FindPropertyRelative("value"), new GUIContent("Value"), true);

                    GUILayout.Space(2);

                    DrawHorizontalLine(new Color(35 / 255f, 35 / 255f, 35 / 255f));
                    DrawHorizontalLine(new Color(96 / 255f, 96 / 255f, 96 / 255f));
                }

                GUILayout.EndScrollView();

                if (removeIndex != -1)
                {
                    keys.DeleteArrayElementAtIndex(removeIndex);
                    values.DeleteArrayElementAtIndex(removeIndex);
                }

                serializedProperty.serializedObject.ApplyModifiedProperties();
            }
            else if (toolbarIndex == 1)
            {
                SerializedProperty serializedProperty = localSerializedObject.FindProperty("variables");
                serializedProperty.serializedObject.Update();

                SerializedProperty keys = serializedProperty.FindPropertyRelative("keys");
                SerializedProperty values = serializedProperty.FindPropertyRelative("values");

                int removeIndex = -1;

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                for (int i = 0; i < keys.arraySize; i++)
                {
                    SerializedProperty key = keys.GetArrayElementAtIndex(i);
                    SerializedProperty value = values.GetArrayElementAtIndex(i);

                    // Type
                    if (value.FindPropertyRelative("value") == null)
                    {
                        removeIndex = i;
                        break;
                    }

                    Type type = ApexReflection.GetPropertyType(value.FindPropertyRelative("value"));
                    Rect typePos = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                    typePos.x += 2;
                    typePos.width -= 4;
                    typePos = EditorGUI.PrefixLabel(typePos, new GUIContent("Type"));
                    typePos.width -= 20;
                    EditorGUI.LabelField(typePos, new GUIContent(type.Name));
                    typePos.x += typePos.width + 2;
                    typePos.y += 2;
                    typePos.width = 20;
                    if (GUI.Button(typePos, removeButtonContent, "IconButton"))
                    {
                        removeIndex = i;
                        break;
                    }

                    GUILayout.Space(2);

                    // Name
                    Rect namePos = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
                    namePos.x += 2;
                    namePos.width -= 4;
                    namePos = EditorGUI.PrefixLabel(namePos, new GUIContent("Name"));
                    key.stringValue = EditorGUI.TextField(namePos, key.stringValue);

                    GUILayout.Space(2);

                    // Value
                    GUIContent valueContent = new GUIContent("Value");
                    SerializedProperty valueProperty = value.FindPropertyRelative("value");
                    float propertyHeight = EditorGUI.GetPropertyHeight(valueProperty, valueContent, true);
                    Rect valuePos = GUILayoutUtility.GetRect(0, propertyHeight);
                    valuePos.x += 2;
                    valuePos.width -= 4;
                    EditorGUI.PropertyField(valuePos, value.FindPropertyRelative("value"), new GUIContent("Value"), true);

                    GUILayout.Space(2);

                    DrawHorizontalLine(new Color(35 / 255f, 35 / 255f, 35 / 255f));
                    DrawHorizontalLine(new Color(96 / 255f, 96 / 255f, 96 / 255f));
                }

                GUILayout.EndScrollView();

                if (removeIndex != -1)
                {
                    keys.DeleteArrayElementAtIndex(removeIndex);
                    values.DeleteArrayElementAtIndex(removeIndex);
                }

                serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            EditorGUIUtility.labelWidth = labelWidth;
        }

        private GenericMenu GetTypesMenu()
        {
            GenericMenu menu = new GenericMenu();

            TypeCache.TypeCollection variables = TypeCache.GetTypesDerivedFrom<TreeVariable>();
            for (int i = 0; i < variables.Count; i++)
            {
                Type variableType = variables[i];
                if (variableType.IsGenericType)
                {
                    continue;
                }

                menu.AddItem(new GUIContent(variableType.Name.Replace("Variable", "")), false, () =>
                {
                    selectedType = variableType;
                });
            }

            return menu;
        }

        private bool ValidVariableName(string variableName)
        {
            return !string.IsNullOrWhiteSpace(variableName) && variableName.IndexOf('/') == -1 && variableName.IndexOf('\\') == -1;
        }

        private void DrawHorizontalLine(Color color)
        {
            Rect position = GUILayoutUtility.GetRect(0, 1);
            EditorGUI.DrawRect(position, color);
        }
    }
}