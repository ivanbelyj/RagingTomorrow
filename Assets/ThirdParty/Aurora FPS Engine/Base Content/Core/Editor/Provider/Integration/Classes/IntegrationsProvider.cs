/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSEditor.Attributes;
using AuroraFPSRuntime.Utilities.FullSerializer;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AuroraFPSEditor.Internal.Integrations
{
    sealed class IntegrationsProvider : SettingsProvider
    {
        public static readonly Color BackgroundColor = new Color(0.25f, 0.25f, 0.25f, 1.0f);
        public static readonly Color BorderColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        public static readonly Color SeparatorColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);

        [System.Serializable]
        private struct Integration
        {
            public readonly string name;
            public readonly string path;
            public readonly string[] dependencies;
        }

        private ApexSettings settings;
        private Integration[] integrations;
        private string path;

        /// <summary>
        /// Integrations provider constructor.
        /// </summary>
        /// <param name="path">Path used to place the SettingsProvider in the tree view of the Settings window. The path should be unique among all other settings paths and should use "/" as its separator.</param>
        /// <param name="scopes">Scope of the SettingsProvider. The Scope determines whether the SettingsProvider appears in the Preferences window (SettingsScope.User) or the Settings window (SettingsScope.Project).</param>
        /// <param name="keywords">List of keywords to compare against what the user is searching for. When the user enters values in the search box on the Settings window, SettingsProvider.HasSearchInterest tries to match those keywords to this list.</param>
        public IntegrationsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords) { }

        /// <summary>
        /// Use this function to implement a handler for when the user clicks on the Settings in the Settings window. You can fetch a settings Asset or set up UIElements UI from this function.
        /// </summary>
        /// <param name="searchContext">Search context in the search box on the Settings window.</param>
        /// <param name="rootElement">Root of the UIElements tree. If you add to this root, the SettingsProvider uses UIElements instead of calling SettingsProvider.OnGUI to build the UI. If you do not add to this VisualElement, then you must use the IMGUI to build the UI.</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            settings = ApexSettings.Current;
            path = $"{settings.GetRootPath()}/Base Content/Core/Editor/Editor Resources/Integrations";
            string manifestPath = string.Empty;
            foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if(fileName == "manifest")
                {
                    manifestPath = filePath;
                    break;
                }
            }

            using(StreamReader reader = new StreamReader(manifestPath))
            {
                fsSerializer serializer = new fsSerializer();
                fsData data = fsJsonParser.Parse(reader.ReadToEnd());
                object deserialized = null;
                serializer.TryDeserialize(data, typeof(Integration[]), ref deserialized).AssertSuccessWithoutWarnings();
                integrations = (Integration[])deserialized;
            }
        }

        /// <summary>
        /// Use this function to draw the UI based on IMGUI. This assumes you haven't added any children to the rootElement passed to the OnActivate function.
        /// </summary>
        /// <param name="searchContext">Search context for the Settings window. Used to show or hide relevant properties.</param>
        public override void OnGUI(string searchContext)
        {
            Rect position = GUILayoutUtility.GetRect(0, 0);
            position.x += 10;
            position.y += 10;
            position.width -= 20;

            if (integrations != null)
            {
                position.height = integrations.Length * 20;
                DrawBackground(position);

                Rect packagePosition = new Rect(position.x, position.y, position.width, 20);
                for (int i = 0; i < integrations.Length; i++)
                {
                    Integration integration = integrations[i];

                    Rect labelPosition = new Rect(packagePosition.x + 5, packagePosition.y, packagePosition.width, packagePosition.height);
                    GUIContent labelContent = new GUIContent(integration.name);
                    GUI.Label(labelPosition, labelContent);

                    Rect importPosition = new Rect(packagePosition.xMax - 49, packagePosition.y + 0.5f, 50, packagePosition.height);
                    if(GUI.Button(importPosition, "Import", EditorStyles.toolbarButton))
                    {
                        if(integration.dependencies != null)
                        {
                            int key = EditorUtility.DisplayDialogComplex(
                                                    "Integration Installer",
                                                    "Before importing this integration, you need to install dependencies." +
                                                    "\n\nImporting without installing dependencies can cause errors.", 
                                                    "Import", 
                                                    "Cancel", 
                                                    "View Dependencies");

                            if(key == 1)
                            {
                                break;
                            }
                            else if(key == 2)
                            {
                                for (int j = 0; j < integration.dependencies.Length; j++)
                                {
                                    Help.BrowseURL(integration.dependencies[j]);
                                }
                                break;
                            }
                        }
                        AssetDatabase.ImportPackage(Path.Combine(path, integration.path), true);
                    }

                    packagePosition.y += packagePosition.height;
                    Rect linePosition = new Rect(packagePosition.x, packagePosition.y, packagePosition.width + 1, 0.75f);
                    EditorGUI.DrawRect(linePosition, BorderColor);

                }
            }
            else
            {
                position.height = 20;
                GUI.Label(position, "Integrations not found...", EditorStyles.centeredGreyMiniLabel);
            }
        }

        private void DrawBackground(Rect position)
        {
            Rect backgroundPosition = new Rect(position.x, position.y, position.width, position.height);
            EditorGUI.DrawRect(backgroundPosition, BackgroundColor);

            Rect topBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMin, backgroundPosition.width + 0.5f, 0.75f);
            EditorGUI.DrawRect(topBorderPosition, BorderColor);

            Rect bottomBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMax, backgroundPosition.width + 0.5f, 0.75f);
            EditorGUI.DrawRect(bottomBorderPosition, BorderColor);

            Rect leftBorderPosition = new Rect(backgroundPosition.xMin, backgroundPosition.yMin, 0.75f, backgroundPosition.height);
            EditorGUI.DrawRect(leftBorderPosition, BorderColor);

            Rect rightBorderPosition = new Rect(backgroundPosition.xMax, backgroundPosition.yMin, 0.75f, backgroundPosition.height);
            EditorGUI.DrawRect(rightBorderPosition, BorderColor);
        }

        /// <summary>
        /// Register IntegrationsProvider in Project Settings window.
        /// </summary>
        /// <returns>New IntegrationsProvider instance.</returns>
        [SettingsProvider]
        public static SettingsProvider RegisterIntegrationProvider()
        {
            return new IntegrationsProvider("Project/Aurora FPS Engine/Integrations", SettingsScope.Project);
        }
    }
}