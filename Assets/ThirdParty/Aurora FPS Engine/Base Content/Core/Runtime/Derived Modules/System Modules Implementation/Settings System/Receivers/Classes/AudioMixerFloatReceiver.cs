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
using UnityEngine.Audio;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/Audio Mixer/Audio Mixer Volume Receiver")]
    public sealed class AudioMixerFloatReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private AudioMixer mixer;

        [SerializeField]
        [NotEmpty]
        private string parameter;

        [SerializeField]
        [VisualClamp(0.001f, 1.0f)]
        private float defaultVolume = 1.0f;

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Read</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            const float MULTIPLIER = 30.0f;
            mixer.SetFloat(parameter, Mathf.Log10((float)value) * MULTIPLIER);
        }

        /// <summary>
        /// <br>Called when settings file is not found or 
        /// target processor GUID is not found in loaded buffer.</br>
        /// <br>Implement this method to determine default value for this processor.</br>
        /// </summary>
        /// <returns>Default value of processor.</returns>
        public override object GetDefaultValue()
        {
            return defaultVolume;
        }
    }
}