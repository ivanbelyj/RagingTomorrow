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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/Canvas Scale Settings Receiver")]
    [DisallowMultipleComponent]
    public sealed class CanvasScaleSettingsReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private CanvasScaler canvasScaler;

        [SerializeField]
        [Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the UI will be scaled up, and if it's smaller, the UI will be scaled down. This is done in accordance with the Screen Match Mode.")]
        private Vector2 referenceResolution;

        [SerializeField]
        [MinValue(0.1f)]
        private float amplifier = 0.35f;

        [SerializeField]
        private float defaultValue = 1.0f;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(canvasScaler != null, $"<b><color=#FF0000>Attach reference of the canvas scaler to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Canvas Scaler<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Load</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            float match = (float)value + amplifier;
            canvasScaler.referenceResolution = referenceResolution * match;
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