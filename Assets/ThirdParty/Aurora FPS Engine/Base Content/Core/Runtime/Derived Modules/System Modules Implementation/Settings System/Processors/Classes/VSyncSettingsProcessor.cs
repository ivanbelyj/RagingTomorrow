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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/VSync Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class VSyncSettingsProcessor : SettingsProcessor
    {
        [System.Serializable]
        public class VSyncLevel
        {
            [SerializeField]
            [NotEmpty]
            private string name = "Level 1";

            [SerializeField]
            [Slider(0, 4)]
            private int count = 0;

            public VSyncLevel(string name, int level)
            {
                this.name = name;
                this.count = level;
            }

            #region [Getter / Setter]
            public string GetName()
            {
                return name;
            }

            public void SetName(string value)
            {
                name = value;
            }

            public int GetCount()
            {
                return count;
            }

            public void SetCount(int value)
            {
                count = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        [NotEmpty]
        private string defaultValue = DefaultLevels[0].GetName();

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetLevelLabel")]
        private VSyncLevel[] levels = DefaultLevels;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            Debug.Assert(levels != null, $"<b><color=#FF0000>Add VSync level to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Levels<i>(field)</i>.</color></b>");
            Debug.Assert(levels.Length != 0, $"<b><color=#FF0000>Add shadow level to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Levels<i>(field)</i>.</color></b>");

            dropdown.options.Clear();
            for (int i = 0; i < levels.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(levels[i].GetName()));
            }
        }

        /// <summary>
        /// Save new VSync count value
        /// </summary>
        /// <returns>New VSync count value</returns>
        protected override object OnSave()
        {
            string level = dropdown.options[dropdown.value].text;
            for (int i = 0; i < levels.Length; i++)
            {
                VSyncLevel vSyncLevel = levels[i];
                if (vSyncLevel.GetName() == level)
                {
                    QualitySettings.vSyncCount = vSyncLevel.GetCount();
                }
            }
            return level;
        }

        /// <summary>
        /// Load VSync count
        /// </summary>
        /// <param name="value">VSync count</param>
        protected override void OnLoad(object value)
        {
            string level = value.ToString();
            for (int i = 0; i < levels.Length; i++)
            {
                VSyncLevel vSyncLevel = levels[i];
                if (vSyncLevel.GetName() == level)
                {
                    QualitySettings.vSyncCount = vSyncLevel.GetCount();
                }
            }
            dropdown.value = dropdown.options.FindIndex(t => t.text == level);
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

        #region [Static Properties]
        public static VSyncLevel[] DefaultLevels
        {
            get
            {
                return new VSyncLevel[2]
                {
                    new VSyncLevel("Disabled", 0),
                    new VSyncLevel("Enabled", 1)
                };
            }
        }
        #endregion

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetLevelLabel(UnityEditor.SerializedProperty property, int index)
        {
            string label = property.FindPropertyRelative("name").stringValue;
            return !string.IsNullOrEmpty(label) ? label : string.Format("Level {0}", index + 1);
        }
#endif
        #endregion
    }
}
