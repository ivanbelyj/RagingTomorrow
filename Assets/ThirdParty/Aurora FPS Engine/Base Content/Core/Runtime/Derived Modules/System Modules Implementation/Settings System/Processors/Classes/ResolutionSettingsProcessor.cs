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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/Resolution Settings Processor")]
    [DisallowMultipleComponent]
    [Obsolete]
    public sealed class ResolutionSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        private bool filterRefreshRate = true;

        [SerializeField]
        private Vector2 minResolution = new Vector2(1280, 720);

        [SerializeField]
        private Vector2 maxResolution = Vector2.zero;

        [SerializeField]
        [ReorderableList]
        private Vector2[] exceptResolutions;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");

            dropdown.options.Clear();
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                Resolution resolution = Screen.resolutions[i];
                if ((!filterRefreshRate || resolution.refreshRate == Screen.currentResolution.refreshRate) &&
                     (resolution.width >= minResolution.x && resolution.height >= minResolution.y) &&
                     (maxResolution == Vector2.zero || (resolution.width <= maxResolution.x && resolution.height <= maxResolution.y)) &&
                     !exceptResolutions.Any(t => t == new Vector2(resolution.width, resolution.height)))
                {
                    string option = string.Format("{0}x{1}", resolution.width, resolution.height);
                    dropdown.options.Add(new Dropdown.OptionData(option));
                }
            }
        }

        /// <summary>
        /// Save new screen resolution value
        /// </summary>
        /// <returns>New screen resolution</returns>
        protected override object OnSave()
        {
            string option = dropdown.options[dropdown.value].text;
            string[] resolution = option.Split('x');
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreen);
            return option;
        }

        /// <summary>
        /// Load screen resolution
        /// </summary>
        /// <param name="value">Screen resolution as object</param>
        protected override void OnLoad(object value)
        {
            string option = value.ToString();
            string[] resolution = option.Split('x');
            Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), Screen.fullScreen);
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
            Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
            return string.Format("{0}x{1}", resolution.width, resolution.height);
        }
    }
}
