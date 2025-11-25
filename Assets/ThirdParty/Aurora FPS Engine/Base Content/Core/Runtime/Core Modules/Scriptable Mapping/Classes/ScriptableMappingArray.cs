/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules
{
    /// <summary>
    /// Represents base class for all scriptable mapping based on default C# array.
    /// 
    /// Computational complexity: O(n).
    /// </summary>
    /// <typeparam name="T">The type that will be contained in the mapping.</typeparam>
    public class ScriptableMappingArray<T> : ScriptableMapping
    {
        [SerializeField] protected T[] mappingValues;

        /// <summary>
        /// Get all mapping values.
        /// </summary>
        public T[] GetMappingValues()
        {
            return mappingValues;
        }

        /// <summary>
        /// Set mapping values range.
        /// </summary>
        public void SetMappingValues(T[] value)
        {
            mappingValues = value;
        }

        /// <summary>
        /// Get value from mapping by index.
        /// </summary>
        public T GetMappingValue(int index)
        {
            return index < mappingValues.Length ? mappingValues[index] : default(T);
        }

        /// <summary>
        /// Set value in mapping by index.
        /// </summary>
        public void SetMappingValue(int index, T value)
        {
            if (index < mappingValues.Length)
            {
                mappingValues[index] = value;
            }
        }

        /// <summary>
        /// Get mapping length.
        /// </summary>
        public override int GetMappingLength()
        {
            return mappingValues != null ? mappingValues.Length : 0;
        }
    }
}