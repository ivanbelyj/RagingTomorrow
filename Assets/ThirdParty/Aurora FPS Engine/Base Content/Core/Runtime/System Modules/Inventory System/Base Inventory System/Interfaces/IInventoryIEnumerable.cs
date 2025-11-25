/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    public interface IInventoryIEnumerable<T>
    {
        /// <summary>
        /// Iterate through all inventory items in inventory.
        /// </summary>
        IEnumerable<T> Items { get; }
    }
}