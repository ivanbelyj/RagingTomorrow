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
    //[InitializeOnLoad]
    internal sealed class GettingStartedWindow : EditorWindow
    {
        private const string GSW_KEY = "Aurora FPS Engine GS Window Is Open";

        //static GettingStartedWindow()
        //{
        //    EditorApplication.delayCall += () =>
        //    {
        //        if (!EditorPrefs.HasKey(GSW_KEY) || !EditorPrefs.GetBool(GSW_KEY))
        //        {
        //            Open();
        //            EditorPrefs.SetBool(GSW_KEY, true);
        //        }
        //    };
        //}

        internal static class Styles
        {
            public static GUIStyle Title
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 15;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }

            public static GUIStyle Title2
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 12;
                    style.fontStyle = FontStyle.Bold;
                    return style;
                }
            }

            public static GUIStyle Text
            {
                get
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);
                    style.fontSize = 11;
                    style.fontStyle = FontStyle.Normal;
                    style.wordWrap = true;
                    return style;
                }
            }
        }

        private Texture2D texture;

        private void OnEnable()
        {
            string path = System.IO.Path.Combine(ApexSettings.Current.GetRootPath(), "Base Content/Core/Editor/Editor Resources/Images/about-window-icon.png");
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private void OnGUI()
        {
            //Icon
            if (texture != null)
            {
                Rect iconPosition = new Rect(20, 50, 132, 132);
                GUI.Label(iconPosition, texture);
            }


            // Label
            Rect labelPosition = new Rect(170, 35, 275, 20);
            GUI.Label(labelPosition, "Welcome to Aurora FPS Engine!", Styles.Title);

            Rect subTitlePosition = new Rect(170, 55, 275, 35);
            GUI.Label(subTitlePosition, "Thank you for purchase the most powerful toolkit for creating games on the Unity engine!", Styles.Text);

            Rect requirementsTitlePosition = new Rect(170, 95, 275, 20);
            GUI.Label(requirementsTitlePosition, "Requirements", Styles.Title2);

            Rect requirementsTextPosition = new Rect(170, 110, 275, 35);
            GUI.Label(requirementsTextPosition, "Aurora FPS Engine requires Unity 2019.4 LTS or above.", Styles.Text);

            Rect setupTitlePosition = new Rect(170, 145, 275, 20);
            GUI.Label(setupTitlePosition, "Setup", Styles.Title2);

            Rect setupTextPosition = new Rect(170, 160, 275, 35);
            GUI.Label(setupTextPosition, "After the package has been imported install required project settings.", Styles.Text);

            Rect footerLinePosition = new Rect(0, 220, 450, 0.5f);
            EditorGUI.DrawRect(footerLinePosition, Color.black);

            Rect footerBackgroundPosition = new Rect(0, 220.5f, 450, 29);
            EditorGUI.DrawRect(footerBackgroundPosition, new Color(0.19f, 0.19f, 0.19f, 1.0f));

            Rect installPSButtonPosition = new Rect(295, 225.5f, 150, 20);
            if (GUI.Button(installPSButtonPosition, "Install Project Settings"))
            {
                UtilitiesMenu.InstallProjectSettings();
                Close();
            }
        }

        // [MenuItem("Aurora FPS Engine/INTERNAL/Getting Started")]
        private static void Open()
        {
            GettingStartedWindow window = GetWindow<GettingStartedWindow>(true, "Getting Started", true);
            window.minSize = new Vector2(450, 250);
            window.maxSize = new Vector2(450, 250);
            window.Show();
        }
    }
}