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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/Global Volume Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class GlobalVolumeSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Slider slider;

        [SerializeField]
        [Slider(0, 1)]
        private float defaultValue = 1;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(slider != null, $"<b><color=#FF0000>Attach reference of the UI Slider element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Slider<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Save new frame-rate value
        /// </summary>
        /// <returns>New frame-rate value</returns>
        protected override object OnSave()
        {
            float volume = slider.value;
            AudioListener.volume = volume;
            return volume;
        }

        /// <summary>
        /// Load frame-rate value
        /// </summary>
        /// <param name="value">frame-rate value</param>
        protected override void OnLoad(object value)
        {
            float volume = System.Convert.ToSingle(value);
            AudioListener.volume = volume;
            slider.value = volume;
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
