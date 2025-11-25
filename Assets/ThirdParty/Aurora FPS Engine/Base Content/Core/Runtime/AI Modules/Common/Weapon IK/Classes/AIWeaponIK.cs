/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/AI Modules/AI Weapon IK")]
    [DisallowMultipleComponent]
    public sealed class AIWeaponIK : WeaponIK
    {
        [System.Serializable]
        private sealed class WeaponDictionary : SerializableDictionary<string, WeaponSettings>
        {
            [SerializeField]
            private string[] keys;

            [SerializeField]
            private WeaponSettings[] values;

            protected override string[] GetKeys()
            {
                return keys;
            }

            protected override WeaponSettings[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(string[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(WeaponSettings[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        private WeaponDictionary weaponDictionary;

        [SerializeField]
        private string startWeapon;

        // Stored required properties.
        private string selectedWeapon = string.Empty;

        private void Start()
        {
            ShowWeapon(startWeapon);
        }

        protected override void HandIK()
        {
            if (weaponDictionary.TryGetValue(selectedWeapon, out WeaponSettings settings))
            {
                base.HandIKHandle(settings);
            }
        }

        #region [Active/Disactive Weapon Methods]
        private void ShowWeapon(string weaponName)
        {
            if (weaponDictionary.TryGetValue(weaponName, out WeaponSettings settings))
            {
                ActiveWeapon(settings);
                selectedWeapon = weaponName;
            }
        }

        private void HideWeapon(string weaponName)
        {
            if (weaponDictionary.TryGetValue(weaponName, out WeaponSettings settings))
            {
                DisableWeapon(settings);
                selectedWeapon = string.Empty;
            }
        }

        protected override void DiactivateAllWeapons()
        {
            foreach (KeyValuePair<string, WeaponSettings> item in weaponDictionary)
            {
                DisableWeapon(item.Value);
            }

            selectedWeapon = string.Empty;
        }
        #endregion
    }
}