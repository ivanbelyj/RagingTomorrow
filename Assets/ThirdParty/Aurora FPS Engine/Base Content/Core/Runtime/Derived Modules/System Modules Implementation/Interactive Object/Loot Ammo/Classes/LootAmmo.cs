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
using AuroraFPSRuntime.SystemModules.InventoryModules;
using AuroraFPSRuntime.WeaponModules;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Looting/Loot Ammo")]
    [DisallowMultipleComponent]
    public class LootAmmo : LootObject
    {
        [SerializeField]
        [NotNull]
        private EquippableItem weaponItem;

        [SerializeField]
        [MinValue(1)]
        private int count = 1;

        [SerializeField]
        private bool leaveUnusedPart = false;

        /// <summary>
        /// Called when character being loot object.
        /// </summary>
        /// <param name="other">Reference of loot object transform.</param>
        /// <returns>The success of looting the specified object.</returns>
        protected override bool OnLoot(Transform other)
        {
            int previousCount = count;
            InventorySystem inventorySystem = other.GetComponent<InventorySystem>();
            if (inventorySystem != null && inventorySystem.TryGetItemTransform(weaponItem, out Transform weapon))
            {
                AmmoSystem ammoSystem = weapon.GetComponent<AmmoSystem>();
                if (ammoSystem != null)
                {
                    int requiredCount = 0;
                    WeaponReloadSystem reloadSystem = ammoSystem as WeaponReloadSystem;
                    if (reloadSystem != null)
                    {
                        requiredCount = reloadSystem.GetMaxClipCount() - reloadSystem.GetClipCount();
                        if (count >= requiredCount)
                        {
                            count -= requiredCount;
                            reloadSystem.SetClipCount(reloadSystem.GetMaxClipCount());
                        }
                        else
                        {
                            reloadSystem.SetClipCount(reloadSystem.GetClipCount() + count);
                            count = 0;
                        }
                    }
                    else
                    {
                        requiredCount = ammoSystem.GetMaxAmmoCount() - ammoSystem.GetAmmoCount();
                        if (count >= requiredCount)
                        {
                            count -= requiredCount;
                            ammoSystem.SetAmmoCount(ammoSystem.GetMaxAmmoCount());
                        }
                        else
                        {
                            ammoSystem.SetAmmoCount(ammoSystem.GetAmmoCount() + count);
                            count = 0;
                        }
                    }
                }
            }

            if(count == 0 || (count != previousCount && !leaveUnusedPart))
            {
                return true;
            }
            else if(count != previousCount && leaveUnusedPart)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void CalculateMessageCode(Transform other, out int messageCode)
        {
            if (count > 0)
            {
                InventorySystem inventorySystem = other.GetComponent<InventorySystem>();
                if (inventorySystem != null && inventorySystem.TryGetItemTransform(weaponItem, out Transform weapon))
                {
                    AmmoSystem ammoSystem = weapon.GetComponent<AmmoSystem>();
                    if (ammoSystem != null)
                    {
                        WeaponReloadSystem reloadSystem = ammoSystem as WeaponReloadSystem;
                        if ((reloadSystem != null && reloadSystem.GetClipCount() < reloadSystem.GetMaxClipCount())
                            || ammoSystem.GetAmmoCount() < ammoSystem.GetMaxAmmoCount())
                        {
                            messageCode = 1;
                            return;
                        }
                    }
                }
            }
            messageCode = 0;
        }

        #region [Getter / Setter]
        public EquippableItem GetWeaponItem()
        {
            return weaponItem;
        }

        public void SetWeaponItem(EquippableItem value)
        {
            weaponItem = value;
        }

        public int GetCount()
        {
            return count;
        }

        public void SetCount(int value)
        {
            count = value;
        }

        public bool GetLeaveUnusedPart()
        {
            return leaveUnusedPart;
        }

        public void SetLeaveUnusedPart(bool value)
        {
            leaveUnusedPart = value;
        }
        #endregion
    }
}
