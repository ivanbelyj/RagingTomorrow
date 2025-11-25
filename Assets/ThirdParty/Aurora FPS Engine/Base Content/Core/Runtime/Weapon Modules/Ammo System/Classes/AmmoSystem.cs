/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Ammo/Ammo System")]
    [DisallowMultipleComponent]
    public class AmmoSystem : MonoBehaviour, IAmmoSystem, IAmmunition
    {
        [SerializeField]
        [Slider(0, "maxAmmoCount")]
        [Order(-299)]
        private int ammoCount;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [MinValue(0)]
        [Order(300)]
        private int maxAmmoCount;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(301)]
        private bool unlimitedAmmo = false;

        #region [IAmmoSystem Implementation]
        public virtual bool AddAmmo(int count)
        {
            int newCount = ammoCount + count;
            if (newCount <= maxAmmoCount)
            {
                ammoCount = newCount;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAmmo(int count)
        {
            if (unlimitedAmmo || WeaponUtilities.UnlimitedAmmo)
            {
                return true;
            }

            int newCount = ammoCount - count;
            if(newCount >= 0)
            {
                ammoCount = newCount;
                return true;
            }
            return false;
        }
        #endregion

        #region [IAmmoCount Implementation]
        public int GetAmmoCount()
        {
            return ammoCount;
        }
        #endregion

        #region [Getter / Setter]
        public virtual void SetAmmoCount(int count)
        {
            if (count >= 0)
            {
                ammoCount = Math.Min(count, maxAmmoCount);
            }
        }

        public int GetMaxAmmoCount()
        {
            return maxAmmoCount;
        }

        public void SetMaxAmmoCount(int count)
        {
            maxAmmoCount = Math.Max(0, count);
            ammoCount = Mathf.Clamp(ammoCount, 0, maxAmmoCount);
        }

        public bool UnlimitedAmmo()
        {
            return unlimitedAmmo;
        }

        public void UnlimitedAmmo(bool value)
        {
            unlimitedAmmo = value;
        }
        #endregion
    }
}