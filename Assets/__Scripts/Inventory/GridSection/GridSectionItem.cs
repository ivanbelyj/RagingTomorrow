using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSectionItem : IEquatable<GridSectionItem>, IInventoryItem
{
    // Todo: инкапсуляция
    public ItemData itemData;
    public int inventoryX;
    public int inventoryY;
    public int count;

    [SerializeField]
    private uint _inventoryNetId;

    /// <summary>
    /// Конструктор без параметров для десериализации. Внимание: используйте 
    /// конструктор с параметрами для установки обязательных закрытых полей
    /// </summary>
    public GridSectionItem() {

    }

    public GridSectionItem(uint inventoryNetId) {
        _inventoryNetId = inventoryNetId;    
    }

    public ItemData ItemData => itemData;

    // Todo: Equals от object
    public bool Equals(GridSectionItem other)
    {
        return this.itemData.Equals(other.itemData) && PlacementId.Equals(other.PlacementId);
        // return (this.itemData.Equals(other.itemData) &&
        //     this.inventoryX == other.inventoryX &&
        //     this.inventoryY == other.inventoryY);
    }

    public override string ToString()
    {
        return $"GridSectionItem: {itemData.ItemStaticDataName}; ({inventoryX}, {inventoryY}); " +
            $"count: {count}";
    }

    public int Count => count;

    public ItemPlacementId PlacementId => new ItemPlacementId(GetLocalIdByInventoryPosition(),
        _inventoryNetId);

    // Todo: private
    /// <summary>
    /// В некоторых случаях требуется идентифицировать элемент в рамках секции инвентаря по его
    /// позиции в сетке. Максимальное число для uint - 4 294 967 295.
    /// local id составляется из двух чисел, значит, для каждого из них максимальное значение,
    /// чтобы оба числа можно было поместить в uint - 42_949
    /// (на практике требуются размеры инвентаря до 15).
    /// </summary>
    private uint GetLocalIdByInventoryPosition() {
        return 100_000 * (uint)inventoryX + (uint)inventoryY;
        // Для 40к: 4_000_000_000 + 40_000 = 4_000_040_000
    }
}
