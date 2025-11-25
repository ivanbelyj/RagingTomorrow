/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.InventoryModules
{
    [HideScriptField]
    public abstract class BaseItem : ScriptableObject, IBaseItem
    {
        // Base item properties.
        [SerializeField]
        [Label("Name")]
        [Tooltip("Unique name of this item.")]
        [NotEmpty]
        [Order(-999)]
        private string itemName;

        #region [Getter / Setter]
        public string GetItemName()
        {
            return itemName;
        }

        public void SetItemName(string itemName)
        {
            this.itemName = itemName;
        }
        #endregion
    }
}

