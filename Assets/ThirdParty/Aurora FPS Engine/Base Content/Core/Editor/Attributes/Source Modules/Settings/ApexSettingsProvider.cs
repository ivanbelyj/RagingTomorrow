/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexSettingsProvider : SettingsProvider
    {
        private ApexSettings settings;
        private Editor editor;

        /// <summary>
        /// ApexSettingsProvider constructor.
        /// </summary>
        /// <param name="path">Path used to place the SettingsProvider in the tree view of the Settings window. The path should be unique among all other settings paths and should use "/" as its separator.</param>
        /// <param name="scopes">Scope of the SettingsProvider. The Scope determines whether the SettingsProvider appears in the Preferences window (SettingsScope.User) or the Settings window (SettingsScope.Project).</param>
        /// <param name="keywords">List of keywords to compare against what the user is searching for. When the user enters values in the search box on the Settings window, SettingsProvider.HasSearchInterest tries to match those keywords to this list.</param>
        public ApexSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        /// <summary>
        /// Register ApexSettingsProvider in Project Settings window.
        /// </summary>
        /// <returns>New ApexSettingsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider RegisterApexSettingsProvider()
        {
            return new ApexSettingsProvider("Project/Aurora FPS Engine", SettingsScope.Project);
        }

        /// <summary>
        /// Use this function to implement a handler for when the user clicks on the Settings in the Settings window. You can fetch a settings Asset or set up UIElements UI from this function.
        /// </summary>
        /// <param name="searchContext">Search context in the search box on the Settings window.</param>
        /// <param name="rootElement">Root of the UIElements tree. If you add to this root, the SettingsProvider uses UIElements instead of calling SettingsProvider.OnGUI to build the UI. If you do not add to this VisualElement, then you must use the IMGUI to build the UI.</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            if (EditorBuildSettings.TryGetConfigObject<ApexSettings>(ApexSettingsEditor.BUILD_CONFIG_OBJECT_KEY, out ApexSettings value))
            {
                settings = value;
            }
            else
            {
                settings = Resources.FindObjectsOfTypeAll<ApexSettings>().FirstOrDefault();
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<ApexSettings>();
                }
            }
            editor = Editor.CreateEditor(settings);
        }
    
        /// <summary>
        /// Use this function to override drawing the title for the SettingsProvider using IMGUI. This allows you to add custom UI (such as a toolbar button) next to the title. 
        /// AssetSettingsProvider uses this mechanism to display the "add to preset" and the "help" buttons.
        /// </summary>
        public override void OnTitleBarGUI()
        {
            Rect position = GUILayoutUtility.GetRect(0, 0);
            position.x -= 19.0f;
            position.y += 6.0f;
            position.width = 20;
            position.height = 20;

            Rect popupPosition = new Rect(position.x, position.y, position.width, position.height);
            if (GUI.Button(popupPosition, EditorGUIUtility.IconContent("_Popup"), "IconButton"))
            {
                GenericMenu popupMenu = new GenericMenu();
                popupMenu.AddItem(new GUIContent("Reset", "Reset setting to default."), false, ResetSettingsFunction);

                Rect dropdownPosition = new Rect(popupPosition.x - 88, popupPosition.y, popupPosition.width, popupPosition.height);
                popupMenu.DropDown(dropdownPosition);
            }
        }

        /// <summary>
        /// Use this function to draw the UI based on IMGUI. This assumes you haven't added any children to the rootElement passed to the OnActivate function.
        /// </summary>
        /// <param name="searchContext">Search context for the Settings window. Used to show or hide relevant properties.</param>
        public override void OnGUI(string searchContext)
        {
            if (settings != null && editor != null)
            {
                bool isNativeAsset = AssetDatabase.IsNativeAsset(settings);

                if (!isNativeAsset)
                {
                    Rect position = GUILayoutUtility.GetRect(0, 90);
                    position.x += 10;
                    position.y += 10;
                    position.width -= 15;

                    Rect helpBoxPosition = new Rect(position.x, position.y, position.width, 35);
                    EditorGUI.HelpBox(helpBoxPosition, "If you want to edit the settings create new settings asset.", MessageType.Info);

                    Rect buttonPosition = new Rect(position.x, helpBoxPosition.yMax + EditorGUIUtility.standardVerticalSpacing, position.width, 35);
                    if (GUI.Button(buttonPosition, "Create Config"))
                    {
                        ApexSettings settings = ScriptableObject.CreateInstance<ApexSettings>();

                        string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/New {settings.GetType().Name}.asset");
                        AssetDatabase.CreateAsset(settings, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        EditorBuildSettings.AddConfigObject(ApexSettingsEditor.BUILD_CONFIG_OBJECT_KEY, settings, true);

                        EditorGUIUtility.PingObject(settings);
                    }
                }

                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                EditorGUIUtility.labelWidth = 248;

                EditorGUI.BeginDisabledGroup(!isNativeAsset);
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 150;
                editor.OnInspectorGUI();
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUI.EndDisabledGroup();

                GUILayout.EndVertical();
                GUILayout.Space(5);
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Reset settings function to use in popup generic menu.
        /// </summary>
        private void ResetSettingsFunction()
        {
            ApexSettings.ResetSettings(ref settings);
            ApexEditor.RepaintAllInstances();
        }
    }
}