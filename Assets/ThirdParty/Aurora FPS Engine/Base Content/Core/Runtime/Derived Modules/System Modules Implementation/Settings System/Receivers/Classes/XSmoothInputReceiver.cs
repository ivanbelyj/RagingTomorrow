/* ================================================================
   ---------------------------------------------------
   Project   :    Aurora FPS Engine
   Publisher :    Renowned Games
   Author    :    Tamerlan Shakirov
   ---------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.Settings
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/X Input Smooth Settings Receiver")]
    [DisallowMultipleComponent]
    public sealed class XSmoothInputReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private FirstPersonCamera firstPersonCamera;

        [SerializeField]
        private bool invertValue = true;

        [SerializeField]
        [VisibleIf("invertValue")]
        [Indent]
        private float minValue = 0.0f;

        [SerializeField]
        [VisibleIf("invertValue")]
        [Indent]
        private float maxValue = 100.0f;

        [SerializeField]
        [MinValue(0.0f)]
        private float defaultValue = 0.0f;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(firstPersonCamera != null, $"<b><color=#FF0000>Attach reference of the first person camera to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> First Person Camera<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Note: Called only if selected stream <i>Load</i> option. Otherwise this callback will be ignored.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            Vector2 smooth = firstPersonCamera.GetRotationSmooth();
            float xSmooth = (float)value;
            if (invertValue)
            {
                float inversed = Mathf.InverseLerp(maxValue, minValue, xSmooth);
                xSmooth = Mathf.Lerp(minValue, maxValue, inversed);
            }
            smooth.x = xSmooth;
            firstPersonCamera.SetRotationSmooth(smooth);
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