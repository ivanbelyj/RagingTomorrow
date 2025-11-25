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

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/Audio Source/Volume Settings Receiver")]
    [DisallowMultipleComponent]
    public sealed class VolumeSettingsReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private AudioSource audioSource;

        [SerializeField]
        [Slider(0.0f, 1.0f)]
        private float defaultValue = 1.0f;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(audioSource != null, $"<b><color=#FF0000>Attach reference of the audio source to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Audio Source<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Load</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            audioSource.volume = (float)value;
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