/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.WeaponModules
{
    public abstract class WeaponReloadSystem : AmmoSystem, IReloadSystem, IClipCount, IInputListener
    {
        [SerializeField]
        [Slider(0, "maxClipCount")]
        [Order(-297)]
        private int clipCount = 256;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0)]
        [Order(351)]
        private int maxClipCount = 512;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(352)]
        private bool unlimitedClip = false;

        [SerializeField]
        [Tooltip("Reload available only when ammo count less then max ammo count.")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(353)]
        private bool smartReloading = true;

        [SerializeField]
        [Tooltip("Automatically reload weapon when ammo count is zero.")]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(354)]
        private bool autoReloading = false;

        [SerializeField]
        [Label("Delay")]
        [Tooltip("Delay before start auto reloading.")]
        [Foldout("Advanced Settings", Style = "Header")]
        [VisibleIf("autoReloading")]
        [MinValue(0.0f)]
        [Indent(1)]
        [Order(355)]
        private float autoReloadingDelay = 0.5f;

        // Stored required components.
        private PlayerController controller;

        // Stored required properties.
        private bool isReloading;
        private float autoReloadingDelayTime;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            controller = GetComponentInParent<PlayerController>();
            
            OnStartReloadCallback += () => isReloading = true;
            OnEndReloadCallback += () => isReloading = false;
        }

        /// <summary>
        /// Called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected virtual void Start()
        {
            PlayerCamera cameraControl = controller.GetPlayerCamera();
            OnStartReloadCallback += () =>
            {
                cameraControl.ZoomOut();
            };
        }

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            PlayerCamera cameraControl = controller.GetPlayerCamera();
            cameraControl.ZoomConditionCallback += CanZooming;
            RegisterInputActions();
        }

        /// <summary>
        /// Called every frame, if the Behaviour is enabled.
        /// </summary>
        protected virtual void LateUpdate()
        {
            if (CheckAutoReloading())
            {
                MakeReload();
            }
        }

        /// <summary>
        /// Called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            isReloading = false;

            PlayerCamera cameraControl = controller.GetPlayerCamera();
            cameraControl.ZoomConditionCallback -= CanZooming;

            RemoveInputActions();
        }

        /// <summary>
        /// Implement this method to make logic of weapon reload.
        /// </summary>
        protected abstract void MakeReload();

        /// <summary>
        /// Default calculate weapon bullet and clip.
        /// Saves the number of bullets in the rechargeable clip.
        /// </summary>
        protected void DefaultCalculate()
        {
            if (unlimitedClip || WeaponUtilities.UnlimitedClip)
            {
                SetAmmoCount(GetMaxAmmoCount());
            }
            else
            {
                if (clipCount > 0)
                {
                    int requiredAmmoCount = GetMaxAmmoCount() - GetAmmoCount();
                    if (clipCount >= requiredAmmoCount)
                    {
                        SetAmmoCount(GetMaxAmmoCount());
                        clipCount -= requiredAmmoCount;
                    }
                    else
                    {
                        SetAmmoCount(GetAmmoCount() + clipCount);
                        clipCount = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Realistic calculate weapon bullet and clip.
        /// Not saves the number of bullets in the rechargeable clip.
        /// </summary>
        protected void RealisticCalculate()
        {
            if (unlimitedClip || WeaponUtilities.UnlimitedClip)
            {
                SetAmmoCount(GetMaxAmmoCount());
            }
            else
            {
                bool conditions = clipCount >= GetMaxAmmoCount();
                SetAmmoCount(conditions ? GetMaxAmmoCount() : clipCount + GetAmmoCount());
                clipCount = conditions ? clipCount - GetMaxAmmoCount() : 0;
            }
        }

        protected bool CheckSmartReloading()
        {
            if(smartReloading)
            {
                return GetAmmoCount() < GetMaxAmmoCount();
            }
            return true;
        }

        protected bool CheckAutoReloading()
        {
            if (GetAmmoCount() == 0 && autoReloadingDelayTime == 0)
            {
                autoReloadingDelayTime = Time.time;
            }

            if((GetAmmoCount() > 0 || isReloading) && autoReloadingDelayTime > 0)
            {
                autoReloadingDelayTime = 0;
            }

            if (autoReloadingDelayTime > 0 && GetAmmoCount() == 0 && autoReloading && (Time.time - autoReloadingDelayTime >= autoReloadingDelay))
            {
                autoReloadingDelayTime = 0;
                return true;
            }
            return false;
        }

        public virtual void RegisterInputActions()
        {
            InputReceiver.ReloadAction.performed += OnReloadAction;
        }

        public virtual void RemoveInputActions()
        {
            InputReceiver.ReloadAction.performed -= OnReloadAction;
        }

        #region [IReloadSystem Implementation]
        public bool IsReloading()
        {
            return isReloading;
        }

        public virtual bool AddClip(int count)
        {
            int newCount = clipCount + count;
            if(newCount <= maxClipCount)
            {
                clipCount = newCount;
                return true;
            }
            return false;
        }

        public virtual bool RemoveClip(int count)
        {
            if (unlimitedClip || WeaponUtilities.UnlimitedClip)
            {
                return true;
            }

            int newCount = clipCount - count;
            if(newCount >= 0)
            {
                clipCount = newCount;
                return true;
            }
            return false;
        }
        #endregion

        #region [Input Action Wrapper]
        private void OnReloadAction(InputAction.CallbackContext context)
        {
            if (!isReloading && clipCount > 0 && CheckSmartReloading())
            {
                MakeReload();
            }
        }
        #endregion

        #region [Camera Callback Wrapper]
        private bool CanZooming()
        {
            return !isReloading;
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called every time when weapon start reloading.
        /// </summary>
        /// <param name="float">Time to reload.</param>
        public abstract event Action OnStartReloadCallback;

        /// <summary>
        /// Called every time when weapon end of reloading.
        /// </summary>
        public abstract event Action OnEndReloadCallback;

        /// <summary>
        /// Called when weapon start base reloading (rechargeable clip isn't empty).
        /// </summary>
        public abstract event Action OnReloadClipCallback;

        /// <summary>
        /// Called when weapon start full reloading (rechargeable clip is empty).
        /// </summary>
        public abstract event Action OnReloadCallback;
        #endregion

        #region [Getter / Setter]
        protected void IsReloading(bool value)
        {
            isReloading = value;
        }

        public int GetClipCount()
        {
            return clipCount;
        }

        public void SetClipCount(int count)
        {
            if(count >= 0)
            {
                clipCount = Math.Min(count, maxClipCount);
            }
        }

        public int GetMaxClipCount()
        {
            return maxClipCount;
        }

        public void SetMaxClipCount(int count)
        {
            maxClipCount = Math.Max(0, count);
        }

        public bool UnlimitedClip()
        {
            return unlimitedClip;
        }

        public void UnlimitedClip(bool value)
        {
            unlimitedClip = value;
        }

        public bool SmartReloading()
        {
            return smartReloading;
        }

        public void SmartReloading(bool value)
        {
            smartReloading = value;
        }

        public bool AutoReloading()
        {
            return autoReloading;
        }

        public void AutoReloading(bool value)
        {
            autoReloading = value;
        }
        #endregion
    }
}