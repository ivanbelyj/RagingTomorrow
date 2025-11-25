/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Pattern;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    /// <summary>
    /// Camera shaker component registers new shakes, holds a list of active shakes, and applies them to the camera additively.
    /// </summary>
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Camera/Camera Shake/Camera Shaker")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraShaker : MonoBehaviour
    {
        private readonly List<ICameraShake> activeShakes = new List<ICameraShake>();

        [SerializeField]
        [Tooltip("Scales the strength of all shakes.")]
        [Slider(0, 1)]
        private float strengthMultiplier = 1.0f;

        /// <summary>
        /// Adds a shake to the list of active shakes.
        /// </summary>
        public void RegisterShake(ICameraShake shake)
        {
            shake.Initialize(transform.position, transform.rotation);
            activeShakes.Add(shake);
        }

        private void Update()
        {
            Displacement cameraDisplacement = Displacement.zero;
            for (int i = activeShakes.Count - 1; i >= 0; i--)
            {
                if (activeShakes[i].IsFinished())
                {
                    activeShakes.RemoveAt(i);
                }
                else
                {
                    activeShakes[i].Update(Time.deltaTime, transform.position, transform.rotation);
                    cameraDisplacement += activeShakes[i].GetCurrentDisplacement();
                }
            }
            transform.localPosition = strengthMultiplier * cameraDisplacement.GetPosition();
            transform.localRotation = Quaternion.Euler(strengthMultiplier * cameraDisplacement.GetEulerAngles());
        }

        public void Clear()
        {
            activeShakes.Clear();
        }
    }
}
