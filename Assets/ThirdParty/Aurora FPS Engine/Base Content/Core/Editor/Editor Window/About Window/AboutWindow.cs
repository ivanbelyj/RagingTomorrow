/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using AuroraFPSEditor.Attributes;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor
{
    internal sealed class AboutWindow : EditorWindow
    {
        private const string LOGOTYPE_RELATIVE_PATH = "Base Content/Core/Editor/Editor Resources/Images/Logotype/AuroraEngine_Logo_Black.png";

        private static readonly Vector2Int Size = new Vector2Int(450, 300);

        internal static class ContentProperties
        {
            public static GUIStyle Title
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 20;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }

            public static GUIStyle Copyright
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 10;
                    style.fontStyle = FontStyle.Normal;
                    style.normal.textColor = Color.gray;
                    return style;
                }
            }

            public static GUIStyle Version
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 10;
                    style.fontStyle = FontStyle.Bold;
                    style.normal.textColor = Color.gray;
                    return style;
                }
            }

            public static GUIStyle PersonTitle
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 11;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }

            public static GUIStyle PersonPost
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.UpperLeft;
                    style.fontSize = 11;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }

            public static GUIStyle Person
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.alignment = TextAnchor.UpperLeft;
                    style.fontSize = 11;
                    style.fontStyle = FontStyle.Bold;
                    style.wordWrap = true;
                    return style;
                }
            }

            public static GUIStyle SpecialThanks
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 11;
                    style.fontStyle = FontStyle.Italic;
                    style.alignment = TextAnchor.UpperLeft;
                    style.wordWrap = true;
                    return style;
                }
            }
        }

        private Texture2D texture;

        private void OnEnable()
        {
            string path = System.IO.Path.Combine(ApexSettings.Current.GetRootPath(), LOGOTYPE_RELATIVE_PATH);
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);
                    if (texture != null)
                    {
                        GUILayout.Label(texture, GUILayout.Width(100), GUILayout.Height(128));
                    }
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                {
                    // Title section.
                    GUILayout.Space(10);
                    GUILayout.Label("Aurora FPS Engine", ContentProperties.Title);
                    GUILayout.Label("Version: 2.5.6", ContentProperties.Version);

                    // Team section.
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Project Director:", ContentProperties.PersonPost);
                        GUILayout.Space(5);
                        GUILayout.Label("Tamerlan Shakirov", ContentProperties.Person);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("3D Supervisor:", ContentProperties.PersonPost);
                        GUILayout.Space(15);
                        GUILayout.Label("Daniil Kostyuk", ContentProperties.Person);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Programmers:", ContentProperties.PersonPost);
                        GUILayout.Space(20);
                        GUILayout.Label("Davleev Zinnur\nDeryabin Vladimir\nAlexandra Averyanova", ContentProperties.Person);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("3D Artist:", ContentProperties.PersonPost);
                        GUILayout.Space(45);
                        GUILayout.Label("Askar Khaibullin\nDinara Gabdullina", ContentProperties.Person);
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);
                    // Special thanks section.
                    GUILayout.Label("Special Thanks", ContentProperties.PersonTitle);
                    GUILayout.Label("Ali Alwasiti, Jacob Dufault, Ben Hymers, Tate McCormick, Michael Aganier, Nathan Loewen, Chen Tao, Markus Ollinger, Taku Kobayashi", ContentProperties.SpecialThanks);

                    // Copyright section.
                    GUILayout.FlexibleSpace();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("Copyright 2019-2023 Renowned Games All rights reserved.", ContentProperties.Copyright);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            // Footer section.
            GUILayout.BeginHorizontal("OL Title");
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Visit the Publisher Website"))
                {
                    Help.BrowseURL("https://assetstore.unity.com/publishers/26774");
                }

                if (GUILayout.Button("License Agreement"))
                {
                    Help.BrowseURL("https://unity3d.com/legal/as_terms/#section-2-end-users-rights-and-obligations");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
        }

        [MenuItem("Aurora FPS Engine/About", false, 0)]
        public static void Open()
        {
            AboutWindow window = GetWindow<AboutWindow>(true, "About", true);
            window.minSize = Size;
            window.maxSize = Size;
            window.Show();
        }
    }
}