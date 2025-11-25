/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory
{
    [System.Serializable]
    public sealed class InventoryGroup : IInventoryGroup
    {
        [SerializeField]
        private SlotDictionary slots;

        public InventoryGroup()
        {
            slots = new SlotDictionary();
        }

        public InventoryGroup(int allocateSpace)
        {
            slots = new SlotDictionary(allocateSpace);
        }

        public InventoryGroup(IDictionary<string, InventoryItem> slots)
        {
            slots = new SlotDictionary(slots);
        }

        /// <summary>
        /// Add new slot in inventory.
        /// </summary>
        /// <param name="input">Input key.</param>
        /// <param name="item">Inventory item.</param>
        /// <returns></returns>
        public void AddSlot(string input, InventoryItem item)
        {
            slots.Add(input, item);
        }

        /// <summary>
        /// Remove slot from inventory.
        /// </summary>
        /// <param name="input">Input key.</param>
        public bool RemoveSlot(string input)
        {
            return slots.Remove(input);
        }

        /// <summary>
        /// Add new item to group.
        /// </summary>
        /// <param name="item">InventoryItem reference.</param>
        /// <returns>
        /// Input name if group has a empty slot to add item and this item was added. 
        /// Otherwise null.
        /// </returns>
        public string AddItem(InventoryItem item)
        {
            string input = null;
            if (item != null)
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    if (slot.Value == null)
                    {
                        input = slot.Key;
                        break;
                    }
                }

                if(input != null)
                {
                    slots[input] = item;
                }
            }
            return input;
        }

        /// <summary>
        /// Add item to specific slot.
        /// </summary>
        /// <param name="item">InventoryItem reference.</param>
        /// <returns>
        /// True if group has a slot with specified input and this item was added. 
        /// Otherwise false.
        /// </returns>
        public bool AddItem(string input, InventoryItem item)
        {
            if (!string.IsNullOrEmpty(input) && slots.ContainsKey(input))
            {
                slots[input] = item;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove item from group.
        /// </summary>
        /// <param name="item">InventoryItem reference.</param>
        /// <returns>
        /// Input name if group contains slot with specified item. 
        /// Otherwise null.
        /// </returns>
        public string RemoveItem(InventoryItem item)
        {
            string input = null;
            if (item != null)
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    if (slot.Value == item)
                    {
                        input = slot.Key;
                        break;
                    }
                }
                if(input != null)
                {
                    slots[input] = null;
                }
            }
            return input;
        }

        /// <summary>
        /// Remove item from group.
        /// </summary>
        /// <param name="item">InventoryItem reference.</param>
        /// <returns>
        /// True if group contains slot with specified input name. 
        /// Otherwise false.
        /// </returns>
        public bool RemoveItem(string input)
        {
            if (!string.IsNullOrEmpty(input) && slots.ContainsKey(input))
            {
                slots[input] = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Replace first found slot with target item with new one.
        /// </summary>
        /// <param name="a">Target item to replace.</param>
        /// <param name="b">New item to replace.</param>
        public string ReplaceItem(InventoryItem a, InventoryItem b)
        {
            string input = null;
            foreach (KeyValuePair<string, InventoryItem> slot in slots)
            {
                if (slot.Value == a)
                {
                    input = slot.Key;
                    break;
                }
            }

            if (input != null)
            {
                slots[input] = b;
            }

            return input;
        }

        /// <summary>
        /// Determines group contains the specified input.
        /// </summary>
        public bool ContainsInput(string input)
        {
            return slots.ContainsKey(input);
        }

        /// <summary>
        /// Determines group contains the specified item.
        /// </summary>
        public bool ContainsItem(InventoryItem item)
        {
            return slots.ContainsValue(item);
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
        /// True if the slots contains an item with the specified input.
        /// Otherwise, false.
        /// </returns>
        public bool TryGetItem(string input, out InventoryItem item)
        {
            return slots.TryGetValue(input, out item);
        }

        /// <summary>
        /// Gets or sets the item associated with the specified input.
        /// </summary>
        /// <value>
        /// The value associated with the specified key.
        /// If the specified key is not found, a get operation throws a KeyNotFoundException,
        /// and a set operation creates a new element with the specified key.
        /// </value>
        public InventoryItem this[string input]
        {
            get
            {
                return slots[input];
            }

            set
            {
                slots[input] = value;
            }
        }

        #region [Iterable Properties]
        public IEnumerable<string> Inputs
        {
            get
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    yield return slot.Key;
                }
            }
        }

        public IEnumerable<string> FreeInputs
        {
            get
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    if (slot.Value == null)
                    {
                        yield return slot.Key;
                    }
                }
            }
        }

        public IEnumerable<string> AssignedInputs
        {
            get
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    if (slot.Value != null)
                    {
                        yield return slot.Key;
                    }
                }
            }
        }

        public IEnumerable<InventoryItem> Items
        {
            get
            {
                foreach (KeyValuePair<string, InventoryItem> slot in slots)
                {
                    yield return slot.Value;
                }
            }
        }
        #endregion

        #region [Getter / Setter]
        public SlotDictionary GetSlotDictionary()
        {
            return slots;
        }

        public void SetSlotDictionary(SlotDictionary value)
        {
            slots = value;
        }
        #endregion
    }
}