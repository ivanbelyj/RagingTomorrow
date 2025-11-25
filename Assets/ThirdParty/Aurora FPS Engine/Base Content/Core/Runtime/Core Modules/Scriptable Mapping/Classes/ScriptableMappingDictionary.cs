/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using AuroraFPSRuntime.CoreModules.Serialization.Collections;

namespace AuroraFPSRuntime.CoreModules
{
    /// <summary>
    /// Represents base class for all scriptable mapping based on generic C# dictionary.
    /// 
    /// Computational complexity: O(1).
    /// </summary>
    /// <typeparam name="TDictionary">The subclass of SerializableDictionary.</typeparam>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    public class ScriptableMappingDictionary<TDictionary, TKey, TValue> : ScriptableMapping where TDictionary : SerializableDictionary<TKey, TValue>
    {
        [SerializeField] protected TDictionary mapping;

        /// <summary>
        /// Add new value in mapping.
        /// </summary>
        /// <returns>True if the value is successfully added. False if key already contains in mapping.</returns>
        public bool Add(TKey key, TValue value)
        {
            if (!mapping.ContainsKey(key))
            {
                mapping.Add(key, value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove mapping value by key (key removed too).
        /// <returns>True if the value is successfully removed. False if key not found in mapping.</returns>
        public bool Remove(TKey key)
        {
            if (mapping.ContainsKey(key))
            {
                mapping.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clear mapping keys and values.
        /// </summary>
        public void ClearMapping()
        {
            mapping.Clear();
        }

        /// <summary>
        /// Check whether this key is contained in the mapping. 
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return mapping.ContainsKey(key);
        }

        /// <summary>
        /// Get value by key from mapping.
        /// </summary>
        public TValue GetValue(TKey key)
        {
            return mapping[key];
        }

        /// <summary>
        /// Try 
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return mapping.TryGetValue(key, out value);
        }

        public TDictionary GetMapping()
        {
            return mapping;
        }

        public void SetMapping(TDictionary value)
        {
            mapping = value;
        }

        /// <summary>
        /// Set value by key.
        /// </summary>
        public void SetValue(TKey key, TValue value)
        {
            mapping[key] = value;
        }

        /// <summary>
        /// Get mapping length.
        /// </summary>
        public override int GetMappingLength()
        {
            return mapping.Count;
        }
    }
}