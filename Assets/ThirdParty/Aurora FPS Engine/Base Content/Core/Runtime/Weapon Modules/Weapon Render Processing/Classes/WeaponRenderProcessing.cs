/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Render/Weapon Render Processing")]
    [DisallowMultipleComponent]
    internal sealed class WeaponRenderProcessing : MonoBehaviour
    {
        [SerializeField]
        [NotNull]
        private Transform mainCamera;

        [SerializeField]
        [NotNull]
        private Transform weaponHinge;

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            weaponHinge.localPosition = -mainCamera.localPosition;
            weaponHinge.localEulerAngles = -mainCamera.localEulerAngles;
        }
    }
}