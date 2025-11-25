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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Graphics/Texture Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class TexturesSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        private string defaultValue = "High";

        [SerializeField]
        [VisibleIf("levels", "NotNull")]
        [Indent(1)]
        private string level1 = "Low";

        [SerializeField]
        [VisibleIf("levels", "NotNull")]
        [Indent(1)]
        private string level2 = "Medium";

        [SerializeField]
        [VisibleIf("levels", "NotNull")]
        [Indent(1)]
        private string level3 = "High";

        [SerializeField]
        [VisibleIf("levels", "NotNull")]
        [Indent(1)]
        private string level4 = "Ultra";

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");

            dropdown.options.Clear();
            dropdown.options.Add(new Dropdown.OptionData(level1));
            dropdown.options.Add(new Dropdown.OptionData(level2));
            dropdown.options.Add(new Dropdown.OptionData(level3));
            dropdown.options.Add(new Dropdown.OptionData(level4));
        }

        /// <summary>
        /// Save new textures resolution value
        /// </summary>
        /// <returns>New textures resolution</returns>
        protected override object OnSave()
        {
            string level = dropdown.options[dropdown.value].text;
#if UNITY_2022_2_OR_NEWER
            if(level == level1)
                QualitySettings.globalTextureMipmapLimit = 3;
            else if(level == level2)
                QualitySettings.globalTextureMipmapLimit = 2;
            else if (level == level3)
                QualitySettings.globalTextureMipmapLimit = 1;
            else if (level == level4)
                QualitySettings.globalTextureMipmapLimit = 0;
#else
            if(level == level1)
                QualitySettings.masterTextureLimit = 3;
            else if(level == level2)
                QualitySettings.masterTextureLimit = 2;
            else if (level == level3)
                QualitySettings.masterTextureLimit = 1;
            else if (level == level4)
                QualitySettings.masterTextureLimit = 0;
#endif
            return level;
        }

        /// <summary>
        /// Load textures resolution
        /// </summary>
        /// <param name="value">Textures resolution as object</param>
        protected override void OnLoad(object value)
        {
            string level = value.ToString();
#if UNITY_2022_2_OR_NEWER

            if (level == level1)
                QualitySettings.globalTextureMipmapLimit = 3;
            else if (level == level2)
                QualitySettings.globalTextureMipmapLimit = 2;
            else if (level == level3)
                QualitySettings.globalTextureMipmapLimit = 1;
            else if (level == level4)
                QualitySettings.globalTextureMipmapLimit = 0;
#else
            if (level == level1)
                QualitySettings.masterTextureLimit = 3;
            else if (level == level2)
                QualitySettings.masterTextureLimit = 2;
            else if (level == level3)
                QualitySettings.masterTextureLimit = 1;
            else if (level == level4)
                QualitySettings.masterTextureLimit = 0;
#endif
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
    }
}
