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
    public interface IBaseItem
    {
        /// <summary>
        /// Unique name of the item.
        /// </summary>
        /// <returns>Item name.</returns>
        string GetItemName();
    }
}