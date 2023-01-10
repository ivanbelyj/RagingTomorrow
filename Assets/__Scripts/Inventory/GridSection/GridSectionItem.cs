using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSectionItem : IEquatable<GridSectionItem>, ICountableItem
{
    public ItemData itemData;
    public int inventoryX;
    public int inventoryY;
    public int count;

    public bool Equals(GridSectionItem other)
    {
        return (this.itemData.Equals(other.itemData) &&
            this.inventoryX == other.inventoryX &&
            this.inventoryY == other.inventoryY);
    }

    public override string ToString()
    {
        return $"GridSectionItem: {itemData.ItemStaticDataName}; ({inventoryX}, {inventoryY}); " +
            $"count: {count}";
    }

    public int Count => count;

    /// <summary>
    /// В некоторых случаях требуется идентифицировать элемент в рамках секции инвентаря по его
    /// позиции в сетке. Максимальное число для uint - 4 294 967 295.
    /// local id составляется из двух чисел, значит, для каждого из них максимальное значение,
    /// чтобы оба числа можно было поместить в uint - 42_949
    /// (на практике требуются размеры инвентаря до 15).
    /// </summary>
    public uint GetLocalIdByInventoryPosition() {
        uint res = 100_000 * (uint)inventoryX + (uint)inventoryY;
        return res;
        // Для 40к: 4_000_000_000 + 40_000 = 4_000_040_000
    }
}
