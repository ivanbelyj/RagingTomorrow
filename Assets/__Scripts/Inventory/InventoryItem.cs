using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Todo: сделать readonly
[System.Serializable]
public struct InventoryItem
{
    public int inventoryX;
    public int inventoryY;

    /// <summary>
    /// Каждый предмет инвентаря относится к его определенной секции.
    /// Хранение данного поля необходимо для обеспечения функциональности секций,
    /// а также для отображения предметов инвентаря в разных секциях UI.
    /// </summary>
    public int inventorySection;
    public int count;

    public ItemGameData itemGameData;

    // public int InventoryX { get => _inventoryX; }
    // public int InventoryY { get => _inventoryY; }
    // public int Count { get => _count; }
    // public string ItemDataName { get => _itemDataName; }

    // public InventoryItem(int inventoryX, int inventoryY, int count, string itemDataName) {
    //     _inventoryX = inventoryX;
    //     _inventoryY = inventoryY;
    //     _count = count;
    //     _itemDataName = itemDataName;
    // }
}
