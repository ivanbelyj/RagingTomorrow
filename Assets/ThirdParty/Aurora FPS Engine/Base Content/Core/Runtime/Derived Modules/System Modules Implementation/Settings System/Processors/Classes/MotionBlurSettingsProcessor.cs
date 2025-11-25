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
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Post Processing/Motion Blur Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class MotionBlurSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        [SerializeField]
        [VisibleIf("toggle", "NotNull")]
        [Indent(1)]
        private string toggleOn = "On";

        [SerializeField]
        [VisibleIf("toggle", "NotNull")]
        [Indent(1)]
        private string toggleOff = "Off";

        [SerializeField]
        private bool defaultValue = true;

        [SerializeField]
        [ReorderableList]
        private PostProcessProfile[] profiles;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");
            Debug.Assert(profiles != null, $"<b><color=#FF0000>Attach reference of the post process profile to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Profiles<i>(field)</i>.</color></b>");
            Debug.Assert(profiles.Length != 0, $"<b><color=#FF0000>Attach reference of the post process profile to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Profiles<i>(field)</i>.</color></b>");


            dropdown.options.Clear();
            dropdown.options.Add(new Dropdown.OptionData(toggleOn));
            dropdown.options.Add(new Dropdown.OptionData(toggleOff));
        }

        /// <summary>
        /// <br>Called when the settings manager saves the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Write</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to save processor value.</br>
        /// </summary>
        protected override object OnSave()
        {
            string active = dropdown.options[dropdown.value].text;
            if (profiles != null && profiles.Length > 0)
            {
                for (int i = 0; i < profiles.Length; i++)
                {
                    PostProcessProfile profile = profiles[i];
                    if (profile.TryGetSettings(out MotionBlur motionBlur))
                    {
                        motionBlur.active = active == toggleOn;
                    }
                }
            }
            return active;
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            string active = value.ToString();
            if (profiles != null && profiles.Length > 0)
            {
                for (int i = 0; i < profiles.Length; i++)
                {
                    PostProcessProfile profile = profiles[i];
                    if (profile.TryGetSettings(out MotionBlur motionBlur))
                    {
                        motionBlur.active = active == toggleOn;
                    }
                }
            }
            dropdown.value = dropdown.options.FindIndex(t => t.text == active);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return defaultValue ? toggleOn : toggleOff;
        }
    }
}