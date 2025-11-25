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
using System;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Reload/Standard Reload System")]
    [DisallowMultipleComponent]
    public class WeaponStandardReloadSystem : WeaponReloadSystem
    {
        public enum ReloadType
        {
            /// <summary>
            /// Default calculate weapon bullet and clip.
            /// Saves the number of bullets in the rechargeable clip.
            /// </summary>
            Default,

            /// <summary>
            /// Realistic calculate weapon bullet and clip.
            /// Not saves the number of bullets in the rechargeable clip.
            /// </summary>
            Realistic
        }

        // Default reload properties.
        [SerializeField]
        [Order(-350)]
        private ReloadType reloadType = ReloadType.Default;

        [SerializeField]
        [Foldout("Time Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(100)]
        private float reloadClipTime = 3.0f;

        [SerializeField]
        [Foldout("Time Settings", Style = "Header")]
        [MinValue(0.0f)]
        [Order(101)]
        private float reloadTime = 4.0f;

        // Stored required properties.
        private CoroutineObject reloadCoroutine;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            reloadCoroutine = new CoroutineObject(this);
        }

        /// <summary>
        /// Implementation of standard weapon reload logic.
        /// </summary>
        protected override void MakeReload()
        {
            reloadCoroutine.Start(ReloadProcessing);
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            reloadCoroutine.Stop();
        }

        /// <summary>
        /// Implementation of standard weapon reload logic with time delay.
        /// </summary>
        protected virtual IEnumerator ReloadProcessing()
        {
            OnStartReloadCallback?.Invoke();

            if (GetAmmoCount() > 0)
            {
                OnReloadClipCallback?.Invoke();
                yield return new WaitForSeconds(reloadClipTime);
            }
            else
            {
                OnReloadCallback?.Invoke();
                yield return new WaitForSeconds(reloadTime);
            }

            switch (reloadType)
            {
                case ReloadType.Default:
                    DefaultCalculate();
                    break;
                case ReloadType.Realistic:
                    RealisticCalculate();
                    break;
            }

            OnEndReloadCallback?.Invoke();
        }

        #region [Event Callback Functions]
        /// <summary>
        /// Called every time when weapon start reloading.
        /// </summary>
        public override event Action OnStartReloadCallback;

        /// <summary>
        /// Called every time when weapon end of reloading.
        /// </summary>
        public override event Action OnEndReloadCallback;

        /// <summary>
        /// Called when weapon start reloading clip(means that rechargeable clip isn't empty).
        /// </summary>
        public override event Action OnReloadClipCallback;

        /// <summary>
        /// Called when weapon start full reloading (means that rechargeable clip is empty).
        /// </summary>
        public override event Action OnReloadCallback;
        #endregion

        #region [Getter / Setter]
        public ReloadType GetReloadType()
        {
            return reloadType;
        }

        public void SetReloadType(ReloadType value)
        {
            reloadType = value;
        }

        public float GetBaseReloadTime()
        {
            return reloadClipTime;
        }

        public void SetBaseReloadTime(float value)
        {
            reloadClipTime = value;
        }

        public float GetFullReloadTime()
        {
            return reloadTime;
        }

        public void SetFullReloadTime(float value)
        {
            reloadTime = value;
        }
        #endregion
    }
}