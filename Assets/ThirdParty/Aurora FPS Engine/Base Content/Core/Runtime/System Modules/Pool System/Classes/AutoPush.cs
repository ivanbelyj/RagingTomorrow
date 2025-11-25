/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using UnityEngine;
using System.Collections;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Pool System/Auto Push")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PoolObject))]
    public sealed class AutoPush : MonoBehaviour
    {
        [SerializeField]
        [MinValue(0.01f)]
        private float delay;

        // Stored required components.
        private PoolObject poolObject;

        // Stored required properties.
        private CoroutineObject timerCoroutine;


        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            poolObject = GetComponent<PoolObject>();
            timerCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            timerCoroutine.Start(Timer, true);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            timerCoroutine.Stop();
        }

        /// <summary>
        /// Delay timer to add target object in pool.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(delay);
            poolObject.Push();
        }
    }
}
