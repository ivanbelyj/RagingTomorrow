/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System;
using System.Collections;
using System.Collections.Generic;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Inventory System/Group Inventory System")]
    [DisallowMultipleComponent]
    public class GroupInventorySystem : InventorySystem
    {
        protected readonly Dictionary<string, Action<InputAction.CallbackContext>> inputEventCache = new Dictionary<string, Action<InputAction.CallbackContext>>();
        protected readonly Dictionary<string, InputAction> inputActionCache = new Dictionary<string, InputAction>();

        // Base group inventory system properties.
        [SerializeField]
        [Order(198)]
        private GroupDictionary groups = new GroupDictionary();

        [SerializeField]
        [Foldout("Advanced Settings", Style = "Header")]
        [Order(996)]
        private string startInputKey = string.Empty;

        // Stored required properties.
        private string lastUsedGroupId;
        private string lastUsedInputKey;

        /// <summary>
        /// Called on the frame when a script is enabled
        /// just before any of the Update methods are called the first time.
        /// </summary>
        protected override void Start()
        {
            base.Start();
            RegisterInputs();
            if (inputEventCache.TryGetValue(startInputKey, out Action<InputAction.CallbackContext> action))
            {
                action.Invoke(new InputAction.CallbackContext());
            }
        }

        /// <summary>
        /// Swap item.
        /// <br>If is equipped item equal by type with specified item, will swapped this two one.</br>
        /// <br>Otherwise will swapped first item with same type.</br>
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        /// <returns>True if item swapped. Otherwise false.</returns>
        public bool SwapItem(InventoryItem item)
        {
            return StartItemProcessing("Swap", item);
        }

        /// <summary>
        /// Determines the specified group contains free space.
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HasSpace(string groupId)
        {
            if (groups.TryGetValue(groupId, out InventoryGroup group))
            {
                IEnumerator enumerator = group.FreeInputs.GetEnumerator();
                return enumerator.MoveNext();
            }
            return false;
        }

        /// <summary>
        /// Gets the item associated with the specified input.
        /// </summary>
        /// <param name="key">The input of the item to get.</param>
        /// <param name="item">
        /// When this method returns, contains the item associated with the specified input, if the input is found.
        /// Otherwise, null. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if whether the group contains an item with the specified input.
        /// Otherwise, false.
        /// </returns>
        public bool TryGetItem(string input, out InventoryItem item)
        {
            item = null;
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                if (group.Value.ContainsInput(input))
                {
                    item = group.Value[input];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the group associated with the specified name.
        /// </summary>
        /// <param name="key">The name of the group to get.</param>
        /// <param name="group">
        /// When this method returns, contains the group associated with the specified name, if the group is found.
        /// Otherwise, null. 
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if the inventory contains an group with the specified name.
        /// Otherwise, false.
        /// </returns>
        public bool TryGetGroup(string groupId, out InventoryGroup group)
        {
            return groups.TryGetValue(groupId, out group);
        }

        /// <summary>
        /// Determines the inventory contains the specified group.
        /// </summary>
        public bool ContainsGroup(string name)
        {
            return groups.ContainsKey(name);
        }

        /// <summary>
        /// Determines whether the group contains the specified slot with input.
        /// </summary>
        public bool ContainsInput(string name)
        {
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                if (group.Value.ContainsInput(name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines the specified group contains the specified slot with input.
        /// </summary>
        public bool ContainsInput(string groupId, string name)
        {
            if (groups.TryGetValue(groupId, out InventoryGroup group))
            {
                return group.ContainsInput(name);
            }
            return false;
        }

        /// <summary>
        /// Determines whether the group contains the specified item.
        /// </summary>
        public bool ContainsItem(InventoryItem item)
        {
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                if (group.Value.ContainsItem(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines the specified group contains the specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool ContainsItem(string groupId, InventoryItem item)
        {
            if (groups.TryGetValue(groupId, out InventoryGroup group))
            {
                return group.ContainsItem(item);
            }
            return false;
        }

        /// <summary>
        /// Register all Inventory items to input action callback.
        /// Called once when the script instance is being loaded.
        /// </summary>
        protected virtual void RegisterInputs()
        {
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                foreach (KeyValuePair<string, InventoryItem> slot in group.Value.GetSlotDictionary())
                {
                    Action<InputAction.CallbackContext> eventCallback = (context) =>
                    {
                        if (lastUsedInputKey != slot.Key)
                        {
                            InventoryItem item = groups[group.Key][slot.Key];
                            if (item != null)
                            {
                                UseItem(item);
                                lastUsedGroupId = group.Key;
                                lastUsedInputKey = slot.Key;
                                OnSelectItemCallback?.Invoke(item);
                            }
                        }
                    };

                    InputAction action = InputReceiver.Asset.FindAction(slot.Key);
                    action.performed += eventCallback;
                    inputEventCache.Add(slot.Key, eventCallback);
                    inputActionCache.Add(slot.Key, action);
                }
            }
        }

        #region [IInventorySystem Implementation]
        /// <summary>
        /// Add new item in inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public override bool AddItem(InventoryItem item)
        {
            if (groups.TryGetValue(item.GetItemType(), out InventoryGroup group))
            {
                return group.AddItem(item) != null;
            }
            return false;
        }

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        public override bool RemoveItem(InventoryItem item)
        {
            if (groups.TryGetValue(item.GetItemType(), out InventoryGroup group))
            {
                return group.RemoveItem(item) != null;
            }
            return false;
        }
        #endregion

        #region [IInventoryIterator Implemetation]
        /// <summary>
        /// Iterate through all inventory items in inventory.
        /// </summary>
        public override IEnumerable<InventoryItem> Items
        {
            get
            {
                foreach (KeyValuePair<string, InventoryGroup> group in groups)
                {
                    SlotDictionary slots = group.Value.GetSlotDictionary();
                    foreach (KeyValuePair<string, InventoryItem> slot in slots)
                    {
                        InventoryItem item = slot.Value;
                        if (item != null)
                        {
                            yield return item;
                        }
                    }
                }
            }
        }
        #endregion

        #region [Override InventorySystem Methods]
        protected override IEnumerator ItemProcessing(string key, InventoryItem item)
        {
            yield return base.ItemProcessing(key, item);
            if (key == "Swap" && item is EquippableItem equippableItem)
            {
                OnSwapItemStartedCallback?.Invoke(equippableItem);
                if (GetEquippedItem() != null)
                {
                    if (IsEquipped())
                    {
                        yield return WaitForHideItem();
                    }
                }

                if (GetEquippedItem().CompareType(item))
                {
                    if (groups.TryGetValue(item.GetItemType(), out InventoryGroup group))
                    {
                        OnTossItem(GetEquippedItem());
                        string input = group.ReplaceItem(GetEquippedItem(), item);
                        lastUsedGroupId = item.GetItemType();
                        lastUsedInputKey = input;
                    }
                }
                else if (groups.TryGetValue(item.GetItemType(), out InventoryGroup group))
                {
                    IEnumerator enumerator = group.Inputs.GetEnumerator();
                    enumerator.MoveNext();
                    string input = enumerator.Current.ToString();

                    InventoryItem _item = group[input];
                    OnTossItem(_item);
                    group.AddItem(input, item);

                    lastUsedGroupId = item.GetItemType();
                    lastUsedInputKey = input;
                }


                yield return WaitForEquipItem(equippableItem);
                OnUseItem(item);
                OnSwapItemPerformedCallback?.Invoke(equippableItem);
            }
        }

        /// <summary>
        /// Try to find the item by name in the inventory. 
        /// If the inventory contains another item with the same name, return the first one.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <returns>InventoryItem reference if item founded in inventory. Otherwise null.</returns>
        public override InventoryItem FindItem(string name)
        {
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                SlotDictionary slots = group.Value.GetSlotDictionary();
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    InventoryItem storedItem = slot.Value;
                    if (storedItem != null && name == storedItem.GetItemName())
                    {
                        return storedItem;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find all the items by name in the inventory. 
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <returns>List of items with target name.</returns>
        public override List<InventoryItem> FindAllItem(string name)
        {
            List<InventoryItem> matchItems = null;
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                SlotDictionary slots = group.Value.GetSlotDictionary();
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    InventoryItem storedItem = slot.Value;
                    if (storedItem != null && name == storedItem.GetItemName())
                    {
                        if (matchItems == null)
                        {
                            matchItems = new List<InventoryItem>(1);
                        }
                        matchItems.Add(storedItem);
                    }
                }
            }
            return matchItems;
        }

        /// <summary>
        /// Try to find the item by type in the inventory. 
        /// If the inventory contains another item with the same type, return the first one.
        /// </summary>
        /// <returns>Item reference with target type.</returns>
        public override T FindItem<T>()
        {
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                SlotDictionary slots = group.Value.GetSlotDictionary();
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    T storedItem = slot.Value as T;
                    if (storedItem != null)
                    {
                        return storedItem;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Find all items with target type.
        /// </summary>
        /// <typeparam name="T">Search target type.</typeparam>
        /// <returns>List of items with target type.</returns>
        public override List<T> FindAllItem<T>()
        {
            List<T> items = null;
            foreach (KeyValuePair<string, InventoryGroup> group in groups)
            {
                SlotDictionary slots = group.Value.GetSlotDictionary();
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    T storedItem = slot.Value as T;
                    if (storedItem != null)
                    {
                        if (items == null)
                        {
                            items = new List<T>(1);
                        }
                        items.Add(storedItem);
                    }
                }
            }
            return items;
        }
        #endregion

        #region [Iterable Properties]
        /// <summary>
        /// Iterate through all inputs.
        /// </summary>
        public IEnumerable<string> Inputs
        {
            get
            {
                foreach (KeyValuePair<string, InventoryGroup> group in groups)
                {
                    foreach (KeyValuePair<string, InventoryItem> slot in group.Value.GetSlotDictionary())
                    {
                        yield return slot.Key;
                    }
                }
            }
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when selected new item.
        /// </summary>
        public event Action<InventoryItem> OnSelectItemCallback;

        /// <summary>
        /// Called when current item swapping to new one is started.
        /// </summary>
        public event Action<EquippableItem> OnSwapItemStartedCallback;

        /// <summary>
        /// Called when current item swapping to new one is performed.
        /// </summary>
        public event Action<EquippableItem> OnSwapItemPerformedCallback;
        #endregion

        #region [Getter / Setter]
        protected GroupDictionary GetGroups()
        {
            return groups;
        }

        protected void SetGroups(GroupDictionary value)
        {
            groups = value;
        }

        public string GetLastUsedGroupName()
        {
            return lastUsedGroupId;
        }

        public void SetLastUsedGroupName(string name)
        {
            lastUsedGroupId = name;
        }

        public string GetLastUsedInputKey()
        {
            return lastUsedInputKey;
        }

        public void SetLastUsedInputKey(string name)
        {
            lastUsedInputKey = name;
        }
        #endregion
    }
}