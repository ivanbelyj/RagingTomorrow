/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.Serialization.Collections
{
    /// <summary>
    /// Serialization storage wrapper of generic type.
    /// </summary>
    /// <typeparam name="T">Type of storage.</typeparam>
    public class SerializationStorage<T> : SerializationStorageBase, ISerializationStorage<T>
    {
        // Storage data.
        [SerializeField] protected T storageData;

        /// <summary>
        /// Get storage data.
        /// </summary>
        /// <returns>Storage data type of (T)</returns>
        public T GetStorageData()
        {
            return storageData;
        }

        /// <summary>
        /// Set storage data.
        /// </summary>
        /// <param name="value">Storage data type of (T)</param>
        public void SetStorageData(T value)
        {
            storageData = value;
        }
    }
}