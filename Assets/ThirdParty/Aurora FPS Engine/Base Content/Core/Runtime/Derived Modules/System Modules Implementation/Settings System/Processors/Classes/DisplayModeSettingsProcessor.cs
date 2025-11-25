/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/Display Mode Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class DisplayModeSettingsProcessor : SettingsProcessor
    {
        [System.Serializable]
        public struct DisplayMode
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private FullScreenMode fullScreenMode;

            [SerializeField]
            private bool fullScreen;

            public DisplayMode(string name, FullScreenMode fullScreenMode, bool fullScreen)
            {
                this.name = name;
                this.fullScreenMode = fullScreenMode;
                this.fullScreen = fullScreen;
            }

            #region [Getter / Setter]
            public string GetDisplayName()
            {
                return name;
            }

            public void SetDisplayName(string value)
            {
                name = value;
            }

            public FullScreenMode GetFullScreenMode()
            {
                return fullScreenMode;
            }

            public void SetFullScreenMode(FullScreenMode value)
            {
                fullScreenMode = value;
            }

            public bool Fullscreen()
            {
                return fullScreen;
            }

            public void Fullscreen(bool value)
            {
                fullScreen = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        private string defaultValue = DefaultModes[0].GetDisplayName();

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetModeLabel")]
        private DisplayMode[] displayModes = DefaultModes;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            dropdown.options.Clear();
            for (int i = 0; i < displayModes.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(displayModes[i].GetDisplayName()));
            }
        }

        /// <summary>
        /// Save new screen resolution value
        /// </summary>
        /// <returns>New screen resolution</returns>
        protected override object OnSave()
        {
            string option = dropdown.options[dropdown.value].text;

            for (int i = 0; i < displayModes.Length; i++)
            {
                DisplayMode mode = displayModes[i];
                if(option == mode.GetDisplayName())
                {
                    Screen.fullScreen = mode.Fullscreen();
                    Screen.fullScreenMode = mode.GetFullScreenMode();
                }
            }
            return option;
        }

        /// <summary>
        /// Load screen resolution
        /// </summary>
        /// <param name="value">Screen resolution as object</param>
        protected override void OnLoad(object value)
        {
            string option = (string)value;
            for (int i = 0; i < displayModes.Length; i++)
            {
                DisplayMode mode = displayModes[i];
                if (option == mode.GetDisplayName())
                {
                    Screen.fullScreen = mode.Fullscreen();
                    Screen.fullScreenMode = mode.GetFullScreenMode();
                }
            }
            dropdown.value = dropdown.options.FindIndex(t => t.text == option);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return defaultValue;
        }

        #region [Static Methods]
        public static DisplayMode[] DefaultModes
        {
            get
            {
                return new DisplayMode[4]
                {
                    new DisplayMode("Fullscreen", FullScreenMode.FullScreenWindow, true),
                    new DisplayMode("Exclusive fullscreen", FullScreenMode.ExclusiveFullScreen, true),
                    new DisplayMode("Maximized window", FullScreenMode.MaximizedWindow, false),
                    new DisplayMode("Windowed", FullScreenMode.Windowed, false)
                };
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetModeLabel(UnityEditor.SerializedProperty property, int index)
        {
            string label = property.FindPropertyRelative("name").stringValue;
            return !string.IsNullOrEmpty(label) ? label : string.Format("Mode {0}", index + 1);
        }
#endif
        #endregion
    }
}
