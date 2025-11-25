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
using AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory;
using AuroraFPSRuntime.CoreModules.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Inventory/Weapon Inventory System")]
    [DisallowMultipleComponent]
    public class WeaponInventorySystem : GroupInventorySystem
    {
        [Serializable]
        private class IgnoreGroups : SerializableHashSet<string> { }

        [Flags]
        public enum Options
        {
            None = 0,
            OnlyUniqueItem = 1 << 0,
            CreateAllEquppableItemsOnAwake = 1 << 2,
            DisposeEquppableItem = 1 << 3,
            Everything = ~0
        }

        [SerializeField]
        [Foldout("Weapon Settings", Style = "Header")]
        [Order(199)]
        private Options options = Options.OnlyUniqueItem;

        [SerializeField]
        [Label("Initialize On Awake")]
        [ReorderableList(ElementLabel = null)]
        [Foldout("Weapon Settings", Style = "Header")]
        [Order(200)]
        private EquippableItem[] initOnAwake;

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(997)]
        private bool scrollItems;

        [SerializeField]
        [Label("Sensitivity")]
        [VisualClamp(1, 0)]
        [VisibleIf("scrollItems", true)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Indent]
        [Order(998)]
        private float scrollSensitivity = 0.5f;

        [SerializeField]
        [VisibleIf("scrollItems", true)]
        [Foldout("Advanced Settings", Style = "Header")]
        [Indent]
        [Order(999)]
        private IgnoreGroups ignoreGroups;

        // Stored required properties.
        private List<string> inputList;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if ((options & Options.CreateAllEquppableItemsOnAwake) != 0)
            {
                foreach (InventoryItem item in Items)
                {
                    if (item is EquippableItem equippableItem)
                    {
                        InstantiateItemObject(equippableItem);
                    }
                }
            }
            else
            {
                for (int i = 0; i < initOnAwake.Length; i++)
                {
                    EquippableItem equippableItem = initOnAwake[i];
                    if (equippableItem != null)
                    {
                        InstantiateItemObject(equippableItem);
                    }
                }
            }

            inputList = new List<string>();
            foreach (string input in Inputs)
            {
                inputList.Add(input);
            }
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            if (scrollItems)
            {
                OnScrollItem();
            }
        }

        /// <summary>
        /// Add new item in inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public override bool AddItem(InventoryItem item)
        {
            bool uniqueItemOnly = (options & Options.OnlyUniqueItem) != 0;
            if (!uniqueItemOnly || (uniqueItemOnly && !ContainsItem(item)))
            {
                if (base.AddItem(item))
                {
                    OnAddedItemCallback?.Invoke(item);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public override bool RemoveItem(InventoryItem item)
        {
            if ((options & Options.DisposeEquppableItem) != 0)
            {
                EquippableItem equippableItem = item as EquippableItem;
                if (TryGetItemTransform(equippableItem, out Transform equippableTransform))
                {
                    Destroy(equippableTransform.gameObject);
                    RemoveItemHash(equippableItem);
                }
            }
            if (base.RemoveItem(item))
            {
                OnRemovedItemCallback?.Invoke(item);
                return true;
            }
            return false;
        }

        #region [Scroll Items Implementation]
        /// <summary>
        /// Processing scroll selecting item type.
        /// </summary>
        protected virtual void OnScrollItem()
        {
            float axisValue = InputReceiver.ScrollItemsAction.ReadValue<float>();
            if (axisValue != 0)
            {
                if (inputList.Count <= 1)
                {
                    if (GetEquippedItem() == null)
                    {
                        if (inputEventCache.TryGetValue(inputList[0], out Action<UnityEngine.InputSystem.InputAction.CallbackContext> action))
                        {
                            action?.Invoke(new UnityEngine.InputSystem.InputAction.CallbackContext());
                        }
                    }
                }
                else
                {
                    int nextIndex = 0;
                    for (int i = 0; i < inputList.Count; i++)
                    {
                        string key = inputList[i];
                        if (key == GetLastUsedInputKey())
                        {
                            nextIndex = i;
                        }
                    }

                    float possitiveAxisValue = Mathf.Abs(axisValue);
                    if (possitiveAxisValue > 0 && possitiveAxisValue > scrollSensitivity)
                    {
                        int storedOriginalIndex = nextIndex;
                        while (true)
                        {
                            nextIndex = axisValue > 0 ? nextIndex + 1 : nextIndex - 1;
                            nextIndex = CoreModules.Mathematics.Math.Loop(nextIndex, 0, inputList.Count - 1);
                            string key = inputList[nextIndex];
                            if (nextIndex == storedOriginalIndex)
                            {
                                continue;
                            }
                            else
                            {
                                foreach (KeyValuePair<string, InventoryGroup> group in GetGroups())
                                {
                                    if (!ignoreGroups.Contains(group.Key) && inputEventCache.TryGetValue(key, out Action<UnityEngine.InputSystem.InputAction.CallbackContext> action))
                                    {
                                        action.Invoke(new UnityEngine.InputSystem.InputAction.CallbackContext());
                                        return;
                                    }
                                }
                                continue;
                            }
                        }
                    }
                }
            }

        }
        #endregion

        #region [Event System Callbacks]
        /// <summary>
        /// Called when added new item in inventory system.
        /// </summary>
        public event Action<InventoryItem> OnAddedItemCallback;

        /// <summary>
        /// Called when removed specified item from inventory system.
        /// </summary>
        public event Action<InventoryItem> OnRemovedItemCallback;
        #endregion

        #region [Getter / Setter]
        public Options GetOptions()
        {
            return options;
        }

        public void SetOptions(Options value)
        {
            options = value;
        }
        #endregion
    }
}