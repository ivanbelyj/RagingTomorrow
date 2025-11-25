/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;

namespace AuroraFPSEditor.Attributes
{
    [CustomEditor(typeof(ApexSettings))]
    public class ApexSettingsEditor : Editor
    {
        public const string BUILD_CONFIG_OBJECT_KEY = "Apex Settings Config Asset Key";

        private SerializedProperty rootPath;
        private SerializedProperty apexEnabled;
        private ReorderableList exceptScripts;
        private ReorderableList defaultTypes;
        private SerializedProperty debugMode;
        private SerializedProperty readmeAtStartup;

        private void OnEnable()
        {
            rootPath = serializedObject.FindProperty(nameof(rootPath));
            apexEnabled = serializedObject.FindProperty(nameof(apexEnabled));
            debugMode = serializedObject.FindProperty(nameof(debugMode));
            readmeAtStartup = serializedObject.FindProperty(nameof(readmeAtStartup));
            exceptScripts = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(exceptScripts)));
            defaultTypes = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(defaultTypes)));
            exceptScripts.headerHeight = 1;
            exceptScripts.drawElementCallback += (position, index, isActive, isFocused) =>
            {
                SerializedProperty element = exceptScripts.serializedProperty.GetArrayElementAtIndex(index);
                position.y += 1;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };
            defaultTypes.headerHeight = 1;
            defaultTypes.drawElementCallback += (position, index, isActive, isFocused) =>
            {
                SerializedProperty element = defaultTypes.serializedProperty.GetArrayElementAtIndex(index);
                position.y += 1;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(position, element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(rootPath, new GUIContent("Root Path"));
            EditorGUILayout.PropertyField(apexEnabled, new GUIContent("Attributes Enabled"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(debugMode, new GUIContent("Debug Mode"));
            if (EditorGUI.EndChangeCheck())
            {
#if UNITY_2022_2_OR_NEWER
                string definesString = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
#else
                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
                List<string> allDefines = definesString.Split(';').ToList();
                if (debugMode.boolValue)
                    allDefines.Add("AURORA_ENGINE_DEBUG");
                else
                    allDefines.RemoveAll(v => v == "AURORA_ENGINE_DEBUG");

#if UNITY_2022_2_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), string.Join(";", allDefines.ToArray()));
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
#endif
            }

            exceptScripts.serializedProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(exceptScripts.serializedProperty.isExpanded, "Except Script");
            if (exceptScripts.serializedProperty.isExpanded)
            {
                exceptScripts.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            defaultTypes.serializedProperty.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(defaultTypes.serializedProperty.isExpanded, "Default Editor");
            if (defaultTypes.serializedProperty.isExpanded)
            {
                defaultTypes.DoLayoutList();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            serializedObject.ApplyModifiedProperties();

            if (AssetDatabase.IsNativeAsset(target) &&
               target != ApexSettings.Current &&
               GUILayout.Button("Make as Global Config", GUILayout.Height(30)))
            {
                EditorBuildSettings.AddConfigObject(BUILD_CONFIG_OBJECT_KEY, target, true);
            }
        }
    }
}
