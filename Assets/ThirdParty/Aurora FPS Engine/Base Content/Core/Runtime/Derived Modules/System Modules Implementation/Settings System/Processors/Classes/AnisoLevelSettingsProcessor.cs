/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Graphics/Aniso Level Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class AnisoLevelSettingsProcessor : SettingsProcessor
    {
        [System.Serializable]
        public class AnisotropicFilteringLevel
        {
            [SerializeField]
            [NotEmpty]
            private string name = "Level 1";

            [SerializeField]
            private AnisotropicFiltering state = AnisotropicFiltering.Enable;

            [SerializeField]
            [Slider(0, 16)]
            private int level = 16;

            public AnisotropicFilteringLevel(string name, AnisotropicFiltering state, int level)
            {
                this.name = name;
                this.state = state;
                this.level = level;
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

            public AnisotropicFiltering GetState()
            {
                return state;
            }

            public void SetState(AnisotropicFiltering value)
            {
                state = value;
            }

            public int GetLevel()
            {
                return level;
            }

            public void SetLevel(int value)
            {
                level = value;
            }
            #endregion
        }

        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        [NotEmpty]
        private string defaultLevel = DefaultLevels[3].GetName();

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetLevelLabel")]
        private AnisotropicFilteringLevel[] levels = DefaultLevels;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            dropdown.options.Clear();
            for (int i = 0; i < levels.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData(levels[i].GetName()));
            }
        }

        /// <summary>
        /// <br>Called when the settings manager saves the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Write</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to save processor value.</br>
        /// </summary>
        protected override object OnSave()
        {
            string level = dropdown.options[dropdown.value].text;
            for (int i = 0; i < levels.Length; i++)
            {
                AnisotropicFilteringLevel anisotropicFilteringLevel = levels[i];
                if (anisotropicFilteringLevel.GetName() == level)
                {
                    Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
                    QualitySettings.anisotropicFiltering = anisotropicFilteringLevel.GetState();
                    for (int j = 0; j < textures.Length; j++)
                    {
                        textures[j].anisoLevel = anisotropicFilteringLevel.GetLevel();
                    }
                    textures = null;
                    break;
                }
            }
            return level;
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            string level = value.ToString();
            for (int i = 0; i < levels.Length; i++)
            {
                AnisotropicFilteringLevel anisotropicFilteringLevel = levels[i];
                if (anisotropicFilteringLevel.GetName() == level)
                {
                    Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
                    QualitySettings.anisotropicFiltering = anisotropicFilteringLevel.GetState();
                    for (int j = 0; j < textures.Length; j++)
                    {
                        textures[j].anisoLevel = anisotropicFilteringLevel.GetLevel();
                    }
                    textures = null;
                    break;
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
            return defaultLevel;
        }

        #region [Static Properties]
        public static AnisotropicFilteringLevel[] DefaultLevels
        {
            get
            {
                return new AnisotropicFilteringLevel[5]
                {
                    new AnisotropicFilteringLevel("Disabled", AnisotropicFiltering.Disable, 0),
                    new AnisotropicFilteringLevel("Low", AnisotropicFiltering.Enable, 3),
                    new AnisotropicFilteringLevel("Medium", AnisotropicFiltering.Enable, 7),
                    new AnisotropicFilteringLevel("High", AnisotropicFiltering.Enable, 16),
                    new AnisotropicFilteringLevel("Ultra", AnisotropicFiltering.ForceEnable, 16)
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