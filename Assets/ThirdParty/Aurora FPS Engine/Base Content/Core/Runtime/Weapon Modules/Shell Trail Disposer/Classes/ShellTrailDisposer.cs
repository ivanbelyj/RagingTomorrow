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
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Other/Shell Trail Disposer")]
    [DisallowMultipleComponent]
    public sealed class ShellTrailDisposer : MonoBehaviour
    {
        // Stored required components.
        private TrailRenderer trail;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            trail = GetComponent<TrailRenderer>();
            if(trail != null)
            {
                PoolObject shell = GetComponentInParent<PoolObject>();
                shell.OnBeforePushCallback += DisposeTrail;
            }
        }

        /// <summary>
        /// Dispose trail renderer line.
        /// </summary>
        private void DisposeTrail()
        {
            trail.Clear();
        }
    }
}