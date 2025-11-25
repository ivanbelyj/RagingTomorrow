/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AuroraFPSEditor.Attributes
{
    public sealed class ApexSettings : ScriptableObject
    {
        #region [Const Properties]
        public const string PATH_KEY = "Aurora FPS Editor Path";
        #endregion

        #region [Static Readonly Properties]
        public static readonly Color HeaderColorLightTheme = new Color(0.85f, 0.85f, 0.85f, 1.0f);
        public static readonly Color PropertyColorLightTheme = new Color(0.795f, 0.795f, 0.795f, 1.0f);
        public static readonly Color GroupBorderColorLightTheme = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        public static readonly Color PropertyBorderColorLightTheme = new Color(0.57f, 0.57f, 0.57f, 1.0f);
        public static readonly Color HeaderColorDarkTheme = new Color(0.275f, 0.275f, 0.275f, 1.0f);
        public static readonly Color PropertyColorDarkTheme = new Color(0.23f, 0.23f, 0.23f, 1.0f);
        public static readonly Color GroupBorderColorDarkTheme = Color.black;
        public static readonly Color PropertyBorderColorDarkTheme = Color.black;
        #endregion

        [System.Flags]
        public enum HighlightExpadablePropertyType
        {
            Disabled = 0,
            InsideGroup = 1 << 0,
            OutsideGroup = 1 << 1,
            All = ~0
        }

        [SerializeField]
        private string rootPath = "Assets/Aurora FPS Engine";

        [SerializeField]
        private bool apexEnabled = true;

        [SerializeField]
        private string[] exceptScripts = null;

        [SerializeField]
        private string[] defaultTypes = new string[1] { "InputAction" };

        [SerializeField]
        private bool debugMode = false;

        /// <summary>
        /// Get actual Apex settings asset.
        /// </summary>
        public static ApexSettings _Current;
        public static ApexSettings Current
        {
            get
            {
                if (!EditorBuildSettings.TryGetConfigObject<ApexSettings>(ApexSettingsEditor.BUILD_CONFIG_OBJECT_KEY, out _Current))
                {
                    _Current = Resources.FindObjectsOfTypeAll<ApexSettings>().FirstOrDefault();
                    if (_Current == null)
                    {
                        _Current = CreateInstance<ApexSettings>();
                    }
                }
                return _Current;
            }
        }

        /// <summary>
        /// Get serialized actual Apex settings in SerializedObject representation.
        /// </summary>
        public static SerializedObject SerializedActualSettings
        {
            get
            {
                return new SerializedObject(Current);
            }
        }

        /// <summary>
        /// Reset specific settings to default.
        /// </summary>
        /// <param name="settings">Settings reference.</param>
        public static void ResetSettings(ref ApexSettings settings)
        {
            settings.SetRootPath("Assets/Aurora FPS Engine");
            settings.ApexEnabled(true);
            settings.DebugMode(false);
            settings.SetDefaultTypes(new string[1] { "InputAction" });
        }

        #region [Static Properties]
        /// <summary>
        /// Get actual header color regarding current Unity editor theme.
        /// </summary>
        public static Color HeaderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? HeaderColorDarkTheme : HeaderColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual property color regarding current Unity editor theme.
        /// </summary>
        public static Color PropertyColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? PropertyColorDarkTheme : PropertyColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual group border color regarding current Unity editor theme.
        /// </summary>
        public static Color GroupBorderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? GroupBorderColorDarkTheme : GroupBorderColorLightTheme;
            }
        }

        /// <summary>
        /// Get actual property border color regarding current Unity editor theme.
        /// </summary>
        public static Color PropertyBorderColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? PropertyBorderColorDarkTheme : PropertyBorderColorLightTheme;
            }
        }

        #endregion

        #region [Getter / Setter]
        public string GetRootPath()
        {
            return rootPath;
        }

        public void SetRootPath(string value)
        {
            rootPath = value;
        }

        public bool ApexEnabled()
        {
            return apexEnabled;
        }

        public void ApexEnabled(bool value)
        {
            apexEnabled = value;
        }

        public bool DebugMode()
        {
            return debugMode;
        }

        public void DebugMode(bool value)
        {
            debugMode = value;
        }

        public string[] GetExceptScripts()
        {
            return exceptScripts;
        }

        public void SetExceptScripts(string[] value)
        {
            exceptScripts = value;
        }

        public string[] GetDefaultTypes()
        {
            return defaultTypes;
        }

        public void SetDefaultTypes(string[] value)
        {
            defaultTypes = value;
        }
        #endregion
    }
}