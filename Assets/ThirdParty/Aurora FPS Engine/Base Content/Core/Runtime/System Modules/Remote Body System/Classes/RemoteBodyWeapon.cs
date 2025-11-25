/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using AuroraFPSRuntime.SystemModules.InventoryModules;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Remote Body/Remote Body Weapon")]
    [DisallowMultipleComponent]
    public sealed class RemoteBodyWeapon : WeaponIK
    {
        [System.Serializable]
        private sealed class WeaponDictionary : SerializableDictionary<EquippableItem, WeaponSettings>
        {
            [SerializeField]
            private EquippableItem[] keys;

            [SerializeField]
            private WeaponSettings[] values;

            protected override EquippableItem[] GetKeys()
            {
                return keys;
            }

            protected override WeaponSettings[] GetValues()
            {
                return values;
            }

            protected override void SetKeys(EquippableItem[] keys)
            {
                this.keys = keys;
            }

            protected override void SetValues(WeaponSettings[] values)
            {
                this.values = values;
            }
        }

        [SerializeField]
        [NotNull]
        private InventorySystem inventorySystem;

        [SerializeField]
        private WeaponDictionary weaponDictionary;

        private void OnEnable()
        {
            inventorySystem.OnEquipStartedCallback += OnEquipStartedCallback;
            inventorySystem.OnHideCallback += OnHideCallback;
            inventorySystem.OnTossCallback += OnTossCallback;
        }

        private void Start()
        {
            DiactivateAllWeapons();
        }

        private void OnDisable()
        {
            inventorySystem.OnEquipStartedCallback -= OnEquipStartedCallback;
            inventorySystem.OnHideCallback -= OnHideCallback;
            inventorySystem.OnTossCallback -= OnTossCallback;
        }

        protected override void HandIK()
        {
            Transform equippedTrasform = inventorySystem.GetEquippedTransform();
            EquippableItem equippableItem = inventorySystem.GetEquippedItem();
            if (equippedTrasform != null && equippableItem != null)
            {
                if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
                {
                    base.HandIKHandle(settings);
                }
            }
        }

        #region [Active/Disactive Weapon Methods]
        private void OnEquipStartedCallback(EquippableItem equippableItem)
        {
            if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
            {
                ActiveWeapon(settings);
            }
        }

        private void OnHideCallback()
        {
            if (inventorySystem.GetEquippedItem() is EquippableItem equippableItem)
            {
                if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
                {
                    DisableWeapon(settings);
                }
            }
        }

        private void OnTossCallback(InventoryItem inventoryItem, Transform item)
        {
            if (inventoryItem is EquippableItem equippableItem)
            {
                if (weaponDictionary.TryGetValue(equippableItem, out WeaponSettings settings))
                {
                    DisableWeapon(settings);
                }
            }
        }

        protected override void DiactivateAllWeapons()
        {
            foreach (KeyValuePair<EquippableItem, WeaponSettings> item in weaponDictionary)
            {
                DisableWeapon(item.Value);
            }
        }
        #endregion
    }
}