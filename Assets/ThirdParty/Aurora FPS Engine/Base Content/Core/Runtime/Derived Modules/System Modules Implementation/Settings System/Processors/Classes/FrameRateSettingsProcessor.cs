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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Display/Frame-rate Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class FrameRateSettingsProcessor : SettingsProcessor
    {
        private int defaultValue = 60;

        [SerializeField]
        [NotNull]
        private InputField inputField;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(inputField != null, $"<b><color=#FF0000>Attach reference of the UI InputField element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Input Field<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// Save new frame-rate value
        /// </summary>
        /// <returns>New frame-rate value</returns>
        protected override object OnSave()
        {
            int targetFrameRate = System.Convert.ToInt32(inputField.text);
            Application.targetFrameRate = targetFrameRate;
            return targetFrameRate;
        }

        /// <summary>
        /// Load frame-rate value
        /// </summary>
        /// <param name="value">frame-rate value</param>
        protected override void OnLoad(object value)
        {
            int targetFrameRate = System.Convert.ToInt32(value);
            Application.targetFrameRate = targetFrameRate;
            inputField.text = targetFrameRate.ToString();
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
