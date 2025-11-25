/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov, Alexandra Averyanova
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/Display Refresh Rate Settings Processor")]
    [DisallowMultipleComponent]
    [Obsolete]
    public sealed class DisplayRefreshRateSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            ForceUpdateRefreshRates();
            SettingsSystem.OnSaveCallback += (section) => ForceUpdateRefreshRates();
        }

        /// <summary>
        /// Save new screen resolution value
        /// </summary>
        /// <returns>New screen resolution</returns>
        protected override object OnSave()
        {
            string option = dropdown.options[dropdown.value].text;
            Resolution current = Screen.currentResolution;
            Screen.SetResolution(current.width, current.height, Screen.fullScreenMode, int.Parse(option));
            return option;
        }

        /// <summary>
        /// Load screen resolution
        /// </summary>
        /// <param name="value">Screen resolution as object</param>
        protected override void OnLoad(object value)
        {
            string option = (string)value;
            Resolution current = Screen.currentResolution;
            Screen.SetResolution(current.width, current.height, Screen.fullScreenMode, int.Parse(option));
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
            return Screen.currentResolution.refreshRate;
        }

        /// <summary>
        /// Forcedly update dropdown options of refresh rates relative current resolution.
        /// </summary>
        public void ForceUpdateRefreshRates()
        {
            dropdown.options.Clear();
            Resolution current = Screen.currentResolution;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                if (current.width == resolution.width && current.height == resolution.height)
                {
                    dropdown.options.Add(new Dropdown.OptionData(resolution.refreshRate.ToString()));
                }
            }
        }
    }
}
