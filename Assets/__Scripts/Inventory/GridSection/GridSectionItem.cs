using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSectionItem : IInventoryItem
{
    [SerializeField]
    private ItemData _itemData;
    public ItemData ItemData { get => _itemData; set => _itemData = value; }

    [SerializeField]
    private int _inventoryX;
    public int InventoryX { get => _inventoryX; set => _inventoryX = value; }

    [SerializeField]
    private int _inventoryY;
    public int InventoryY { get => _inventoryY; set => _inventoryY = value;}

    [SerializeField]
    private int _count;
    public int Count { get => _count; set => _count = value;}

    [SerializeField]
    private uint _inventoryNetId;
    public uint InventoryNetId { get => _inventoryNetId; set => _inventoryNetId = value; }

    public override bool Equals(object obj)
    {
        return obj is GridSectionItem other ? this.ItemData.Equals(other.ItemData)
            && PlacementId.Equals(other.PlacementId) : false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_itemData.GetHashCode(), _inventoryX, _inventoryY, _count,
            _inventoryNetId);
    }

    public override string ToString()
    {
        return $"GridSectionItem. ItemData: {ItemData}; ({InventoryX}, {InventoryY}); " +
            $"count: {Count}; Placement: {PlacementId}";
    }

    public ItemPlacementId PlacementId => new ItemPlacementId() {
        LocalId = GetLocalIdByInventoryPosition(),
        InventorySectionNetId = _inventoryNetId
    };

    /// <summary>
    /// В некоторых случаях требуется идентифицировать элемент в рамках секции инвентаря по его
    /// позиции в сетке. Максимальное число для uint - 4 294 967 295.
    /// local id составляется из двух чисел, значит, для каждого из них максимальное значение,
    /// чтобы оба числа можно было поместить в uint - 42_949
    /// (на практике требуются размеры инвентаря до 15).
    /// </summary>
    private uint GetLocalIdByInventoryPosition() {
        return 100_000 * (uint)InventoryX + (uint)InventoryY;
        // Для 40к: 4_000_000_000 + 40_000 = 4_000_040_000
    }
}
