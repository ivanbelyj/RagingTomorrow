/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.Serialization.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.InventoryModules.GroupInventory
{
    [System.Serializable]
    public sealed class SlotDictionary : SerializableDictionary<string, InventoryItem>
    {
        [SerializeField]
        private string[] keys;

        [SerializeField]
        private InventoryItem[] values;

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public SlotDictionary() : base() { }

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that contains elements copied from the specified ISlotDictionary(string, InventoryItem) and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The ISlotDictionary(string, InventoryItem) whose elements are copied to the new SlotDictionary(string, InventoryItem).</param>
        public SlotDictionary(IDictionary<string, InventoryItem> dictionary) : base(dictionary) { }

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that contains elements copied from the specified ISlotDictionary(string, InventoryItem) and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public SlotDictionary(IEqualityComparer<string> comparer) : base(comparer) { }

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that is empty, has the default initial capacity, and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="capacity">The initial number of elements that the SlotDictionary(string, InventoryItem) can contain.</param>
        public SlotDictionary(int capacity) : base(capacity) { }

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The initial number of elements that the SlotDictionary(string, InventoryItem) can contain.</param>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public SlotDictionary(IDictionary<string, InventoryItem> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }

        /// <summary>
        /// Initializes a new instance of the SlotDictionary(string, InventoryItem) class that is empty, has the specified initial capacity, and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="capacity">The initial number of elements that the SlotDictionary(string, InventoryItem) can contain.</param>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public SlotDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }

        protected override string[] GetKeys()
        {
            return keys;
        }

        protected override InventoryItem[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(string[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(InventoryItem[] values)
        {
            this.values = values;
        }
    }
}