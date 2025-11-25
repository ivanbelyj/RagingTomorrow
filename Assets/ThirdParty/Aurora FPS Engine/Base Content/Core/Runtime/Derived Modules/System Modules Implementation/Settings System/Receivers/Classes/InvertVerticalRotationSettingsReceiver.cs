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
    [AddComponentMenu("Aurora FPS Engine/System Modules/Settings System/Receivers/Invert Vertical Rotation Settings Receiver")]
    [DisallowMultipleComponent]
    public sealed class InvertVerticalRotationSettingsReceiver : SettingsReceiver
    {
        [SerializeField]
        [NotNull]
        private PawnCamera pawnCamera;

        [SerializeField]
        private bool defaultValue = false;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(pawnCamera != null, $"<b><color=#FF0000>Attach reference of the player camera to {gameObject.name}<i>(gameobject)</i> -> {GetType().Name}<i>(component)</i> -> Pawn Camera<i>(field)</i>.</color></b>");
        }

        /// <summary>
        /// <br>Called when the settings manager load the settings file.</br>
        /// <br>Implement this method to load processor value.</br>
        /// </summary>
        protected override void OnLoad(object value)
        {
            bool invert = (bool)value;
            pawnCamera.InvertRotation(pawnCamera.IsHorizontalRotationInverted(), invert);
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