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
    public sealed class GroupDictionary : SerializableDictionary<string, InventoryGroup>
    {
        [SerializeField]
        private string[] keys;

        [SerializeField]
        private InventoryGroup[] values;

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public GroupDictionary() : base() { }

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that contains elements copied from the specified IGroupDictionary(string, InventoryGroup) and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The IGroupDictionary(string, InventoryGroup) whose elements are copied to the new GroupDictionary(string, InventoryGroup).</param>
        public GroupDictionary(IDictionary<string, InventoryGroup> dictionary) : base(dictionary) { }

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that contains elements copied from the specified IGroupDictionary(string, InventoryGroup) and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public GroupDictionary(IEqualityComparer<string> comparer) : base(comparer) { }

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that is empty, has the default initial capacity, and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="capacity">The initial number of elements that the GroupDictionary(string, InventoryGroup) can contain.</param>
        public GroupDictionary(int capacity) : base(capacity) { }

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that is empty, has the specified initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="dictionary">The initial number of elements that the GroupDictionary(string, InventoryGroup) can contain.</param>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public GroupDictionary(IDictionary<string, InventoryGroup> dictionary, IEqualityComparer<string> comparer) : base(dictionary, comparer) { }

        /// <summary>
        /// Initializes a new instance of the GroupDictionary(string, InventoryGroup) class that is empty, has the specified initial capacity, and uses the specified IEqualityComparer(T).
        /// </summary>
        /// <param name="capacity">The initial number of elements that the GroupDictionary(string, InventoryGroup) can contain.</param>
        /// <param name="comparer">The IEqualityComparer(T) implementation to use when comparing keys, or null to use the default EqualityComparer(T) for the type of the key.</param>
        public GroupDictionary(int capacity, IEqualityComparer<string> comparer) : base(capacity, comparer) { }

        protected override string[] GetKeys()
        {
            return keys;
        }

        protected override InventoryGroup[] GetValues()
        {
            return values;
        }

        protected override void SetKeys(string[] keys)
        {
            this.keys = keys;
        }

        protected override void SetValues(InventoryGroup[] values)
        {
            this.values = values;
        }
    }
}