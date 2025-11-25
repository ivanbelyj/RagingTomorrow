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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Processors/Post Processing/Antialiasing Settings Processor")]
    [DisallowMultipleComponent]
    public sealed class AntialiasingSettingsProcessor : SettingsProcessor
    {
        [SerializeField]
        [NotNull]
        private Dropdown dropdown;

        private PostProcessLayer[] layers;

        /// <summary>
        /// Called when the script instance is being loaded
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(dropdown != null, $"<b><color=#FF0000>Attach reference of the UI Dropdown element to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Dropdown<i>(field)</i>.</color></b>");

#if UNITY_2022_2_OR_NEWER
            layers = FindObjectsByType<PostProcessLayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            layers = GameObject.FindObjectsOfType<PostProcessLayer>();
#endif
            dropdown.options.Clear();
            dropdown.options.Add(new Dropdown.OptionData("Disabled"));
            dropdown.options.Add(new Dropdown.OptionData("FXAA"));
            dropdown.options.Add(new Dropdown.OptionData("SMAA"));
            dropdown.options.Add(new Dropdown.OptionData("TAA"));
        }

        /// <summary>
        /// <br>Called when the settings manager saves the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Write</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to save processor value.</br>
        /// </summary>
        protected override object OnSave()
        {
            string method = dropdown.options[dropdown.value].text;
            if (layers != null && layers.Length > 0)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    switch (method)
                    {
                        case "Disabled":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.None;
                            break;
                        case "FXAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                            break;
                        case "SMAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                            break;
                        case "TAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                            break;
                    }
                }
            }
            return method;
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            string method = value.ToString();
            if (layers != null && layers.Length > 0)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    switch (method)
                    {
                        case "Disabled":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.None;
                            break;
                        case "FXAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                            break;
                        case "SMAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                            break;
                        case "TAA":
                            layers[i].antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                            break;
                    }
                }
            }
            dropdown.value = dropdown.options.FindIndex(t => t.text == method);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return "FXAA";
        }
    }
}