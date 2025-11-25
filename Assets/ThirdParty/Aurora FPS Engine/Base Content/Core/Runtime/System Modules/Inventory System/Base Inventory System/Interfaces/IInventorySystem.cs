/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    public interface IInventorySystem<T>
    {
        /// <summary>
        /// Add new item in inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        bool AddItem(T item);

        /// <summary>
        /// Remove item from inventory.
        /// </summary>
        /// <param name="item">Inventory item reference.</param>
        bool RemoveItem(T item);
    }
}