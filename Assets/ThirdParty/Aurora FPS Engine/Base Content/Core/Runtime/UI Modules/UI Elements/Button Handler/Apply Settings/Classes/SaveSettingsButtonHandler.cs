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
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Buttons/Save Setting Handler")]
    [DisallowMultipleComponent]
    public sealed class SaveSettingsButtonHandler : MonoBehaviour
    {
        /// <summary>
        /// Save specified section of settings.
        /// </summary>
        public void SaveSection(string section)
        {
            SettingsSystem.Save(section);
        }

        /// <summary>
        /// Save all sections of settings.
        /// </summary>
        public void SaveAll()
        {
            SettingsSystem.SaveAll();
        }
    }
}
